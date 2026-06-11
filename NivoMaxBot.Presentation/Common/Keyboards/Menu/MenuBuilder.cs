using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Presentation.Handlers;
using NivoMaxBot.Presentation.Handlers.Callbacks.Admins;
using NivoMaxBot.Presentation.Handlers.Callbacks.Broadcast;
using NivoMaxBot.Presentation.Handlers.Callbacks.Categories.Admin;
using NivoMaxBot.Presentation.Handlers.Callbacks.ConsultationRequests;
using NivoMaxBot.Presentation.Handlers.Callbacks.Orders.Admin;
using NivoMaxBot.Presentation.Handlers.Callbacks.User;
using NivoMaxBot.Presentation.Handlers.Callbacks.User.Brand;
using NivoMaxBot.Presentation.Handlers.Callbacks.User.Catalog;
using NivoMaxBot.Presentation.Handlers.Callbacks.WarrantyRequests.Admin;

namespace NivoMaxBot.Presentation.Common.Keyboards.Menu
{
    public class MenuBuilder : IMenuBuilder
    {
        public IInlineKeyboardMarkup AddControlButtons(
            IEnumerable<IEnumerable<IInlineKeyboardButton>> existingButtons,
            string? backCallback,
            MenuType menuType)
        {
            var buttons = existingButtons.ToList();

            // Кнопка "Назад" (если передан callback)
            if (!string.IsNullOrEmpty(backCallback))
            {
                buttons.Add([
                    new InlineKeyboardButton("🔙 Назад", backCallback)
                ]);
            }

            // Кнопка "Меню" в зависимости от типа
            var menuCallback = menuType == MenuType.Admin ? MenuRoutes.AdminMode 
                : MenuRoutes.UserMode;
            buttons.Add([new InlineKeyboardButton("🏠 Меню", menuCallback)]);

            return new InlineKeyboardMarkup(buttons);
        }

        public IInlineKeyboardMarkup CreateStartAdminMenu()
        {
            var menu = new InlineKeyboardMarkup(
            [
                [new InlineKeyboardButton("👤 Пользовательский режим", MenuRoutes.UserMode)],
                [new InlineKeyboardButton("🛠 Режим администратора", MenuRoutes.AdminMode)],
            ]);

            return menu;
        }

        public IInlineKeyboardMarkup CreateMenu(MenuType menuType)
        {
            return menuType switch
            {
                MenuType.Admin => CreateAdminMenu(),
                MenuType.User => CreateUserMenu(),
                _ => CreateUserMenu()
            };
        }

        private IInlineKeyboardMarkup CreateUserMenu()
        {
            var keyboard = new InlineKeyboardMarkup(
            [
                [
                    new InlineKeyboardButton("🛍 Каталог", UserCatalogRoutes.CatalogRoot),
                    new InlineKeyboardButton("🛒 Корзина", "user:cart:view")
                ],
                [
                    new InlineKeyboardButton("⚙️ Сервис и гарантия", UserModeRoutes.ServiceSection),
                    new InlineKeyboardButton("🏢 О бренде", UserBrandRoutes.Menu)
                ],
                [new InlineKeyboardButton("👤 Профиль", "profile:main")],
            ]);

            return keyboard;
        }

        private IInlineKeyboardMarkup CreateAdminMenu()
        {
            var keyboard = new InlineKeyboardMarkup(
            [
                [
                    new InlineKeyboardButton("📁 Категории", AdminCategoryRoutes.List),
                    new InlineKeyboardButton("📦 Товары", "admin:products"),
                ],
                [
                    new InlineKeyboardButton("📦 Заказы", AdminOrdersRoutes.List),
                    new InlineKeyboardButton("🔧 Заявки на ремонт", AdminWarrantyRequestsRoutes.List),
                ],
                [new InlineKeyboardButton("📞 Заявки на консультацию", ConsultationRequestRoutes.ConsultationList)],
                [new InlineKeyboardButton("📢 Рассылка", AdminBroadcastRoutes.TypeSelection)],
                [new InlineKeyboardButton("👥 Администраторы", AdminsRoutes.List)],
                [new InlineKeyboardButton("🔙 Пользовательский режим", MenuRoutes.UserMode)],
            ]);

            return keyboard;
        }
    }
}
