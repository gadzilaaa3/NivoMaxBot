using Microsoft.Extensions.Logging;
using NivoMaxBot.Shared.Brand;
using System.Reflection;
using System.Text.Json;

namespace NivoMaxBot.Presentation.Services.Brand
{
    public class BrandDataService : IBrandDataService
    {
        public BrandData Data { get; }

        private readonly ILogger<BrandDataService> _logger;
        public BrandDataService(ILogger<BrandDataService> logger)
        {
            _logger = logger;

            // Определяем базовую директорию приложения (где лежит .exe)
            // var basePath = AppContext.BaseDirectory;
            var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var filePath = Path.Combine(basePath, "Resources", "brand_data.json");
            if (!File.Exists(filePath))
            {
                var ex = new FileNotFoundException("brand_data.json not found", filePath);
                _logger.LogError(ex, "Brand information configuration file not found");
            }
            var json = File.ReadAllText(filePath);
            Data = JsonSerializer.Deserialize<BrandData>(json) ?? new BrandData();
        }
    }
}
