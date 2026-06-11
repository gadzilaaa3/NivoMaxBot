using MediatR;
using NivoMaxBot.Application.Features.ConsultationRequests.Dtos;
using NivoMaxBot.Application.Features.Users.Commands.Register;
using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Domain.Constants;
using NivoMaxBot.Domain.Entities;
using NivoMaxBot.Domain.Interfaces.Repositories;

namespace NivoMaxBot.Application.Features.ConsultationRequests.Commands.Create
{
    public class CreateConsultationCommandHandler 
        : IRequestHandler<CreateConsultationCommand, int>
    {
        private readonly IConsultationRequestRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;
        private readonly INotificationService _notificationService;

        public CreateConsultationCommandHandler(
            IConsultationRequestRepository repository, 
            IUserRepository userRepository,
            IMediator mediator,
            INotificationService notificationService)
        {
            _repository = repository;
            _userRepository = userRepository;
            _mediator = mediator;
            _notificationService = notificationService;
        }

        public async Task<int> Handle(CreateConsultationCommand request, 
            CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByMaxIdAsync(request.UserMaxId, cancellationToken);
            if (user == null)
            {
                var id = await _mediator.Send(new RegisterUserCommand
                {
                    MaxId = request.UserMaxId
                });
                user = await _userRepository.GetByIdAsync(id);
            }

            var consultation = new ConsultationRequest
            {
                UserId = user.Id,
                ContactName = request.ContactName,
                City = request.City,
                PhoneNumber = request.PhoneNumber,
                Description = request.Description,
                Status = ConsultationRequestStatus.New // начальный статус "New"
            };

            await _repository.AddAsync(consultation, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            // Формируем DTO для уведомления
            var dto = new ConsultationRequestDto
            {
                Id = consultation.Id,
                UserId = consultation.UserId,
                CustomerName = consultation.ContactName,
                City = consultation.City,
                PhoneNumber = consultation.PhoneNumber,
                Description = consultation.Description,
                Status = consultation.Status,
                CreatedAt = consultation.CreatedAt
            };

            await _notificationService.SendConsultationRequestNotification(dto, cancellationToken);

            return consultation.Id;
        }
    }
}
