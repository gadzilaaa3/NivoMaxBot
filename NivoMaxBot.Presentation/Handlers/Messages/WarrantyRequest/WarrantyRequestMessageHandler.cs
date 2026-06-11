using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using NivoMaxBot.Application.Common.Exceptions;
using NivoMaxBot.Application.Features.WarrantyRequest.Commands.Create;
using NivoMaxBot.Application.Features.WarrantyRequest.Queries.ById;
using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.ErrorHandling;
using NivoMaxBot.Messaging.Handlers.Interfaces;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;
using NivoMaxBot.Presentation.Handlers.Callbacks.User;
using NivoMaxBot.Shared.Services;
using static NivoMaxBot.Presentation.Handlers.Callbacks.User.WarrantyRequest.Create.WarrantyRequestCreateHandler;

namespace NivoMaxBot.Presentation.Handlers.Messages.WarrantyRequest
{
    public class WarrantyRequestMessageHandler : IMessageHandler
    {
        private readonly IUserStateService _userStateService;
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly INotificationService _notificationService;
        private readonly ILogger<WarrantyRequestMessageHandler> _logger;
        private readonly IMenuBuilder _menuBuilder;
        private readonly IErrorHandler _errorHandler;

        public WarrantyRequestMessageHandler(
            IUserStateService userStateService,
            IMediator mediator,
            IMessengerClient botClient,
            INotificationService notificationService,
            ILogger<WarrantyRequestMessageHandler> logger,
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

        public async Task HandleAsync(IMessage message, CancellationToken ct)
        {
            var userId = message.From.Id;
            var chatId = message.ChatId.Value;
            var state = _userStateService.GetState(userId);
            var data = (CreateWarrantyData)state.TypedData;

            try
            {
                await (data.CurrentStep switch
                {
                    CreateWarrantyStep.ContactPerson => HandleContactPersonStep(message, data,
                        chatId, ct),
                    CreateWarrantyStep.ContactPhone => HandleContactPhoneStep(message, data, chatId, ct),
                    CreateWarrantyStep.City => HandleCityStep(message, data, chatId, ct),
                    CreateWarrantyStep.ProductSerialNumber => HandleProductSerialNumberStep(message,
                        data, chatId, ct),
                    CreateWarrantyStep.INN => HandleINNStep(message, data, chatId, ct),
                    CreateWarrantyStep.ContactEmail => HandleContactEmailStep(message,
                        data, chatId, ct),
                    CreateWarrantyStep.ProblemDescription => HandleProblemDescriptionStep(message,
                        data, chatId, ct),
                    _ => throw new IncorrectStateException("Ошибка состояния")
                });

                _userStateService.SetState(userId, state);
            }
            catch (ValidationException ex)
            {
                _userStateService.ClearState(userId);

                var button = new InlineKeyboardButton("Попробовать снова",
                    UserModeRoutes.WarrantyCreate);

                var keyboard = _menuBuilder.AddControlButtons([[button]], null, MenuType.User);

                await _errorHandler.HandleError(chatId, ex, keyboard, ct);
            }
            catch (Exception ex)
            {
                _userStateService.ClearState(userId);

                _logger.LogError(ex, "Error creating warranty request");

                var keyboard = _menuBuilder.AddControlButtons([], null, MenuType.User);
                await _errorHandler.HandleError(chatId, ex, keyboard, ct);
            }
        }

        public async Task HandleContactPersonStep(IMessage message, CreateWarrantyData data, 
            long chatId, CancellationToken cancellationToken)
        {
            data.ContactPerson = message.Text;
            data.CurrentStep = CreateWarrantyStep.ContactPhone;
            await _botClient.SendTextMessageAsync(chatId, "Введите контактный телефон:", 
                ct: cancellationToken);
        }

        public async Task HandleContactPhoneStep(IMessage message, CreateWarrantyData data,
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
            data.CurrentStep = CreateWarrantyStep.City;

            await _botClient.SendTextMessageAsync(chatId, "Введите город:", ct: cancellationToken);
        }

        public async Task HandleCityStep(IMessage message, CreateWarrantyData data,
            long chatId, CancellationToken cancellationToken)
        {
            data.City = message.Text;
            data.CurrentStep = CreateWarrantyStep.ProductSerialNumber;
            await _botClient.SendTextMessageAsync(chatId, "Введите серийный номер товара:", 
                ct: cancellationToken);
        }

        public async Task HandleProductSerialNumberStep(IMessage message, CreateWarrantyData data,
            long chatId, CancellationToken cancellationToken)
        {
            data.ProductSerialNumber = message.Text;
            data.CurrentStep = CreateWarrantyStep.INN;

            await _botClient.SendTextMessageAsync(chatId, "Введите ИНН (или '-' чтобы пропустить):", ct: cancellationToken);
        }

        public async Task HandleINNStep(IMessage message, CreateWarrantyData data,
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
            data.CurrentStep = CreateWarrantyStep.ContactEmail;

            await _botClient.SendTextMessageAsync(chatId, "Введите email:", 
                ct: cancellationToken);
        }

        public async Task HandleContactEmailStep(IMessage message, CreateWarrantyData data,
            long chatId, CancellationToken cancellationToken)
        {
            var emailText = message.Text;
            var isValid = EmailValidator.IsValidEmail(emailText);

            if (isValid)
            {
                data.ContactEmail = emailText!;
            }
            else
            {
                await _botClient.SendTextMessageAsync(chatId, "Неверный формат email адреса (Пример: example@mail.ru)\n" +
                    "Попробуйте еще раз.");
                return;
            }

            data.CurrentStep = CreateWarrantyStep.ProblemDescription;

            await _botClient.SendTextMessageAsync(chatId, "Опишите проблему (поломка, дефект):", 
                ct: cancellationToken);
        }

        public async Task HandleProblemDescriptionStep(IMessage message, CreateWarrantyData data,
            long chatId, CancellationToken cancellationToken)
        {
            data.ProblemDescription = message.Text;
            await HandleSubmitRequest(message, data, chatId, cancellationToken);
        }

        public async Task HandleSubmitRequest(IMessage message, CreateWarrantyData data,
            long chatId, CancellationToken cancellationToken)
        {
            // Собираем команду
            var command = new CreateWarrantyRequestCommand
            {
                UserMaxId = data.UserMessengerId,
                City = data.City,
                ContactEmail = data.ContactEmail,
                ContactPerson = data.ContactPerson,
                ContactPhone = data.ContactPhone,
                INN = data.INN,
                ProblemDescription = data.ProblemDescription,
                ProductSerialNumber = data.ProductSerialNumber,
            };

            var requestId = await _mediator.Send(command, cancellationToken);
            _logger.LogInformation("Warranty request created with id {RequestId}", requestId);

            // Получаем DTO для уведомления
            var requestDto = await _mediator.Send(new GetWarrantyRequestByIdQuery { Id = requestId }, 
                cancellationToken);
            await _notificationService.SendWarrantyRequestNotification(requestDto, cancellationToken);

            var keyboard = _menuBuilder.AddControlButtons([], UserModeRoutes.Profile, MenuType.User);

            await _botClient.SendTextMessageAsync(chatId,
                "✅ Гарантийная заявка принята! Менеджер свяжется с вами в ближайшее время.",
                replyMarkup: keyboard, ct: cancellationToken);
        }
    }
}
