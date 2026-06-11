using MediatR;
using NivoMaxBot.Application.Common.Extensions;
using NivoMaxBot.Application.Features.ConsultationRequests.Dtos;
using NivoMaxBot.Domain.Interfaces.Repositories;
using NivoMaxBot.Shared.Pagination;

namespace NivoMaxBot.Application.Features.ConsultationRequests.Queries.Paged
{
    public class GetConsultationRequestsPagedQueryHandler 
        : IRequestHandler<GetConsultationRequestsPagedQuery, PagedResult<ConsultationRequestDto>>
    {
        private readonly IConsultationRequestRepository _repository;

        public GetConsultationRequestsPagedQueryHandler(
            IConsultationRequestRepository repository)
        {
            _repository = repository;
        }
        public async Task<PagedResult<ConsultationRequestDto>> Handle(GetConsultationRequestsPagedQuery request, CancellationToken ct)
        {
            var query = _repository.GetQueryWithFilter(request.StatusFilter);
            var projected = query.OrderByDescending(cr => cr.CreatedAt)
                .Select(cr => new ConsultationRequestDto
                {
                    Id = cr.Id,
                    UserId = cr.UserId,
                    CustomerName = cr.ContactName,
                    City = cr.City,
                    PhoneNumber = cr.PhoneNumber,
                    Description = cr.Description,
                    Status = cr.Status,
                    CreatedAt = cr.CreatedAt
                });
            return await projected.ToPagedResultAsync(request.PagedRequest, ct);
        }
    }
}
