using MediatR;
using NivoMaxBot.Application.Features.Orders.Dtos;

namespace NivoMaxBot.Application.Features.Orders.Queries
{
    public class GetOrderByIdQuery : IRequest<OrderDto>
    {
        public int OrderId { get; set; }
    }
}
