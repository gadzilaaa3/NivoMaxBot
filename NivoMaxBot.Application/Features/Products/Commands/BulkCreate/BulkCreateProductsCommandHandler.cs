using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using NivoMaxBot.Application.Features.Products.Dtos;
using NivoMaxBot.Domain.Entities;
using NivoMaxBot.Domain.Interfaces.Repositories;

namespace NivoMaxBot.Application.Features.Products.Commands.BulkCreate
{
    public class BulkCreateProductsCommandHandler : IRequestHandler<BulkCreateProductsCommand, BulkCreateProductsResult>
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IValidator<ProductImportDto> _productValidator;
        private readonly ILogger<BulkCreateProductsCommandHandler> _logger;

        public BulkCreateProductsCommandHandler(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IValidator<ProductImportDto> productValidator,
            ILogger<BulkCreateProductsCommandHandler> logger)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _productValidator = productValidator;
            _logger = logger;
        }

        public async Task<BulkCreateProductsResult> Handle(BulkCreateProductsCommand request, CancellationToken cancellationToken)
        {
            var result = new BulkCreateProductsResult();

            var errors = new List<string>();
            // Проверяем существование категории
            var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
            if (category == null)
            {
                errors.Add($"Категория с ID {request.CategoryId} не найдена.");

                result.Errors = [.. errors];
                return result;
            }

            // Получаем все существующие имена продуктов в этой категории
            var existingProductNames = await _productRepository.GetProductNamesInCategoryAsync(request.CategoryId, cancellationToken);
            var existingNamesSet = new HashSet<string>(existingProductNames, StringComparer.OrdinalIgnoreCase);

            // Множество для отслеживания дубликатов внутри импорта
            var importNamesSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var productsToAdd = new List<Product>();

            int productIndex = 0;
            foreach (var importDto in request.Products)
            {
                productIndex++;
                // Валидация каждого товара (не зависит от уникальности)
                var validationResult = await _productValidator.ValidateAsync(importDto, cancellationToken);
                if (!validationResult.IsValid)
                {
                    var validationErrors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));

                    var prodName = string.IsNullOrWhiteSpace(importDto.Name) ? $"№{productIndex}" : importDto.Name;
                    errors.Add($"Ошибка в товаре '{prodName}'" +
                        $": {validationErrors}");
                    continue;
                }

                // Проверка уникальности имени
                if (existingNamesSet.Contains(importDto.Name))
                {
                    errors.Add($"Товар с именем '{importDto.Name}' уже существует в категории.");
                    continue;
                }

                if (importNamesSet.Contains(importDto.Name))
                {
                    errors.Add($"Обнаружен дубликат имени '{importDto.Name}' в загружаемом JSON (повторяется несколько раз).");
                    continue;
                }

                importNamesSet.Add(importDto.Name);

                // Создаём товар
                var product = new Product
                {
                    Name = importDto.Name,
                    Description = importDto.Description,
                    Price = importDto.Price,
                    WarrantyInMonths = importDto.WarrantyInMonths,
                    IsAvailable = importDto.IsAvailable ?? true,
                    Categories = new List<Category> { category },
                    PhotoUrl = importDto.PhotoUrl,
                };
                productsToAdd.Add(product);
                result.SuccessCount++;
            }

            if (productsToAdd.Any())
            {
                foreach (var product in productsToAdd)
                {
                    await _productRepository.AddAsync(product, cancellationToken);
                }
                await _productRepository.SaveChangesAsync(cancellationToken);
            }

            result.Errors = [.. errors];
            return result;
        }
    }
}
