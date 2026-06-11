using MediatR;

namespace NivoMaxBot.Application.Features.Basket.Commands.UpdateCartItem
{
    public class UpdateCartItemQuantityCommand : IRequest<bool>
    {
        public int BasketDetailId { get; set; }
        public int NewQuantity { get; set; }
        public int UserId { get; set; } // для проверки принадлежности
    }
}
