using Microsoft.Extensions.Logging;
using NivoMaxBot.Application.Interfaces;
using System.Reflection;

namespace NivoMaxBot.Infrastructure.Services
{
    public class MenuPhotoService : IMenuPhotoService
    {
        private readonly string _photosFolder;
        private readonly ILogger<MenuPhotoService> _logger;

        public MenuPhotoService(ILogger<MenuPhotoService> logger)
        {
            _logger = logger;
            // Определяем папку с фото: в папке приложения (где exe) + "MenuPhotos"
            var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _photosFolder = Path.Combine(basePath, "MenuPhotos");
        }

        public string? GetPhotoPath(string sectionKey)
        {
            // Ищем файл с именем sectionKey (например, main_menu.jpg, main_menu.png)
            var supportedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            foreach (var ext in supportedExtensions)
            {
                var filePath = Path.Combine(_photosFolder, sectionKey + ext);
                if (File.Exists(filePath))
                    return filePath;
            }
            return null;
        }
    }
}
