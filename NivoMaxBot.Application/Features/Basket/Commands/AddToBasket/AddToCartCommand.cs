using MediatR;

namespace NivoMaxBot.Application.Features.Basket.Commands.AddToBasket
{
    public class AddToCartCommand : IRequest<bool>
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
