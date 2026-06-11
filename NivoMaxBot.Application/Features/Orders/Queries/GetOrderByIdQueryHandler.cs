using MediatR;
using NivoMaxBot.Application.Common.Exceptions;
using NivoMaxBot.Application.Features.Orders.Dtos;
using NivoMaxBot.Domain.Entities;
using NivoMaxBot.Domain.Interfaces.Repositories;

namespace NivoMaxBot.Application.Features.Orders.Queries
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrderByIdQueryHandler(IOrderRepository orderRepository) => _orderRepository = orderRepository;

        public async Task<OrderDto> Handle(GetOrderByIdQuery request, 
            CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdWithDetailsAsync(request.OrderId, cancellationToken);
            if (order == null)
                throw new NotFoundException(nameof(Order), request.OrderId);

            return new OrderDto
            {
                Id = order.Id,
                INN = order.INN,
                UserId = order.UserId,
                CustomerName = order.CustomerName,
                CustomerPhone = order.ContactPhone,
                CustomerEmail = order.ContactEmail,
                CreatedAt = order.CreatedAt,
                Status = order.Status,
                Items = order.Details.Select(d => new OrderItemDto
                {
                    ProductName = d.ProductNavigation.Name,
                    Quantity = d.ProductsQuantity,
                    Price = d.PriceAtOrder
                }).ToList()
            };
        }
    }
}
