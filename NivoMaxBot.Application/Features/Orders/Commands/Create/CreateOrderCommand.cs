using MediatR;

namespace NivoMaxBot.Application.Features.Orders.Commands.Create
{
    public class CreateOrderCommand : IRequest<int>
    {
        public long UserMaxId { get; set; }

        public string ContactPhone { get; set; } = string.Empty;

        public string CustomerName { get; set; } = string.Empty;

        public string? ContactEmail { get; set; }

        public string? INN { get; set; }
    }
}
