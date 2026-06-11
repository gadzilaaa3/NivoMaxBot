using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using NivoMaxBot.Application.Common.Exceptions;
using NivoMaxBot.Application.Features.Orders.Commands.Create;
using NivoMaxBot.Application.Features.Orders.Queries;
using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Abstractions.Types.Enums;
using NivoMaxBot.Messaging.ErrorHandling;
using NivoMaxBot.Messaging.Handlers.Interfaces;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;
using NivoMaxBot.Presentation.Handlers.Callbacks.User;
using NivoMaxBot.Shared.Services;
using static NivoMaxBot.Presentation.Handlers.Callbacks.User.Order.Create.OrderCreateHandler;

namespace NivoMaxBot.Presentation.Handlers.Messages.Orders.Create
{
    public class OrderCreateMessageHandler : IMessageHandler
    {
        private readonly IUserStateService _userStateService;
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly INotificationService _notificationService;
        private readonly ILogger<OrderCreateMessageHandler> _logger;
        private readonly IMenuBuilder _menuBuilder;
        private readonly IErrorHandler _errorHandler;

        public OrderCreateMessageHandler(
            IUserStateService userStateService,
            IMediator mediator,
            IMessengerClient botClient,
            INotificationService notificationService,
            ILogger<OrderCreateMessageHandler> logger,
            IMenuBuilder menuBuilder,
            IErrorHandler errorHandler)
        {
            _userStateService = userStateService;
            _mediator = mediator;
            _botClient = botClient;
            _notificationService = notificationService;
            _logger = logger;
            _menuBuilder = menuBuilder;
            _errorHandler = errorHandler;
        }
        public bool CanHandle(IMessage message)
        {
            var state = _userStateService.GetState(message.From.Id);
            return state.CurrentAction == ActionName;
        }

        public async Task HandleAsync(IMessage message, CancellationToken cancellationToken)
        {
            var userId = message.From.Id;
            var chatId = message.ChatId.Value;
            var state = _userStateService.GetState(userId);
            var data = (CreateOrderData)state.TypedData;

            try
            {
                await (data.CurrentStep switch
                {
                    CreateOrderStep.CustomerPerson => HandleCustomerPersonStep(message, data, 
                        chatId, cancellationToken),
                    CreateOrderStep.ContactPhone => HandleContactPhoneStep(message, data, 
                        chatId, cancellationToken),
                    CreateOrderStep.ContactEmail => HandleContactEmailStep(message, data,
                        chatId, cancellationToken),
                    CreateOrderStep.INN => HandleINNStep(message, data, chatId, 
                        cancellationToken),
                    _ => throw new IncorrectStateException("Ошибка состояния")
                });

                _userStateService.SetState(userId, state);
            }
            catch (ValidationException ex)
            {
                _userStateService.ClearState(userId);

                var button = new InlineKeyboardButton("Попробовать снова",
                    UserModeRoutes.OrderCreate);

                var keyboard = _menuBuilder.AddControlButtons([[button]], null, MenuType.User);

                await _errorHandler.HandleError(chatId, ex, keyboard, cancellationToken);
            }
            catch (Exception ex)
            {
                _userStateService.ClearState(userId);

                _logger.LogError(ex, "Error creating order");

                var keyboard = _menuBuilder.AddControlButtons([], null, MenuType.User);
                await _errorHandler.HandleError(chatId, ex, keyboard, cancellationToken);
            }
        }

        public async Task HandleCustomerPersonStep(IMessage message, CreateOrderData data,
            long chatId, CancellationToken cancellationToken)
        {
            data.CustomerName = message.Text;
            data.CurrentStep = CreateOrderStep.ContactPhone;

            await _botClient.SendTextMessageAsync(chatId, "Введите контактный телефон:", 
                ct: cancellationToken);
        }
        public async Task HandleContactPhoneStep(IMessage message, CreateOrderData data,
            long chatId, CancellationToken cancellationToken)
        {
            var phone = message.Text;
            var isValid = PhoneValidator.Validate(phone);
            if (!isValid)
            {
                await _botClient.SendTextMessageAsync(chatId, "Номер телефона должен содержать ровно 11 цифр " +
                    "(например, +7 (988) 888-88-88 или 89888888888).\n" +
                    "Попробуйте еще раз", ct: cancellationToken);
                return;
            }
            data.ContactPhone = phone;
            data.CurrentStep = CreateOrderStep.ContactEmail;

            await _botClient.SendTextMessageAsync(chatId, "Введите email (или '-' чтобы пропустить):", ct: cancellationToken);
        }
        public async Task HandleContactEmailStep(IMessage message, CreateOrderData data,
            long chatId, CancellationToken cancellationToken)
        {
            string? emailText = message.Text;
            if (emailText == "-")
            {
                emailText = null;
            }
            else
            {
                var isValid = EmailValidator.IsValidEmail(emailText);

                if (isValid)
                {
                    data.ContactEmail = emailText;
                }
                else
                {
                    await _botClient.SendTextMessageAsync(chatId, "Неверный формат email адреса (Пример: example@mail.ru)\n" +
                        "Попробуйте еще раз.");
                    return;
                }
            }

            data.ContactEmail = emailText;
            data.CurrentStep = CreateOrderStep.INN;

            await _botClient.SendTextMessageAsync(chatId, "Введите ИНН (или '-' чтобы пропустить):", ct: cancellationToken);
        }
        public async Task HandleINNStep(IMessage message, CreateOrderData data,
            long chatId, CancellationToken cancellationToken)
        {
            string? inn = message.Text;
            if (inn == "-")
            {
                inn = null;
            }
            // Простейшая валидация
            else if (!System.Text.RegularExpressions.Regex.IsMatch(message.Text, @"^\d{10,12}$"))
            {
                await _botClient.SendTextMessageAsync(chatId,
                    "❌ ИНН должен содержать 10 или 12 цифр. Попробуйте ещё раз:",
                    ct: cancellationToken);
                return;
            }
            data.INN = inn;
            await HandleSubmitOrder(message, data, chatId, cancellationToken);
        }
        public async Task HandleSubmitOrder(IMessage message, CreateOrderData data,
            long chatId, CancellationToken cancellationToken)
        {
            var keyboard = _menuBuilder.AddControlButtons([], UserModeRoutes.Profile, MenuType.User);

            var command = new CreateOrderCommand
            {
                UserMaxId = data.UserMessengerId,
                ContactEmail = data.ContactEmail,
                ContactPhone = data.ContactPhone,
                CustomerName = data.CustomerName,
                INN = data.INN,
            };
            var orderId = await _mediator.Send(command, cancellationToken);
            var order = await _mediator.Send(new GetOrderByIdQuery { OrderId = orderId }, 
                cancellationToken);

            await _notificationService.SendOrderNotification(order, cancellationToken);

            // Показываем подтверждение пользователю
            var text = $"✅ *Заказ #{order.Id} оформлен!*\n\n" +
                       $"Состав заказа:\n";
            foreach (var item in order.Items)
                text += $"{item.ProductName} x {item.Quantity} = {item.Total} руб.\n";
            text += $"\n*Итого: {order.TotalAmount} руб.*\n\n" +
                    "Менеджер свяжется с вами в ближайшее время.";

            await _botClient.SendTextMessageAsync(message.ChatId.Value, text, 
                textFormat: TextFormat.Markdown, replyMarkup: keyboard, 
                ct: cancellationToken);
        }
    }
}
