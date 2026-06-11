using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Shared.Pagination;

namespace NivoMaxBot.Presentation.Common.Keyboards.Pagination
{
    public interface IPaginationControlsBuilder
    {
        List<InlineKeyboardButton[]> CreatePaginationButtons<T>(
            PagedResult<T> pagedResult,
            string callbackDataPattern,
            params object[] routeValues);
    }
}
