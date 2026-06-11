using MediatR;
using NivoMaxBot.Application.Common.Extensions;
using NivoMaxBot.Application.Features.Orders.Dtos;
using NivoMaxBot.Domain.Interfaces.Repositories;
using NivoMaxBot.Shared.Pagination;

namespace NivoMaxBot.Application.Features.Orders.Queries
{
    public class GetUserOrdersPagedQueryHandler : IRequestHandler<GetUserOrdersPagedQuery, PagedResult<OrderSummaryDto>>
    {
        private readonly IOrderRepository _orderRepository;

        public GetUserOrdersPagedQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<PagedResult<OrderSummaryDto>> Handle(GetUserOrdersPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _orderRepository.GetUserOrdersQuery(request.UserId)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderSummaryDto
                {
                    Id = o.Id,
                    CreatedAt = o.CreatedAt,
                    Status = o.Status,
                    TotalAmount = o.Details.Sum(d => d.PriceAtOrder * d.ProductsQuantity),
                    ItemsCount = o.Details.Count
                });

            return await query.ToPagedResultAsync(request.PagedRequest, cancellationToken);
        }
    }
}
