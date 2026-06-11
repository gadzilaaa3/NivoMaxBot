using MediatR;
using NivoMaxBot.Application.Common.Exceptions;
using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Domain.Interfaces.Repositories;

namespace NivoMaxBot.Application.Features.WarrantyRequest.Commands.Update
{
    public class UpdateWarrantyRequestStatusCommandHandler : IRequestHandler<UpdateWarrantyRequestStatusCommand, bool>
    {
        private readonly IWarrantyRequestRepository _warrantyRequestRepository;
        private readonly INotificationService _notificationService;
        private readonly IUserRepository _userRepository;

        public UpdateWarrantyRequestStatusCommandHandler(
            IWarrantyRequestRepository warrantyRequestRepository,
            INotificationService notificationService,
            IUserRepository userRepository)
        {
            _warrantyRequestRepository = warrantyRequestRepository;
            _notificationService = notificationService;
            _userRepository = userRepository;
        }
        public async Task<bool> Handle(UpdateWarrantyRequestStatusCommand request, CancellationToken ct)
        {
            var req = await _warrantyRequestRepository.GetByIdWithUserAsync(request.RequestId, ct);
            if (req == null) throw new NotFoundException(nameof(WarrantyRequest), request.RequestId);
            if (req.Status == request.NewStatus) return true;

            req.Status = request.NewStatus;
            _warrantyRequestRepository.Update(req);
            await _warrantyRequestRepository.SaveChangesAsync(ct);
            var user = await _userRepository.GetByIdAsync(req.UserId, ct);
            if (user != null)
            {
                var message = $"🔧 Статус вашей заявки на ремонт #{req.Id} изменён на: *{request.NewStatus}*";
                await _notificationService.SendMessageToUserAsync(user.MaxId, message, ct);
            }
            return true;
        }
    }
}
