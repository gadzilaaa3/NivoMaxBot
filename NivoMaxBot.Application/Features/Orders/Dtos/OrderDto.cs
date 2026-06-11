namespace NivoMaxBot.Application.Features.Orders.Dtos
{
    public class OrderDto
    {
        public int Id { get; set; }
        
        public int UserId { get; set; }

        public string Status { get; set; } = string.Empty;

        public string CustomerPhone { get; set; } = string.Empty;

        public string CustomerName { get; set; } = string.Empty;

        public string? CustomerEmail { get; set; }

        public string? INN { get; set; }

        public DateTime CreatedAt { get; set; }

        public IEnumerable<OrderItemDto> Items { get; set; } = [];

        public decimal TotalAmount => Items.Sum(i => i.Total);
    }
}
