using MediatR;
using NivoMaxBot.Application.Common.Exceptions;
using NivoMaxBot.Domain.Entities;
using NivoMaxBot.Domain.Interfaces.Repositories;

namespace NivoMaxBot.Application.Features.Products.Commands.Update
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, bool>
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public UpdateProductCommandHandler(
            IProductRepository productRepository, 
            ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdWithCategoriesAsync(request.Id, cancellationToken);
            if (product == null)
                throw new NotFoundException(nameof(Product), request.Id);

            if (request.CategoryIds == null || !request.CategoryIds.Any())
                throw new BusinessRuleViolationException("Продукт должен принадлежать хотя бы одной категории.");

            // Проверка уникальности имени (если имя изменилось)
            if (!string.Equals(product.Name, request.Name, StringComparison.OrdinalIgnoreCase))
            {
                foreach (var categoryId in request.CategoryIds)
                {
                    var exists = await _productRepository.ExistsInCategoryAsync(categoryId,
                        request.Name,
                        cancellationToken);

                    if (exists)
                        throw new BusinessRuleViolationException($"Продукт с именем '{request.Name}' уже существует в категории.");
                }
            }

            // Обновление основных полей
            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.WarrantyInMonths = request.WarrantyInMonths;
            product.PhotoMaxFileId = request.PhotoMaxFileId;
            product.IsAvailable = request.IsAvailable;
            product.PhotoUrl = request.PhotoUrl;

            // Обновление категорий
            var currentCategoryIds = product.Categories.Select(c => c.Id).ToList();

            // Категории для добавления (которые есть в запросе, но нет в продукте)
            var categoriesToAddIds = request.CategoryIds.Except(currentCategoryIds).ToList();
            if (categoriesToAddIds.Any())
            {
                var categoriesToAdd = await _categoryRepository.GetByIdsAsync(categoriesToAddIds, cancellationToken);
                foreach (var cat in categoriesToAdd)
                    product.Categories.Add(cat);
            }

            // Категории для удаления (которые есть в продукте, но нет в запросе)
            var categoriesToRemoveIds = currentCategoryIds.Except(request.CategoryIds).ToList();
            var categoriesToRemove = product.Categories.Where(c => categoriesToRemoveIds.Contains(c.Id)).ToList();
            foreach (var cat in categoriesToRemove)
                product.Categories.Remove(cat);

            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
