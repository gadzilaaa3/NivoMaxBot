namespace NivoMaxBot.Application.Features.Products.Commands.BulkCreate
{
    public class BulkCreateProductsResult
    {
        public int SuccessCount { get; set; }

        public IEnumerable<string> Errors { get; set; } = [];

        public bool IsSuccess => !Errors.Any();
    }
}
