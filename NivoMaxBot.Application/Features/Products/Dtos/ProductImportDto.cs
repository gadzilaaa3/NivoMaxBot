namespace NivoMaxBot.Application.Features.Products.Dtos
{
    public class ProductImportDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int WarrantyInMonths { get; set; }
        public string? PhotoUrl { get; set; }
        public bool? IsAvailable { get; set; }
    }
}
