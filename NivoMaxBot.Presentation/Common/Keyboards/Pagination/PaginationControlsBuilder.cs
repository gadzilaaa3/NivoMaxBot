using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Shared.Pagination;

namespace NivoMaxBot.Presentation.Common.Keyboards.Pagination
{
    public class PaginationControlsBuilder : IPaginationControlsBuilder
    {
        public List<InlineKeyboardButton[]> CreatePaginationButtons<T>(
            PagedResult<T> pagedResult,
            string callbackDataPattern,
            params object[] routeValues)
        {
            var buttons = new List<InlineKeyboardButton[]>();
            var paginationRow = new List<InlineKeyboardButton>();

            if (pagedResult.PageNumber > 1)
            {
                var prevValues = routeValues.Concat(new object[] { pagedResult.PageNumber - 1 }).ToArray();
                var prevCallback = string.Format(callbackDataPattern, prevValues);
                paginationRow.Add(new InlineKeyboardButton("◀️ Назад", prevCallback));
            }

            if (pagedResult.PageNumber < pagedResult.TotalPages)
            {
                var nextValues = routeValues.Concat(new object[] { pagedResult.PageNumber + 1 }).ToArray();
                var nextCallback = string.Format(callbackDataPattern, nextValues);
                paginationRow.Add(new InlineKeyboardButton("Вперёд ▶️", nextCallback));
            }

            if (paginationRow.Any())
                buttons.Add(paginationRow.ToArray());

            return buttons;
        }
    }
}
