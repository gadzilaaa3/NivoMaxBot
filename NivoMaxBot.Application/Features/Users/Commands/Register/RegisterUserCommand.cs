using MediatR;

namespace NivoMaxBot.Application.Features.Users.Commands.Register
{
    public class RegisterUserCommand : IRequest<int>
    {
        public long MaxId { get; set; }        
    }
}
