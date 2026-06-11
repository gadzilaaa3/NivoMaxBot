using NivoMaxBot.Application.Features.Products.Dtos;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Presentation.Common.Keyboards.Admin.Product;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;
using NivoMaxBot.Presentation.Common.Keyboards.Pagination;
using NivoMaxBot.Shared.Pagination;

public class ProductListKeyboardFactory : IProductListKeyboardFactory
{
    private readonly IMenuBuilder _menuBuilder;
    private readonly IPaginationControlsBuilder _paginationControlsBuilder;

    public ProductListKeyboardFactory(IMenuBuilder menuBuilder, 
        IPaginationControlsBuilder paginationControlsBuilder)
    {
        _menuBuilder = menuBuilder;
        _paginationControlsBuilder = paginationControlsBuilder;
    }

    public IInlineKeyboardMarkup CreateProductListKeyboard(
        PagedResult<ProductDto> pagedResult,
        int categoryId,
        int? currentParentId,
        bool hasParent,
        string backCallback,
        MenuType menuType)
    {
        var buttons = new List<InlineKeyboardButton[]>();

        // Кнопки товаров
        foreach (var product in pagedResult.Items)
        {
            var text = $"🛒 {product.Name} — {product.Price} руб.";
            var callback = $"product:view:{product.Id}";
            buttons.Add(new[] { new InlineKeyboardButton(text, callback) });
        }

        // Кнопки пагинации
        var paginationButtons = _paginationControlsBuilder.CreatePaginationButtons(
            pagedResult,
            "products:page:{0}:{1}", // categoryId, pageNumber
            categoryId);
        buttons.AddRange(paginationButtons);

        // Кнопка добавления товара
        buttons.Add(new[] { new InlineKeyboardButton("➕ Добавить товар", $"product:add:{categoryId}") });

        buttons.Add(new[] { new InlineKeyboardButton("📦 Загрузить из JSON", $"product:bulkadd:{categoryId}") });

        // Кнопка "Подкатегории" (если есть подкатегории)
        if (currentParentId.HasValue)
        {
            buttons.Add(new[] { new InlineKeyboardButton("📂 Подкатегории", $"products:subcategories:{currentParentId}") });
        }

        // Добавляем управляющие кнопки через MenuBuilder
        return _menuBuilder.AddControlButtons(buttons, backCallback, menuType);
    }
}