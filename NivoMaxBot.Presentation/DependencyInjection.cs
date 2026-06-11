using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Handlers.Interfaces;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Common.Keyboards.Admin.Category;
using NivoMaxBot.Presentation.Common.Keyboards.Admin.Category.ParentSelection;
using NivoMaxBot.Presentation.Common.Keyboards.Admin.Product;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;
using NivoMaxBot.Presentation.Common.Keyboards.Pagination;
using NivoMaxBot.Presentation.Common.Keyboards.User.Catalog;
using NivoMaxBot.Presentation.Services.Brand;
using NivoMaxBot.Presentation.Services.MenuDisplay;
using NivoMaxBot.Presentation.Services.Notifications;
using NivoMaxBot.Presentation.Services.User.Cart;
using NivoMaxBot.Presentation.Services.User.Order;
using NivoMaxBot.Presentation.Services.User.Profile;
using NivoMaxBot.Presentation.Services.User.WarrantyRequest;

namespace NivoMaxBot.Presentation
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentationServices(this IServiceCollection services)
        {
            #region Handlers
            services.Scan(scan => scan
                .FromAssemblyOf<Program>()
                .AddClasses(classes => classes.WithAttribute<CallbackRouteAttribute>())
                .AsSelf()
                .WithScopedLifetime());

            services.Scan(scan => scan
                .FromAssemblyOf<Program>()
                .AddClasses(classes => classes.AssignableTo<IMessageHandler>())
                .As<IMessageHandler>()
                .WithScopedLifetime());
            #endregion Handlers

            services.AddScoped<IMenuBuilder, MenuBuilder>();
            services.AddScoped<IPaginationControlsBuilder, PaginationControlsBuilder>();

            #region Keyboards
            services.AddScoped<IAdminCategoryKeyboardFactory, AdminCategoryKeyboardFactory>();
            services.AddScoped<ICategoryParentSelectionKeyboardBuilder, CategoryParentSelectionKeyboardBuilder>();

            services.AddScoped<IProductCategoryChoiceKeyboardFactory, ProductCategoryChoiceKeyboardFactory>();
            services.AddScoped<IProductListKeyboardFactory, ProductListKeyboardFactory>();
            services.AddScoped<IProductViewKeyboardFactory, ProductViewKeyboardFactory>();

            services.AddScoped<IUserCategoryKeyboardFactory, UserCategoryKeyboardFactory>();
            services.AddScoped<IUserProductListKeyboardFactory, UserProductListKeyboardFactory>();
            services.AddScoped<IUserProductViewKeyboardFactory, UserProductViewKeyboardFactory>();
            #endregion Keyboards

            #region Display
            services.AddScoped<IMenuDisplayService, MenuDisplayService>();

            services.AddScoped<IProfileViewService, ProfileViewService>();
            services.AddScoped<ICartViewService, CartViewService>();
            services.AddScoped<IOrdersViewService, OrdersViewService>();
            services.AddScoped<IWarrantyRequestsViewService, WarrantyRequestsViewService>();
            #endregion Display

            services.AddScoped<INotificationService, MessengerNotificationService>();

            services.AddSingleton<IBrandDataService, BrandDataService>();

            return services;
        }
    }
}
