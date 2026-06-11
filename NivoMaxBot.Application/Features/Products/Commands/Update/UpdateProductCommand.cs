using MediatR;
using NivoMaxBot.Application.Common.Attributes;

namespace NivoMaxBot.Application.Features.Products.Commands.Update
{
    [Authorize(RequiredRole = AdminRole.Admin)]
    public class UpdateProductCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int WarrantyInMonths { get; set; }
        public string? PhotoMaxFileId { get; set; }
        public string? PhotoUrl { get; set; }
        public bool IsAvailable { get; set; }
        public IEnumerable<int> CategoryIds { get; set; } = [];
    }
}
