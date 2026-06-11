namespace NivoMaxBot.Application.Interfaces
{
    public interface IMenuPhotoService
    {
        /// <summary>
        /// Возвращает путь к файлу фото для указанного раздела, если файл существует.
        /// </summary>
        string? GetPhotoPath(string sectionKey);
    }
}
