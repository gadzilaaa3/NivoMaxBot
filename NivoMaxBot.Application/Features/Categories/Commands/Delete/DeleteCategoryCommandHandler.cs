using MediatR;
using NivoMaxBot.Application.Common.Exceptions;
using NivoMaxBot.Domain.Entities;
using NivoMaxBot.Domain.Interfaces.Repositories;

namespace NivoMaxBot.Application.Features.Categories.Commands.Delete
{
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, bool>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;

        public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository, IProductRepository productRepository)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
        }

        public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
            if (category == null)
                throw new NotFoundException(nameof(Category), request.Id);

            // Получаем ID всех потомков для проверки наличия товаров
            var descendantIds = await _categoryRepository.GetDescendantIdsAsync(request.Id, cancellationToken);
            var hasProducts = await _categoryRepository.AnyHasProductsAsync(descendantIds, cancellationToken);
            if (hasProducts)
                throw new BusinessRuleViolationException("Невозможно удалить категорию, так как она или её подкатегории содержат товары.");

            // Удаляем только корневую категорию – EF Core каскадно удалит всех потомков
            _categoryRepository.Remove(category);
            await _categoryRepository.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
