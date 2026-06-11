using System.Runtime.InteropServices;

namespace NivoMaxBot.Shared.Helpers
{
    public static class MoscowTimeHelper
    {
        private static readonly TimeZoneInfo _moscowTimeZone;

        static MoscowTimeHelper()
        {
            // Кросс-платформенное определение часового пояса Москвы
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                _moscowTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
            }
            else // Linux, macOS
            {
                _moscowTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Moscow");
            }
        }

        /// <summary>
        /// Преобразует UTC время в Московское (не изменяет исходный объект).
        /// </summary>
        public static DateTime ToMoscowTime(DateTime utcDateTime)
        {
            if (utcDateTime.Kind != DateTimeKind.Utc)
            {
                // Если время не UTC, предположим, что оно UTC (можно также принудительно задать Kind)
                utcDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
            }
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, _moscowTimeZone);
        }

        /// <summary>
        /// Возвращает строковое представление Московского времени с пометкой "МСК".
        /// </summary>
        /// <param name="utcDateTime">UTC дата/время из БД</param>
        /// <param name="format">Формат (по умолчанию "dd.MM.yyyy HH:mm:ss")</param>
        public static string ToMoscowTimeString(DateTime utcDateTime, string format = "dd.MM.yyyy HH:mm:ss")
        {
            var moscowTime = ToMoscowTime(utcDateTime);
            return $"{moscowTime.ToString(format)} (МСК)";
        }
    }
}
