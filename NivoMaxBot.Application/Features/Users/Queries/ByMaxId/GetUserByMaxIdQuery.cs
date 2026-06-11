using MediatR;
using NivoMaxBot.Application.Features.Users.Dtos;

namespace NivoMaxBot.Application.Features.Users.Queries.ByMaxId
{
    public class GetUserByMaxIdQuery : IRequest<UserDto?>
    {
        public long MaxId { get; set; }
    }
}
