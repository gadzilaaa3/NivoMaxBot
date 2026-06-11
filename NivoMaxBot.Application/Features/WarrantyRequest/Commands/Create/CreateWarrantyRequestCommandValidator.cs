using FluentValidation;

namespace NivoMaxBot.Application.Features.WarrantyRequest.Commands.Create
{
    public class CreateWarrantyRequestCommandValidator : AbstractValidator<CreateWarrantyRequestCommand>
    {
        public CreateWarrantyRequestCommandValidator()
        {
            When(x => x.INN != null, () =>
            {
                RuleFor(x => x.INN)
                    .Matches(@"^\d{10,12}$").WithMessage("ИНН должен содержать 10 или 12 цифр");
            });

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("Город обязателен")
                .MaximumLength(100);

            RuleFor(x => x.ContactPhone)
                .NotEmpty().WithMessage("Номер телефона обязателен.")
                .MaximumLength(20).WithMessage("Номер телефона не может быть длиннее 20 символов.")
                .MinimumLength(10).WithMessage("Номер телефона слишком короткий.")
                // Регулярное выражение для допустимых символов: цифры, +, пробелы, скобки, дефисы
                .Matches(@"^[\+\d\s\-\(\)]+$").WithMessage("Номер телефона содержит недопустимые символы. Разрешены только цифры, +, пробелы, скобки и дефисы.")
                // Проверка количества цифр (должно быть ровно 11)
                .Must(BeValidRussianPhoneNumber).WithMessage("Номер телефона должен содержать ровно 11 цифр (например, +7 (988) 888-88-88 или 89888888888).");

            RuleFor(x => x.ContactPerson)
                .NotEmpty().WithMessage("Контактное лицо обязательно")
                .MaximumLength(200);

            RuleFor(x => x.ContactEmail)
                .NotEmpty().EmailAddress().WithMessage("Неверный формат email.")
                .MaximumLength(100).WithMessage("Email не может превышать 100 символов.");

            RuleFor(x => x.ProblemDescription)
                .NotEmpty().WithMessage("Описание проблемы обязательно")
                .MaximumLength(2000);

            RuleFor(x => x.ProductSerialNumber)
                .NotEmpty().WithMessage("Серийный номер обязателен");
        }

        private bool BeValidRussianPhoneNumber(string phoneNumber)
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
