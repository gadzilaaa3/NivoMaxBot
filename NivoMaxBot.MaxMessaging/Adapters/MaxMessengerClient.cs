using Max.Bot;
using Max.Bot.Types;
using Max.Bot.Types.Enums;
using Max.Bot.Types.Requests;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace NivoMaxBot.MaxMessaging.Adapters
{
    public class MaxMessengerClient : IMessengerClient
    {
        private readonly MaxClient _maxClient;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _botToken;
        private readonly string _maxApiUrl;

        public MaxMessengerClient(
            MaxClient maxClient, 
            IHttpClientFactory httpClientFactory,
            string botToken,
            string maxApiUrl)
        {
            _maxClient = maxClient;
            _httpClientFactory = httpClientFactory;
            _botToken = botToken;
            _maxApiUrl = maxApiUrl;
        }

        public async Task<IMessage> SendTextMessageAsync(long chatId, string text,
            IInlineKeyboardMarkup? replyMarkup = null,
            Messaging.Abstractions.Types.Enums.TextFormat? textFormat = null, CancellationToken ct = default)
        {
            var format = ToMaxTextFormat(textFormat);

            var keyboard = ToMaxInlineKeyboard(replyMarkup);
            var message = await _maxClient.Messages.SendMessageAsync(chatId, text, keyboard, 
                format: format, cancellationToken: ct);
                
            return new MaxMessageAdapter(message);
        }

        public async Task<IMessage> SendPhotoAsync(long chatId, InputFileStream photo, 
            string? caption = null, IInlineKeyboardMarkup? replyMarkup = null,
            Messaging.Abstractions.Types.Enums.TextFormat? textFormat = null, CancellationToken ct = default)
        {
            using (photo)
            {
                var uploadResponse = await _maxClient.Files.UploadFileAsync(UploadType.Image, ct).ConfigureAwait(false);
                var payload = await _maxClient.Files.UploadFileDataAsync(
                    uploadResponse.Url,
                    photo.Stream,
                    photo.FileName,
                    ct).ConfigureAwait(false);

                var imageAttachment = new AttachmentRequest
                {
                    Type = "image",
                    Payload = payload
                };

                List<AttachmentRequest> attachments = [];
                attachments.Add(imageAttachment);
                if (replyMarkup != null)
                {
                    attachments.Add(new AttachmentRequest
                    {
                        Type = "inline_keyboard",
                        Payload = ToMaxInlineKeyboardPayload(replyMarkup)!
                    });
                }

                var format = ToMaxTextFormat(textFormat);

                var message = await _maxClient.Messages.SendMessageWithAttachmentsAsync(attachments, chatId,
                    null, caption, format: format, cancellationToken: ct);

                return new MaxMessageAdapter(message);
            }
        }

        public async Task<IMessage> SendPhotoByFileIdAsync(long chatId, string fileId, 
            string? caption = null, IInlineKeyboardMarkup? replyMarkup = null, 
            Messaging.Abstractions.Types.Enums.TextFormat? textFormat = null, CancellationToken ct = default)
        {
            var imageAttachment = new AttachmentRequest
            {
                Type = "image",
                Payload = new
                {
                    token = fileId,
                }
            };

            List<AttachmentRequest> attachments = [];
            attachments.Add(imageAttachment);
            if (replyMarkup != null)
            {
                attachments.Add(new AttachmentRequest
                {
                    Type = "inline_keyboard",
                    Payload = ToMaxInlineKeyboardPayload(replyMarkup)!
                });
            }

            var format = ToMaxTextFormat(textFormat);

            var message = await _maxClient.Messages.SendMessageWithAttachmentsAsync(attachments, chatId,
                null, caption, format: format, cancellationToken: ct);

            return new MaxMessageAdapter(message);
        }

        public async Task<IMessage> SendPhotoByUrlAsync(long chatId, string url, 
            string? caption = null, IInlineKeyboardMarkup? replyMarkup = null, 
            Messaging.Abstractions.Types.Enums.TextFormat? textFormat = null, CancellationToken ct = default)
        {
            var imageAttachment = new AttachmentRequest
            {
                Type = "image",
                Payload = new
                {
                    url = url,
                }
            };

            List<AttachmentRequest> attachments = [];
            attachments.Add(imageAttachment);
            if (replyMarkup != null)
            {
                attachments.Add(new AttachmentRequest
                {
                    Type = "inline_keyboard",
                    Payload = ToMaxInlineKeyboardPayload(replyMarkup)!
                });
            }

            var format = ToMaxTextFormat(textFormat);

            var message = await _maxClient.Messages.SendMessageWithAttachmentsAsync(attachments, chatId,
                null, caption, format: format, cancellationToken: ct);

            return new MaxMessageAdapter(message);
        }

        public async Task<IMessage> SendVideoAsync(long chatId, InputFileStream video, 
            string? caption = null, IInlineKeyboardMarkup? replyMarkup = null, 
            Messaging.Abstractions.Types.Enums.TextFormat? textFormat = null, CancellationToken ct = default)
        {
            using (video)
            {
                var uploadResponse = await _maxClient.Files.UploadFileAsync(UploadType.Video, ct).ConfigureAwait(false);
                var payload = await _maxClient.Files.UploadFileDataAsync(
                    uploadResponse.Url,
                    video.Stream,
                    video.FileName,
                    ct).ConfigureAwait(false);

                var videoAttachment = new AttachmentRequest
                {
                    Type = "video",
                    Payload = new
                    {
                        token = uploadResponse.Token
                    }
                };

                List<AttachmentRequest> attachments = [];
                attachments.Add(videoAttachment);
                if (replyMarkup != null)
                {
                    attachments.Add(new AttachmentRequest
                    {
                        Type = "inline_keyboard",
                        Payload = ToMaxInlineKeyboardPayload(replyMarkup)!
                    });
                }

                var format = ToMaxTextFormat(textFormat);

                var message = await _maxClient.Messages.SendMessageWithAttachmentsAsync(attachments, chatId,
                    null, caption, format: format, cancellationToken: ct);

                return new MaxMessageAdapter(message);
            }
        }

        //[Obsolete("Неккоректная работа метода")]
        //public async Task AnswerCallbackQueryAsync(string callbackQueryId, string? text = null, CancellationToken ct = default)
        //{
        //    await _maxClient.Messages.AnswerCallbackQueryAsync(callbackQueryId, new AnswerCallbackQueryRequest
        //    {
        //        Notification = text
        //    }, ct);
        //}

        public async Task AnswerCallbackQueryAsync(string callbackQueryId, string text = "✅", CancellationToken ct = default)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var encodedId = Uri.EscapeDataString(callbackQueryId);
            var requestUrl = $"{_maxApiUrl.TrimEnd('/')}/answers?callback_id={encodedId}";

            using var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);

            var payload = new { notification = text };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            request.Content = content;
            
            request.Headers.Add("Authorization", $"{_botToken}");
            request.Headers.Add("Accept", "application/json");

            var response = await httpClient.SendAsync(request, ct);
        }

        public async Task EditMessageTextAsync(long chatId, string messageId, string text, IInlineKeyboardMarkup? replyMarkup = null,
            Messaging.Abstractions.Types.Enums.TextFormat? textFormat = null, CancellationToken ct = default)
        {
            var format = ToMaxTextFormat(textFormat);
            var attachments = replyMarkup != null ? new AttachmentRequest[] 
            {
                new AttachmentRequest
                {
                    Type = "inline_keyboard",
                    Payload = ToMaxInlineKeyboardPayload(replyMarkup)!
                }
            } : [];


            await _maxClient.Messages.EditMessageAsync(messageId, new EditMessageRequest
            {
                Format = format,
                Text = text,
                Attachments = attachments,
            }, ct);
        }

        public async Task<IMessage> CopyMessageAsync(long fromChatId, string messageId, long? chatId = null,
            long? userId = null, CancellationToken ct = default)
        {
            var message = await _maxClient.Messages.GetMessageAsync(messageId, ct);
            if (message == null)
                throw new InvalidOperationException("Сообщение не найдено");

            var attachments = message.Body?.Attachments ?? [];
            var attachmentRequests = new List<AttachmentRequest>();

            foreach (var att in attachments)
            {
                var json = JsonSerializer.Serialize(att);
                var jsonNode = JsonSerializer.Deserialize<JsonObject>(json);
                jsonNode.Remove("type"); // убираем Type, чтобы не дублировать
                attachmentRequests.Add(new AttachmentRequest
                {
                    Type = att.Type,
                    Payload = jsonNode
                });
            }

            var newMsg = await _maxClient.Messages.SendMessageWithAttachmentsAsync(
                attachmentRequests, chatId, userId, message?.Text, cancellationToken: ct
            );
            return new MaxMessageAdapter(newMsg);
        }

        public async Task SendOrEditMessageAsync(long chatId, IMessage? message, 
            string text, IInlineKeyboardMarkup? replyMarkup = null, 
            Messaging.Abstractions.Types.Enums.TextFormat? textFormat = null, CancellationToken ct = default)
        {
            if (message?.Text != null && message?.MessageId != null)
            {
                await EditMessageTextAsync(chatId, message.MessageId, text, replyMarkup, textFormat, ct);
            }
            else
            {
                await SendTextMessageAsync(chatId, text, replyMarkup, textFormat, ct);
            }
        }

        private TextFormat? ToMaxTextFormat(Messaging.Abstractions.Types.Enums.TextFormat? textFormat)
        {
            return textFormat switch
            {
                Messaging.Abstractions.Types.Enums.TextFormat.Html => TextFormat.Html,
                Messaging.Abstractions.Types.Enums.TextFormat.Markdown => TextFormat.Markdown,
                _ => null
            };
        }

        private InlineKeyboard? ToMaxInlineKeyboard(IInlineKeyboardMarkup? markup)
        {
            var buttons = markup?.Buttons.Select(row => row.Select(btn => new Max.Bot.Types.InlineKeyboardButton
            {
                CallbackData = btn.CallbackData,
                Text = btn.Text,
                Url = btn.Url,
            }).ToArray()).ToArray();

            return buttons != null ? new InlineKeyboard(buttons) : null;
        }

        private object? ToMaxInlineKeyboardPayload(IInlineKeyboardMarkup? markup)
        {
            if (markup == null) return null;

            var buttons = markup.Buttons
                .Select(row => row
                    .Select<IInlineKeyboardButton, object>(btn =>
                    {
                        if (btn.CallbackData != null)
                            return new { type = "callback", text = btn.Text, payload = btn.CallbackData };
                        if (btn.Url != null)
                            return new { type = "link", text = btn.Text, url = btn.Url };
                        // если нет ни callback, ни url (просто текстовая кнопка)
                        return new { type = "message", text = btn.Text };
                    })
                    .ToArray()
                ).ToArray();

            return new { buttons };
        }

        public async Task<IMessage> SendTextMessageToUserAsync(long userId, string text, 
            IInlineKeyboardMarkup? replyMarkup = null, 
            Messaging.Abstractions.Types.Enums.TextFormat? textFormat = null, 
            CancellationToken ct = default)
        {
            var format = ToMaxTextFormat(textFormat);
            var keyboard = ToMaxInlineKeyboard(replyMarkup);

            var message = await _maxClient.Messages.SendMessageToUserAsync(userId, text,
                keyboard, format: format, cancellationToken: ct);

            return new MaxMessageAdapter(message);
        }
    }
}
