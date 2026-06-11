using MediatR;
using NivoMaxBot.Application.Common.Exceptions;
using NivoMaxBot.Domain.Entities;
using NivoMaxBot.Domain.Interfaces.Repositories;

namespace NivoMaxBot.Application.Features.Products.Commands.Create
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, int>
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public CreateProductCommandHandler(
            IProductRepository productRepository, 
            ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            // Проверяем существование категории
            var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
            if (category == null)
                throw new NotFoundException(nameof(Category), request.CategoryId);

            // Проверяем уникальность имени в категории
            if (await _productRepository.ExistsInCategoryAsync(request.CategoryId, 
                request.Name, cancellationToken))
                throw new BusinessRuleViolationException($"Продукт с именем " +
                    $"'{request.Name}' уже существует в данной категории.");

            // Создаём товар
            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                WarrantyInMonths = request.WarrantyInMonths,
                PhotoMaxFileId = request.PhotoMaxFileId,
                PhotoUrl = request.PhotoUrl,
                IsAvailable = request.IsAvailable,
                Categories = new List<Category> { category }
            };

            await _productRepository.AddAsync(product, cancellationToken);
            await _productRepository.SaveChangesAsync(cancellationToken);

            return product.Id;
        }
    }
}
