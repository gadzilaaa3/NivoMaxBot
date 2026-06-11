using FluentValidation;
using NivoMaxBot.Domain.Interfaces.Repositories;

namespace NivoMaxBot.Application.Features.Categories.Commands.Update
{
    public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
    {
        private readonly ICategoryRepository _categoryRepository;

        public UpdateCategoryCommandValidator(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;

            RuleFor(x => x.Id)
                .MustAsync(async (id, cancellationToken) => await _categoryRepository.ExistsAsync(id, cancellationToken))
                .WithMessage("Категория не существует");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Название категории обязательно")
                .MaximumLength(100).WithMessage("Название не может превышать 100 символов");

            RuleFor(x => x.Order)
                .GreaterThanOrEqualTo(0);

            When(x => x.ParentId.HasValue, () =>
            {
                RuleFor(x => x.ParentId)
                    .MustAsync(async (parentId, cancellationToken) =>
                    {
                        return await _categoryRepository.ExistsAsync(parentId!.Value, cancellationToken);
                    })
                    .WithMessage("Указанная родительская категория не существует");
            });

            // Проверка уникальности имени в пределах одного родителя, исключая текущую категорию
            RuleFor(x => x.Name)
                .MustAsync(async (command, name, cancellationToken) =>
                {
                    var existing = await _categoryRepository.GetAllAsync(cancellationToken);
                    return !existing.Any(c => c.Id != command.Id && c.ParentId == command.ParentId && c.Name == name);
                })
                .WithMessage("Категория с таким именем уже существует в указанной родительской категории");
        }
    }
}
