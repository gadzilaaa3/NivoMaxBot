using MediatR;
using NivoMaxBot.Application.Common.Exceptions;
using NivoMaxBot.Application.Features.Users.Commands.Register;
using NivoMaxBot.Domain.Constants;
using NivoMaxBot.Domain.Entities;
using NivoMaxBot.Domain.Interfaces.Repositories;

namespace NivoMaxBot.Application.Features.WarrantyRequest.Commands.Create
{
    public class CreateWarrantyRequestCommandHandler : IRequestHandler<CreateWarrantyRequestCommand, int>
    {
        private readonly IWarrantyRequestRepository _requestRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;

        public CreateWarrantyRequestCommandHandler(
            IWarrantyRequestRepository requestRepository, 
            IUserRepository userRepository,
            IMediator mediator)
        {
            _requestRepository = requestRepository;
            _userRepository = userRepository;
            _mediator = mediator;
        }

        public async Task<int> Handle(CreateWarrantyRequestCommand request, CancellationToken cancellationToken)
        {
            // Создаем пользователя если его нет
            var user = await _userRepository.GetByMaxIdAsync(request.UserMaxId, cancellationToken);
            if (user == null)
            {
                var id = await _mediator.Send(new RegisterUserCommand
                {
                    MaxId = request.UserMaxId
                });
                user = await _userRepository.GetByIdAsync(id);
            }

            var warrantyRequest = new Domain.Entities.WarrantyRequest
            {
                UserId = user.Id,
                INN = request.INN,
                City = request.City,
                ContactPhone = request.ContactPhone,
                ContactPerson = request.ContactPerson,
                ContactEmail = request.ContactEmail,
                ProblemDescription = request.ProblemDescription,
                ProductSerialNumber = request.ProductSerialNumber,
                Status = WarrantyRequestStatus.New
            };

            await _requestRepository.AddAsync(warrantyRequest, cancellationToken);
            await _requestRepository.SaveChangesAsync(cancellationToken);
            return warrantyRequest.Id;
        }
    }
}
