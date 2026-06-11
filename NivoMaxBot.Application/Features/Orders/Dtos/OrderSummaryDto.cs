namespace NivoMaxBot.Application.Features.Orders.Dtos
{
    public class OrderSummaryDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int ItemsCount { get; set; }
    }
}
