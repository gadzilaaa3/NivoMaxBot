using FluentValidation;

namespace NivoMaxBot.Application.Features.Users.Commands.Register
{
    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {
            RuleFor(x => x.MaxId)
                .NotEmpty().WithMessage("Идентификатор Max обязателен.");
        }
    }
}