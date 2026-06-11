using NivoMaxBot.Domain.Entities;
using NivoMaxBot.Domain.Interfaces.Repositories.Base;
using NivoMaxBot.Shared.Pagination;

namespace NivoMaxBot.Domain.Interfaces.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<Product?> GetByIdWithCategoriesAsync(int id, CancellationToken cancellationToken = default);

        Task<IEnumerable<Product>> GetAvailableProductsAsync(CancellationToken cancellationToken = default);

        Task<PagedResult<Product>> GetAvailableProductsPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);

        Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId, CancellationToken cancellationToken = default);

        Task<PagedResult<Product>> GetByCategoryIdPagedAsync(int categoryId, PagedRequest request,
            bool includeUnavailable,
            CancellationToken cancellationToken = default);

        Task<List<string>> GetProductNamesInCategoryAsync(int categoryId, CancellationToken cancellationToken = default);

        Task<bool> ExistsInCategoryAsync(int categoryId, string name, CancellationToken cancellationToken = default);
    }
}
