using MediatR;
using NivoMaxBot.Application.Features.Products.Dtos;
using NivoMaxBot.Domain.Interfaces.Repositories;

namespace NivoMaxBot.Application.Features.Products.Queries.ById
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
    {
        private readonly IProductRepository _productRepository;

        public GetProductByIdQueryHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdWithCategoriesAsync(request.Id, cancellationToken);
            if (product == null) return null;

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                PhotoMaxFileId = product.PhotoMaxFileId,
                PhotoUrl = product.PhotoUrl,
                IsAvailable = product.IsAvailable,
                WarrantyInMonths = product.WarrantyInMonths,
                CategoryIds = product.Categories.Select(c => c.Id).ToList(),
                CategoryNames = product.Categories.Select(c => c.Name).ToList()
            };
        }
    }
}
