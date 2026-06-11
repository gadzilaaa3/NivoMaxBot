using MediatR;
using NivoMaxBot.Application.Common.Exceptions;
using NivoMaxBot.Application.Features.ConsultationRequests.Dtos;
using NivoMaxBot.Domain.Entities;
using NivoMaxBot.Domain.Interfaces.Repositories;

namespace NivoMaxBot.Application.Features.ConsultationRequests.Queries.ById
{
    public class GetConsultationRequestByIdQueryHandler 
        : IRequestHandler<GetConsultationRequestByIdQuery, ConsultationRequestDto>
    {
        private readonly IConsultationRequestRepository _repository;

        public GetConsultationRequestByIdQueryHandler(
            IConsultationRequestRepository repository)
        {
            _repository = repository;
        }
        public async Task<ConsultationRequestDto> Handle(GetConsultationRequestByIdQuery request, CancellationToken ct)
        {
            var cr = await _repository.GetByIdWithUserAsync(request.Id, ct);
            if (cr == null)
                throw new NotFoundException(nameof(ConsultationRequest), request.Id);
            return new ConsultationRequestDto
            {
                Id = cr.Id,
                UserId = cr.UserId,
                CustomerName = cr.ContactName,
                City = cr.City,
                PhoneNumber = cr.PhoneNumber,
                Description = cr.Description,
                Status = cr.Status,
                CreatedAt = cr.CreatedAt
            };
        }
    }
}
