using MediatR;
using NivoMaxBot.Application.Common.Exceptions;
using NivoMaxBot.Application.Features.WarrantyRequest.Dtos;
using NivoMaxBot.Domain.Interfaces.Repositories;

namespace NivoMaxBot.Application.Features.WarrantyRequest.Queries.ById
{
    public class GetWarrantyRequestByIdQueryHandler 
        : IRequestHandler<GetWarrantyRequestByIdQuery, WarrantyRequestDto>
    {
        private readonly IWarrantyRequestRepository _requestRepository;

        public GetWarrantyRequestByIdQueryHandler(
            IWarrantyRequestRepository warrantyRequestRepository)
        {
            _requestRepository = warrantyRequestRepository;
        }

        public async Task<WarrantyRequestDto> Handle(GetWarrantyRequestByIdQuery request, CancellationToken cancellationToken)
        {
            var req = await _requestRepository.GetByIdAsync(request.Id, cancellationToken);
            if (req == null)
                throw new NotFoundException(nameof(WarrantyRequest), request.Id);

            return new WarrantyRequestDto
            {
                Id = req.Id,
                UserId = req.UserId,
                INN = req.INN,
                City = req.City,
                ContactPhone = req.ContactPhone,
                ContactPerson = req.ContactPerson,
                ContactEmail = req.ContactEmail,
                ProblemDescription = req.ProblemDescription,
                ProductSerialNumber = req.ProductSerialNumber,
                Status = req.Status,
                CreatedAt = req.CreatedAt
            };
        }
    }
}
