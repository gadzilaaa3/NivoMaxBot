using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Application.Services;
using NivoMaxBot.Domain.Interfaces.Repositories;
using NivoMaxBot.Infrastructure.Data;
using NivoMaxBot.Infrastructure.Repositories;
using NivoMaxBot.Infrastructure.Services;
using NivoMaxBot.Infrastructure.Services.Initializers;
using NivoMaxBot.Infrastructure.Settings.Admin;

namespace NivoMaxBot.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AdminSettings>(configuration.GetSection(AdminSettings.SectionName));
            services.AddHostedService<AdminInitializer>();

            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IAdminRepository, AdminRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IBasketRepository, BasketRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IWarrantyRequestRepository, WarrantyRequestRepository>();
            services.AddScoped<IConsultationRequestRepository, ConsultationRequestRepository>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IMenuPhotoService, MenuPhotoService>();

            services.AddSingleton<IUserStateService, UserStateService>(); // синглтон для хранения состояний в памяти

            var connectionString = configuration.GetConnectionString("DbDefaultConnection");
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));

            return services;
        }
    }
}
