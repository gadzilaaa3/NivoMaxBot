using MediatR;
using NivoMaxBot.Application.Common.Attributes;
using NivoMaxBot.Application.Features.Products.Dtos;

namespace NivoMaxBot.Application.Features.Products.Commands.BulkCreate
{
    [Authorize(RequiredRole = AdminRole.Admin)]
    public class BulkCreateProductsCommand : IRequest<BulkCreateProductsResult>
    {
        public int CategoryId { get; set; }
        public IEnumerable<ProductImportDto> Products { get; set; } = [];
    }
}
