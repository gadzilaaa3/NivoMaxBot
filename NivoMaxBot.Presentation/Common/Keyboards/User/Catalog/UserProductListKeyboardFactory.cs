using NivoMaxBot.Application.Features.Products.Dtos;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;
using NivoMaxBot.Presentation.Common.Keyboards.Pagination;
using NivoMaxBot.Shared.Pagination;

namespace NivoMaxBot.Presentation.Common.Keyboards.User.Catalog
{
    public class UserProductListKeyboardFactory : IUserProductListKeyboardFactory
    {
        private readonly IMenuBuilder _menuBuilder;
        private readonly IPaginationControlsBuilder _paginationControlsBuilder;

        public UserProductListKeyboardFactory(
            IMenuBuilder menuBuilder,
            IPaginationControlsBuilder paginationControlsBuilder)
        {
            _menuBuilder = menuBuilder;
            _paginationControlsBuilder = paginationControlsBuilder;
        }

        public IInlineKeyboardMarkup CreateProductListKeyboard(
            PagedResult<ProductDto> pagedResult,
            int categoryId,
            string? backCallback)
        {
            var buttons = pagedResult.Items.Select(p =>
                new[] { new InlineKeyboardButton($"🛒 {p.Name} — {p.Price} руб.", $"user:product:view:{p.Id}") }
            ).ToList();

            var paginationButtons = _paginationControlsBuilder.CreatePaginationButtons(
                pagedResult,
                "user:products:page:{0}:{1}",
                categoryId);

            buttons.AddRange(paginationButtons);

            return _menuBuilder.AddControlButtons(buttons, backCallback, MenuType.User);
        }
    }
}
