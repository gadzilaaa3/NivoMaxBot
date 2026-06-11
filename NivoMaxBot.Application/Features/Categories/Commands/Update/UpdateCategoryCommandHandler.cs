using MediatR;
using NivoMaxBot.Application.Common.Exceptions;
using NivoMaxBot.Domain.Interfaces.Repositories;

namespace NivoMaxBot.Application.Features.Categories.Commands.Update
{
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, bool>
    {
        private readonly ICategoryRepository _categoryRepository;

        public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository, IAdminRepository adminRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<bool> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
            if (category == null) return false;

            // Проверка на цикличность: если ParentId задан, убедиться, что новый родитель не является потомком текущей категории
            if (request.ParentId.HasValue)
            {
                if (request.ParentId.Value == request.Id)
                    throw new BusinessRuleViolationException("Категория не может быть родителем самой себя.");

                var isChild = await _categoryRepository.IsAncestorOfAsync(request.Id, request.ParentId.Value, cancellationToken);
                if (isChild)
                    throw new BusinessRuleViolationException("Нельзя сделать родителем своего потомка.");
            }

            category.Name = request.Name;
            category.ParentId = request.ParentId;
            category.Order = request.Order;

            _categoryRepository.Update(category);
            await _categoryRepository.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
