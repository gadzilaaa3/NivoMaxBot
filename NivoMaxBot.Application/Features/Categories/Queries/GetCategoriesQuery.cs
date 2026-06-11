using MediatR;
using NivoMaxBot.Application.Features.Categories.Dtos;

namespace NivoMaxBot.Application.Features.Categories.Queries
{
    public class GetCategoriesQuery : IRequest<IEnumerable<CategoryDto>>
    {
        public int? ParentId { get; set; }
    }
}
