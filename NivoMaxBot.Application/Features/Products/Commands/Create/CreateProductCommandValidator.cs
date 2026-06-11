using FluentValidation;

namespace NivoMaxBot.Application.Features.Products.Commands.Create
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Название товара обязательно")
                .MaximumLength(200).WithMessage("Название не может превышать 200 символов");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Описание не может превышать 1000 символов");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Цена должна быть положительной");

            RuleFor(x => x.WarrantyInMonths)
                .GreaterThanOrEqualTo(0).WithMessage("Гарантия должна быть неотрицательной");

            RuleFor(x => x.PhotoMaxFileId)
                .MaximumLength(200).WithMessage("ID файла слишком длинный");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Категория обязательна");
        }
    }
}
