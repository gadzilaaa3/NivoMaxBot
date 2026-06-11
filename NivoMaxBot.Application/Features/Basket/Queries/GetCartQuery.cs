using MediatR;
using NivoMaxBot.Application.Features.Basket.Dtos;

namespace NivoMaxBot.Application.Features.Basket.Queries
{
    public class GetCartQuery : IRequest<IEnumerable<CartItemDto>>
    {
        public int UserId { get; set; }
    }
}
