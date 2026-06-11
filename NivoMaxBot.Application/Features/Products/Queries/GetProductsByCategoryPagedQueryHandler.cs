using MediatR;
using NivoMaxBot.Application.Features.Products.Dtos;
using NivoMaxBot.Domain.Interfaces.Repositories;
using NivoMaxBot.Shared.Pagination;

namespace NivoMaxBot.Application.Features.Products.Queries
{
    public class GetProductsByCategoryPagedQueryHandler 
        : IRequestHandler<GetProductsByCategoryPagedQuery, PagedResult<ProductDto>>
    {
        private readonly IProductRepository _productRepository;

        public GetProductsByCategoryPagedQueryHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<PagedResult<ProductDto>> Handle(GetProductsByCategoryPagedQuery request, 
            CancellationToken cancellationToken)
        {
            var pagedProducts = await _productRepository.GetByCategoryIdPagedAsync(
                request.CategoryId,
                request.PagedRequest,
                request.IncludeUnavailable,
                cancellationToken);

            var dtos = pagedProducts.Items.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                PhotoMaxFileId = p.PhotoMaxFileId,
                PhotoUrl = p.PhotoUrl,
                IsAvailable = p.IsAvailable,
                WarrantyInMonths = p.WarrantyInMonths,
                CategoryIds = p.Categories.Select(c => c.Id).ToList(),
                CategoryNames = p.Categories.Select(c => c.Name).ToList()
            }).ToList();

            return new PagedResult<ProductDto>
            {
                Items = dtos,
                TotalCount = pagedProducts.TotalCount,
                PageNumber = pagedProducts.PageNumber,
                PageSize = pagedProducts.PageSize
            };
        }
    }
}