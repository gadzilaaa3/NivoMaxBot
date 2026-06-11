using MediatR;
using NivoMaxBot.Application.Common.Extensions;
using NivoMaxBot.Application.Features.WarrantyRequest.Dtos;
using NivoMaxBot.Domain.Interfaces.Repositories;
using NivoMaxBot.Shared.Pagination;

namespace NivoMaxBot.Application.Features.WarrantyRequest.Queries.Paged
{
    public class GetUserWarrantyRequestsPagedQueryHandler 
        : IRequestHandler<GetUserWarrantyRequestsPagedQuery, PagedResult<WarrantyRequestDto>>
    {
        private readonly IWarrantyRequestRepository _requestRepository;

        public GetUserWarrantyRequestsPagedQueryHandler(
            IWarrantyRequestRepository warrantyRequestRepository)
        {
            _requestRepository = warrantyRequestRepository;
        }

        public async Task<PagedResult<WarrantyRequestDto>> Handle(GetUserWarrantyRequestsPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _requestRepository.GetUserRequestsQuery(request.UserId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new WarrantyRequestDto
                {
                    Id = r.Id,
                    INN = r.INN,
                    City = r.City,
                    ContactPhone = r.ContactPhone,
                    ContactPerson = r.ContactPerson,
                    ContactEmail = r.ContactEmail,
                    ProblemDescription = r.ProblemDescription,
                    ProductSerialNumber = r.ProductSerialNumber,
                    Status = r.Status,
                    CreatedAt = r.CreatedAt
                });

            return await query.ToPagedResultAsync(request.PagedRequest, cancellationToken);
        }
    }
}
