using System.Text.RegularExpressions;

namespace NivoMaxBot.Shared.Services
{
    public static class PhoneValidator
    {
        public static bool Validate(string phone)
        {
            var isMatch = Regex.IsMatch(phone, @"^[\+\d\s\-\(\)]+$");
            if (isMatch)
            {
                return BeValidRussianPhoneNumber(phone);
            }
            else
            {
                return false;
            }
        }

        private static bool BeValidRussianPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            // Удаляем все нецифровые символы
            var digitsOnly = new string(phoneNumber.Where(char.IsDigit).ToArray());

            // Российский номер должен содержать 11 цифр (например, 8XXX... или +7XXX...)
            return digitsOnly.Length == 11;
        }
    }
}
