using MediatR;
using NivoMaxBot.Application.Features.Categories.Dtos;
using NivoMaxBot.Domain.Interfaces.Repositories;

namespace NivoMaxBot.Application.Features.Categories.Queries.ById
{
    public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto?>
    {
        private readonly ICategoryRepository _categoryRepository;

        public GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<CategoryDto?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetByIdWithParentAsync(request.Id, cancellationToken);
            if (category == null) return null;

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Order = category.Order,
                ParentId = category.ParentId,
                ParentName = category.ParentNavigation?.Name,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt,
                HasChildren = await _categoryRepository.HasChildrenAsync(request.Id, cancellationToken),
            };
        }
    }
}
