using MediatR;
using NivoMaxBot.Application.Features.Categories.Dtos;

namespace NivoMaxBot.Application.Features.Categories.Queries.ById
{
    public class GetCategoriesByIdsQuery : IRequest<IEnumerable<CategoryDto>>
    {
        public IEnumerable<int> Ids { get; set; } = [];
    }
}
