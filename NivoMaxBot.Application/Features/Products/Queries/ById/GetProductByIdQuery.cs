using MediatR;
using NivoMaxBot.Application.Features.Products.Dtos;

namespace NivoMaxBot.Application.Features.Products.Queries.ById
{
    public class GetProductByIdQuery : IRequest<ProductDto?>
    {
        public int Id { get; set; }
    }
}
