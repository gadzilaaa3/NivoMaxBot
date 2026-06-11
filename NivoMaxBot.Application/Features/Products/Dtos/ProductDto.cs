namespace NivoMaxBot.Application.Features.Products.Dtos
{
    public class ProductDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public string? PhotoUrl { get; set; }

        public string? PhotoMaxFileId { get; set; }

        public bool IsAvailable { get; set; } = true;

        public int WarrantyInMonths { get; set; }

        public IEnumerable<int> CategoryIds { get; set; } = [];

        public IEnumerable<string> CategoryNames { get; set; } = [];
    }
}
