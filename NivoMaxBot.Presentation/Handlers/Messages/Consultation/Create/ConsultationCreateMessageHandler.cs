using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using NivoMaxBot.Application.Common.Exceptions;
using NivoMaxBot.Application.Features.ConsultationRequests.Commands.Create;
using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Abstractions.Types.Enums;
using NivoMaxBot.Messaging.ErrorHandling;
using NivoMaxBot.Messaging.Handlers.Interfaces;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;
using NivoMaxBot.Presentation.Handlers.Callbacks.User;
using NivoMaxBot.Presentation.Handlers.Callbacks.User.Catalog;
using NivoMaxBot.Presentation.Handlers.Callbacks.User.Consultation.Create;
using NivoMaxBot.Shared.Services;
using static NivoMaxBot.Presentation.Handlers.Callbacks.User.Consultation.Create.ConsultationCreateHandler;

namespace NivoMaxBot.Presentation.Handlers.Messages.Consultation.Create
{
    public class ConsultationCreateMessageHandler : IMessageHandler
    {
        private readonly IUserStateService _userStateService;
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly INotificationService _notificationService;
        private readonly ILogger<ConsultationCreateMessageHandler> _logger;
        private readonly IMenuBuilder _menuBuilder;
        private readonly IErrorHandler _errorHandler;

        public ConsultationCreateMessageHandler(
            IUserStateService userStateService,
            IMediator mediator,
            IMessengerClient botClient,
            INotificationService notificationService,
            ILogger<ConsultationCreateMessageHandler> logger,
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
            return state.CurrentAction == ConsultationCreateHandler.ActionName;
        }

        public async Task HandleAsync(IMessage message, CancellationToken cancellationToken)
        {
            var userId = message.From.Id;
            var chatId = message.ChatId.Value;
            var state = _userStateService.GetState(userId);
            var data = (CreateConsultationData)state.TypedData;

            try
            {
                await (data.CurrentStep switch
                {
                    CreateConsultationStep.ContactName => HandleContactNameStep(message, data, chatId, cancellationToken),
                    CreateConsultationStep.City => HandleCityStep(message, data, chatId, cancellationToken),
                    CreateConsultationStep.PhoneNumber => HandlePhoneNumberStep(message, data, chatId, cancellationToken),
                    CreateConsultationStep.Description => HandleDescriptionStep(message, data, chatId, cancellationToken),
                    _ => throw new IncorrectStateException("Ошибка состояния")
                });

                _userStateService.SetState(userId, state);
            }
            catch (ValidationException ex)
            {
                _userStateService.ClearState(userId);
                var button = new InlineKeyboardButton("Попробовать снова", UserModeRoutes.ConsultationCreate);
                var keyboard = _menuBuilder.AddControlButtons([[button]], null, MenuType.User);
                await _errorHandler.HandleError(chatId, ex, keyboard, cancellationToken);
            }
            catch (Exception ex)
            {
                _userStateService.ClearState(userId);
                _logger.LogError(ex, "Error creating consultation request");
                var keyboard = _menuBuilder.AddControlButtons([], null, MenuType.User);
                await _errorHandler.HandleError(chatId, ex, keyboard, cancellationToken);
            }
        }

        private async Task HandleContactNameStep(IMessage message, CreateConsultationData data, long chatId, CancellationToken ct)
        {
            data.ContactName = message.Text;
            data.CurrentStep = CreateConsultationStep.City;
            await _botClient.SendTextMessageAsync(chatId, "Введите ваш город:", ct: ct);
        }

        private async Task HandleCityStep(IMessage message, CreateConsultationData data, long chatId, CancellationToken ct)
        {
            data.City = message.Text;
            data.CurrentStep = CreateConsultationStep.PhoneNumber;
            await _botClient.SendTextMessageAsync(chatId, "Введите контактный телефон:", ct: ct);
        }

        private async Task HandlePhoneNumberStep(IMessage message, CreateConsultationData data, long chatId, CancellationToken ct)
        {
            var phone = message.Text;
            var isValid = PhoneValidator.Validate(phone);
            if (!isValid)
            {
                await _botClient.SendTextMessageAsync(chatId,
                    "Номер телефона должен содержать ровно 11 цифр (например, +7 (988) 888-88-88 или 89888888888).\nПопробуйте еще раз:",
                    ct: ct);
                return;
            }
            data.PhoneNumber = phone;
            data.CurrentStep = CreateConsultationStep.Description;
            await _botClient.SendTextMessageAsync(chatId, "Опишите, по какому вопросу требуется консультация:",
                ct: ct);
        }

        private async Task HandleDescriptionStep(IMessage message, CreateConsultationData data, long chatId, CancellationToken ct)
        {
            var description = message.Text;
            data.Description = description;

            await SubmitConsultation(message, data, chatId, ct);
        }

        private async Task SubmitConsultation(IMessage message, CreateConsultationData data, long chatId, CancellationToken ct)
        {
            var command = new CreateConsultationCommand
            {
                UserMaxId = data.UserMessengerId,
                ContactName = data.ContactName,
                City = data.City,
                PhoneNumber = data.PhoneNumber,
                Description = data.Description
            };

            var requestId = await _mediator.Send(command, ct);

            var text = $"✅ *Заявка на консультацию #{requestId} отправлена!*\n\n" +
                       "Менеджер свяжется с вами в ближайшее время.";

            var keyboard = _menuBuilder.AddControlButtons([], UserCatalogRoutes.CatalogRoot, MenuType.User);
            await _botClient.SendTextMessageAsync(chatId, text, textFormat: TextFormat.Markdown, 
                replyMarkup: keyboard, ct: ct);
        }
    }
}
