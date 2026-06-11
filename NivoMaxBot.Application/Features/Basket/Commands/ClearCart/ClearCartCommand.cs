using MediatR;

namespace NivoMaxBot.Application.Features.Basket.Commands.ClearCart
{
    public class ClearCartCommand : IRequest<bool>
    {
        public int UserId { get; set; }
    }
}
