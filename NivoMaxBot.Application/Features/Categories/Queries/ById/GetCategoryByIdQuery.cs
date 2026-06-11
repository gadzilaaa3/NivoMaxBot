using MediatR;
using NivoMaxBot.Application.Features.Categories.Dtos;

namespace NivoMaxBot.Application.Features.Categories.Queries.ById
{
    public class GetCategoryByIdQuery : IRequest<CategoryDto?>
    {
        public int Id { get; set; }
    }
}
