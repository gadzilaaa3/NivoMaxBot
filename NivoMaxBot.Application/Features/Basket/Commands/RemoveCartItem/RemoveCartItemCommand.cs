using MediatR;

namespace NivoMaxBot.Application.Features.Basket.Commands.RemoveCartItem
{
    public class RemoveCartItemCommand : IRequest<bool>
    {
        public int BasketDetailId { get; set; }
        public int UserId { get; set; }
    }
}
