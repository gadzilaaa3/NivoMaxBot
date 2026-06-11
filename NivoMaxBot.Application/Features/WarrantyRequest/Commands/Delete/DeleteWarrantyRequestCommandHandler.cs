using MediatR;
using NivoMaxBot.Application.Common.Exceptions;
using NivoMaxBot.Domain.Constants;
using NivoMaxBot.Domain.Interfaces.Repositories;

namespace NivoMaxBot.Application.Features.WarrantyRequest.Commands.Delete
{
    public class DeleteWarrantyRequestCommandHandler : IRequestHandler<DeleteWarrantyRequestCommand, bool>
    {
        private readonly IWarrantyRequestRepository _warrantyRequestRepository;

        public DeleteWarrantyRequestCommandHandler(
            IWarrantyRequestRepository warrantyRequestRepository)
        {
            _warrantyRequestRepository = warrantyRequestRepository;
        }
        public async Task<bool> Handle(DeleteWarrantyRequestCommand request, CancellationToken ct)
        {
            var req = await _warrantyRequestRepository.GetByIdAsync(request.RequestId, ct);
            if (req == null) throw new NotFoundException(nameof(WarrantyRequest), request.RequestId);

            // Разрешаем удаление только со статусом "Завершена"
            if (req.Status != WarrantyRequestStatus.Completed)
                throw new BusinessRuleViolationException(
                    $"Удалить можно только заявки со статусом '{WarrantyRequestStatus.Completed}'.");
            _warrantyRequestRepository.Remove(req);
            await _warrantyRequestRepository.SaveChangesAsync(ct);
            return true;
        }
    }
}
