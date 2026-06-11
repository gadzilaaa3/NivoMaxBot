using MediatR;
using Microsoft.Extensions.Logging;
using NivoMaxBot.Application.Features.Products.Dtos;
using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Handlers.Interfaces;

namespace NivoMaxBot.Presentation.Handlers.Messages.Products.Admin.Update
{
    public class ProductEditMessageHandler : IMessageHandler
    {
        private readonly IUserStateService _userStateService;
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly ILogger<ProductEditMessageHandler> _logger;

        public ProductEditMessageHandler(
            IUserStateService userStateService,
            IMediator mediator,
            IMessengerClient botClient,
            ILogger<ProductEditMessageHandler> logger)
        {
            _userStateService = userStateService;
            _mediator = mediator;
            _botClient = botClient;
            _logger = logger;
        }

        public bool CanHandle(IMessage message)
        {
            var state = _userStateService.GetState(message.From.Id);
            return state.CurrentAction == "EditingProduct";
        }

        public async Task HandleAsync(IMessage message, CancellationToken ct)
        {
            var userId = message.From.Id;
            var chatId = message.ChatId.Value;
            var state = _userStateService.GetState(userId);
            var original = state.Data["original"] as ProductDto;

            try
            {
                if (state.Step == 1) // название
                {
                    state.Data["name"] = message.Text == "-" ? original.Name : message.Text;
                    state.Step = 2;
                    await _botClient.SendTextMessageAsync(
                        chatId,
                        $"Текущее описание: {original.Description ?? "—"}" +
                        $"\nВведите новое описание (или отправьте '-' чтобы оставить, или '.' чтобы очистить):",
                        ct: ct);
                }
                else if (state.Step == 2) // описание
                {
                    string? newDescription;
                    if (message.Text == "-")
                        newDescription = original.Description;
                    else if (message.Text == ".")
                        newDescription = null;
                    else
                        newDescription = message.Text;

                    state.Data["description"] = newDescription;
                    state.Step = 3;
                    await _botClient.SendTextMessageAsync(
                        chatId,
                        $"Текущая цена: {original.Price}\nВведите новую цену (число, или '-' для пропуска):",
                        ct: ct);
                }
                else if (state.Step == 3) // цена
                {
                    decimal newPrice;
                    if (message.Text == "-")
                        newPrice = original.Price;
                    else if (!decimal.TryParse(message.Text, out newPrice) || newPrice <= 0)
                    {
                        await _botClient.SendTextMessageAsync(chatId,
                            "Цена должна быть положительным числом. Попробуйте ещё раз:", ct: ct);
                        return;
                    }
                    state.Data["price"] = newPrice;
                    state.Step = 4;
                    await _botClient.SendTextMessageAsync(
                        chatId,
                        $"Текущая гарантия: {original.WarrantyInMonths} мес." +
                        $"\nВведите новую гарантию (число, 0 если нет, '-' для пропуска.\n" +
                        $"Числа больше 1000 будут отображаться как пожизненная гарантия):",
                        ct: ct);
                }
                else if (state.Step == 4) // гарантия
                {
                    int newWarranty;
                    if (message.Text == "-")
                        newWarranty = original.WarrantyInMonths;
                    else if (!int.TryParse(message.Text, out newWarranty) || newWarranty < 0)
                    {
                        await _botClient.SendTextMessageAsync(chatId,
                            "Гарантия должна быть неотрицательным числом. Попробуйте ещё раз:", ct: ct);
                        return;
                    }
                    state.Data["warranty"] = newWarranty;
                    state.Step = 5;
                    await _botClient.SendTextMessageAsync(
                        chatId,
                        "Отправьте новое фото или ссылку на фото " +
                        "(отправьте '-' чтобы оставить текущее, '.' чтобы удалить фото):",
                        ct: ct);
                }
                else if (state.Step == 5) // фото
                {
                    string? newPhoto;
                    state.Data["photoUrl"] = original.PhotoUrl;
                    if (message.Photo != null)
                    {
                        newPhoto = message.Photo.FileId;
                    }
                    else if (message.Text == "-")
                    {
                        newPhoto = original.PhotoMaxFileId;
                    }
                    else if (message.Text == ".")
                    {
                        state.Data["photoUrl"] = null;
                        newPhoto = null;
                    }
                    else if (message.Text != null)
                    {
                        state.Data["photoUrl"] = message.Text;
                        newPhoto = original.PhotoMaxFileId;
                    }
                    else
                    {
                        await _botClient.SendTextMessageAsync(chatId, "Пожалуйста, отправьте фото, '-' или '.'", ct: ct);
                        return;
                    }
                    state.Data["photo"] = newPhoto;
                    state.Step = 6;
                    await _botClient.SendTextMessageAsync(
                        chatId,
                        $"Товар доступен для пользователей? (да/нет, текущее значение: {(original.IsAvailable ? "да" : "нет")}):",
                        ct: ct);
                }
                else if (state.Step == 6) // доступность
                {
                    bool newIsAvailable;
                    if (message.Text == "-")
                        newIsAvailable = original.IsAvailable;
                    else
                        newIsAvailable = message.Text?.ToLower() != "нет";

                    state.Data["isAvailable"] = newIsAvailable;
                    state.Step = 7;

                    // Показываем текущие категории и предлагаем выбрать действие
                    var categoriesList = string.Join(", ", original.CategoryNames);
                    
                    var buttons = new List<List<InlineKeyboardButton>>
                    {
                        ([
                            new InlineKeyboardButton("➕ Добавить категорию",
                            $"product:edit:addcategory:{original.Id}")
                        ])
                    };

                    if (original.CategoryIds.Count() > 1)
                        buttons.Add([
                            new InlineKeyboardButton("➖ Удалить категорию",
                            $"product:edit:removecategory:{original.Id}")
                        ]);

                    buttons.Add([
                        new InlineKeyboardButton("✅ Завершить", $"product:edit:finish:{original.Id}")
                    ]);

                    var keyboard = new InlineKeyboardMarkup(buttons);

                    await _botClient.SendTextMessageAsync(
                        chatId,
                        $"Текущие категории: {categoriesList}\n\nВыберите действие:",
                        replyMarkup: keyboard,
                        ct: ct);
                }
            }
            finally
            {
                _userStateService.SetState(userId, state);
            }
        }
    }
}
