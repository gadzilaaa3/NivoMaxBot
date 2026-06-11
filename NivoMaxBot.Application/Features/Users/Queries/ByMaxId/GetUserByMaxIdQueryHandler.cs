using MediatR;
using NivoMaxBot.Application.Features.Users.Dtos;
using NivoMaxBot.Domain.Interfaces.Repositories;

namespace NivoMaxBot.Application.Features.Users.Queries.ByMaxId
{
    public class GetUserByMaxIdQueryHandler 
        : IRequestHandler<GetUserByMaxIdQuery, UserDto?>
    {
        private readonly IUserRepository _userRepository;

        public GetUserByMaxIdQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDto?> Handle(GetUserByMaxIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByMaxIdAsync(request.MaxId, cancellationToken);
            if (user == null)
                return null;

            return new UserDto
            {
                Id = user.Id,
                MaxId = user.MaxId,
                CreatedAt = user.CreatedAt,
            };
        }
    }
}
