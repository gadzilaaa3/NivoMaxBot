using MediatR;
using NivoMaxBot.Application.Features.Categories.Dtos;
using NivoMaxBot.Domain.Interfaces.Repositories;

namespace NivoMaxBot.Application.Features.Categories.Queries.ById
{
    public class GetCategoriesByIdsQueryHandler
        : IRequestHandler<GetCategoriesByIdsQuery, IEnumerable<CategoryDto>>
    {
        private readonly ICategoryRepository _categoryRepository;

        public GetCategoriesByIdsQueryHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<CategoryDto>> Handle(GetCategoriesByIdsQuery request, 
            CancellationToken cancellationToken)
        {
            var categories = await _categoryRepository.GetCategoriesByIdsAsync(request.Ids, cancellationToken);

            return categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Order = c.Order,
                ParentId = c.ParentId,
                ParentName = c.ParentNavigation?.Name,
                HasChildren = c.Children?.Any() ?? false,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            }).ToList();
        }
    }
}
