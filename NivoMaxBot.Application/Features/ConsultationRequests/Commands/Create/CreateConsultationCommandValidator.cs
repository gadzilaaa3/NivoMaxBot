using FluentValidation;

namespace NivoMaxBot.Application.Features.ConsultationRequests.Commands.Create
{
    public class CreateConsultationCommandValidator : AbstractValidator<CreateConsultationCommand>
    {
        public CreateConsultationCommandValidator()
        {
            RuleFor(x => x.UserMaxId)
                .NotEmpty().WithMessage("UserId обязателен.");

            RuleFor(x => x.ContactName)
                .NotEmpty().WithMessage("Имя обязательно.")
                .MaximumLength(200).WithMessage("Имя не может превышать 200 символов.");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("Город обязателен.")
                .MaximumLength(200).WithMessage("Название города не может превышать 200 символов.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Номер телефона обязателен.")
                .MaximumLength(20).WithMessage("Номер телефона не может быть длиннее 20 символов.")
                .MinimumLength(10).WithMessage("Номер телефона слишком короткий.")
                .Matches(@"^[\+\d\s\-\(\)]+$").WithMessage("Номер телефона содержит недопустимые символы.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Описание обязательно.")
                .MaximumLength(5000).WithMessage("Описание не может превышать 5000 символов.");
        }
    }
}
