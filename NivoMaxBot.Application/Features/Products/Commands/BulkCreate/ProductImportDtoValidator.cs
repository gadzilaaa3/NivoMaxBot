using FluentValidation;
using NivoMaxBot.Application.Features.Products.Dtos;

namespace NivoMaxBot.Application.Features.Products.Commands.BulkCreate
{
    public class ProductImportDtoValidator : AbstractValidator<ProductImportDto>
    {
        public ProductImportDtoValidator()
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
        }
    }
}
