using MediatR;
using NivoMaxBot.Application.Common.Extensions;
using NivoMaxBot.Application.Features.Orders.Dtos;
using NivoMaxBot.Domain.Interfaces.Repositories;
using NivoMaxBot.Shared.Pagination;

namespace NivoMaxBot.Application.Features.Orders.Queries.Paged
{
    public class GetOrdersFilteredPagedQueryHandler : IRequestHandler<GetOrdersFilteredPagedQuery, PagedResult<OrderDto>>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrdersFilteredPagedQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        public async Task<PagedResult<OrderDto>> Handle(GetOrdersFilteredPagedQuery request, CancellationToken ct)
        {
            var query = _orderRepository.GetOrdersQuery(); // IQueryable<Order>

            if (!string.IsNullOrEmpty(request.StatusFilter))
                query = query.Where(o => o.Status == request.StatusFilter);
            var projected = query.OrderByDescending(o => o.CreatedAt).Select(o => new OrderDto
            {
                Id = o.Id,
                UserId = o.UserId,
                INN = o.INN,
                CustomerName = o.CustomerName,
                CustomerPhone = o.ContactPhone,
                CustomerEmail = o.ContactEmail,
                CreatedAt = o.CreatedAt,
                Status = o.Status,
                Items = o.Details.Select(d => new OrderItemDto
                {
                    ProductName = d.ProductNavigation.Name,
                    Quantity = d.ProductsQuantity,
                    Price = d.PriceAtOrder
                })
            });

            return await projected.ToPagedResultAsync(request.PagedRequest, ct);
        }
    }
}
