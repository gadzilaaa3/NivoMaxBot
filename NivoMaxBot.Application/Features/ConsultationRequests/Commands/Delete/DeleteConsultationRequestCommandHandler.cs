using MediatR;
using NivoMaxBot.Application.Common.Exceptions;
using NivoMaxBot.Domain.Constants;
using NivoMaxBot.Domain.Entities;
using NivoMaxBot.Domain.Interfaces.Repositories;

namespace NivoMaxBot.Application.Features.ConsultationRequests.Commands.Delete
{
    public class DeleteConsultationRequestCommandHandler 
        : IRequestHandler<DeleteConsultationRequestCommand, bool>
    {
        private readonly IConsultationRequestRepository _repository;

        public DeleteConsultationRequestCommandHandler(
            IConsultationRequestRepository repository)
        {
            _repository = repository;
        }
        public async Task<bool> Handle(DeleteConsultationRequestCommand request, CancellationToken ct)
        {
            var cr = await _repository.GetByIdAsync(request.RequestId, ct);
            if (cr == null) throw new NotFoundException(nameof(ConsultationRequest), request.RequestId);
            if (cr.Status != ConsultationRequestStatus.Completed)
                throw new BusinessRuleViolationException(
                    $"Удалить можно только заявки со статусом '{ConsultationRequestStatus.Completed}'.");
            _repository.Remove(cr);
            await _repository.SaveChangesAsync(ct);
            return true;
        }
    }
}
