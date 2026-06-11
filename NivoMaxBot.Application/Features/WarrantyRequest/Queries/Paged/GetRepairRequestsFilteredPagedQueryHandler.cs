using MediatR;
using NivoMaxBot.Application.Common.Extensions;
using NivoMaxBot.Application.Features.WarrantyRequest.Dtos;
using NivoMaxBot.Domain.Interfaces.Repositories;
using NivoMaxBot.Shared.Pagination;

namespace NivoMaxBot.Application.Features.WarrantyRequest.Queries.Paged
{
    public class GetRepairRequestsFilteredPagedQueryHandler : IRequestHandler<GetRepairRequestsFilteredPagedQuery, PagedResult<WarrantyRequestDto>>
    {
        private readonly IWarrantyRequestRepository _warrantyRequestRepository;

        public GetRepairRequestsFilteredPagedQueryHandler(
            IWarrantyRequestRepository warrantyRequestRepository)
        {
            _warrantyRequestRepository = warrantyRequestRepository;
        }
        public async Task<PagedResult<WarrantyRequestDto>> Handle(GetRepairRequestsFilteredPagedQuery request, CancellationToken ct)
        {
            var query = _warrantyRequestRepository.GetWarrantyRequestsQuery();
            if (!string.IsNullOrEmpty(request.StatusFilter))
                query = query.Where(r => r.Status == request.StatusFilter);
            var projected = query.OrderByDescending(r => r.CreatedAt).Select(r => new WarrantyRequestDto
            {
                Id = r.Id,
                UserId = r.UserId,
                ContactPhone = r.ContactPhone,
                ContactEmail = r.ContactEmail,
                ProblemDescription = r.ProblemDescription,
                ProductSerialNumber = r.ProductSerialNumber,
                City = r.City,
                ContactPerson = r.ContactPerson,
                INN = r.ContactPhone,
                CreatedAt = r.CreatedAt,
                Status = r.Status
            });
            return await projected.ToPagedResultAsync(request.PagedRequest, ct);
        }
    }
}
