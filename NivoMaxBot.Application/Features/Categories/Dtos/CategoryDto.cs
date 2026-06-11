namespace NivoMaxBot.Application.Features.Categories.Dtos
{
    public class CategoryDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int Order { get; set; }

        public int? ParentId { get; set; }

        public string? ParentName { get; set; }

        public bool HasChildren { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
