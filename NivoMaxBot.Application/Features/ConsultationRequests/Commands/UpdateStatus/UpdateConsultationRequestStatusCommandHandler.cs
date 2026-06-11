using MediatR;
using NivoMaxBot.Application.Common.Exceptions;
using NivoMaxBot.Domain.Entities;
using NivoMaxBot.Domain.Interfaces.Repositories;

namespace NivoMaxBot.Application.Features.ConsultationRequests.Commands.UpdateStatus
{
    public class UpdateConsultationRequestStatusCommandHandler 
        : IRequestHandler<UpdateConsultationRequestStatusCommand, bool>
    {
        private readonly IConsultationRequestRepository _repository;

        public UpdateConsultationRequestStatusCommandHandler(
            IConsultationRequestRepository repository)
        {
            _repository = repository;
        }
        public async Task<bool> Handle(UpdateConsultationRequestStatusCommand request, CancellationToken ct)
        {
            var cr = await _repository.GetByIdAsync(request.RequestId, ct) 
                ?? throw new NotFoundException(nameof(ConsultationRequest), request.RequestId);
            
            if (cr.Status == request.NewStatus) return true;
            cr.Status = request.NewStatus;
            _repository.Update(cr);
            await _repository.SaveChangesAsync(ct);

            return true;
        }
    }
}
