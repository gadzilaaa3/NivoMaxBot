using MediatR;
using NivoMaxBot.Application.Features.Categories.Dtos;
using NivoMaxBot.Domain.Interfaces.Repositories;

namespace NivoMaxBot.Application.Features.Categories.Queries
{
    public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, IEnumerable<CategoryDto>>
    {
        private readonly ICategoryRepository _categoryRepository;

        public GetCategoriesQueryHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = await _categoryRepository
                .GetCategoriesWithChildrenAsync(request.ParentId, cancellationToken);

            return categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Order = c.Order,
                ParentId = c.ParentId,
                HasChildren = c.Children.Count != 0,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
            }).ToList();
        }
    }
}
