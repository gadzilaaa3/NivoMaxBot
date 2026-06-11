using NivoMaxBot.Domain.Entities;
using NivoMaxBot.Domain.Interfaces.Repositories.Base;
using NivoMaxBot.Shared.Pagination;

namespace NivoMaxBot.Domain.Interfaces.Repositories
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<PagedResult<Category>> GetAllCategoriesPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);

        Task<IEnumerable<Category>> GetCategoriesByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);

        Task<IEnumerable<Category>> GetCategoriesWithChildrenAsync(int? parentId, CancellationToken cancellationToken = default);

        Task<Category?> GetCategoryWithChildrenAsync(int id, CancellationToken cancellationToken = default);

        Task<IEnumerable<Category>> GetTreeAsync(CancellationToken cancellationToken = default);

        Task<bool> HasChildrenAsync(int id, CancellationToken cancellationToken = default);

        Task<Category?> GetByIdWithParentAsync(int id, CancellationToken cancellationToken = default);

        Task<bool> IsAncestorOfAsync(int ancestorId, int descendantId, 
            CancellationToken cancellationToken = default);

        Task<IEnumerable<int>> GetDescendantIdsAsync(int categoryId, CancellationToken cancellationToken = default);

        Task<bool> HasProductsAsync(int categoryId, CancellationToken cancellationToken = default);

        Task<bool> AnyHasProductsAsync(IEnumerable<int> categoryIds, CancellationToken cancellationToken = default);
    }
}
