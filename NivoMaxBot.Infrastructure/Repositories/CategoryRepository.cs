using Microsoft.EntityFrameworkCore;
using NivoMaxBot.Domain.Entities;
using NivoMaxBot.Domain.Interfaces.Repositories;
using NivoMaxBot.Infrastructure.Data;
using NivoMaxBot.Infrastructure.Repositories.Base;
using NivoMaxBot.Shared.Pagination;

namespace NivoMaxBot.Infrastructure.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context) { }

        public async Task<PagedResult<Category>> GetAllCategoriesPagedAsync(PagedRequest request, CancellationToken cancellationToken = default)
        {
            var query = _context.Categories
                .OrderBy(c => c.Order)
                .ThenBy(c => c.Name)
                .AsQueryable();

            return await GetPagedAsync(query, request, cancellationToken);
        }

        public async Task<Category?> GetCategoryWithChildrenAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .Include(c => c.Children)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Category>> GetTreeAsync(CancellationToken cancellationToken = default)
        {
            var categories = await _context.Categories
                .Include(c => c.Children)
                .ToListAsync(cancellationToken);
            return categories.Where(c => c.ParentId == null).OrderBy(c => c.Order).ToList();
        }

        public async Task<bool> HasChildrenAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Categories.AnyAsync(c => c.ParentId == id, cancellationToken);
        }

        public async Task<Category?> GetByIdWithParentAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .Include(c => c.ParentNavigation)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<bool> IsAncestorOfAsync(int ancestorId, 
            int descendantId, CancellationToken cancellationToken = default)
        {
            var allCategories = await _context.Categories.ToListAsync(cancellationToken);
            var visited = new HashSet<int>();
            var stack = new Stack<int>();
            stack.Push(ancestorId);

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                if (current == descendantId)
                    return true;
                if (!visited.Add(current))
                    continue;
                var children = allCategories.Where(c => c.ParentId == current).Select(c => c.Id).ToList();
                foreach (var child in children)
                    stack.Push(child);
            }
            return false;
        }

        public async Task<IEnumerable<int>> GetDescendantIdsAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            // Загружаем все категории (оптимизация: можно сделать рекурсивный SQL, но для простоты загрузим)
            var allCategories = await _context.Categories.ToListAsync(cancellationToken);
            var result = new List<int>();
            var stack = new Stack<int>();
            stack.Push(categoryId);

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                result.Add(current);
                var children = allCategories.Where(c => c.ParentId == current).Select(c => c.Id);
                foreach (var child in children)
                    stack.Push(child);
            }
            return result;
        }

        public async Task<bool> HasProductsAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            return await _context.Products.AnyAsync(p => p.Categories.Any(c => c.Id == categoryId), cancellationToken);
        }

        public async Task<bool> AnyHasProductsAsync(IEnumerable<int> categoryIds, CancellationToken cancellationToken = default)
        {
            return await _context.Products.AnyAsync(p => p.Categories.Any(c => categoryIds.Contains(c.Id)), cancellationToken);
        }

        public async Task<IEnumerable<Category>> GetCategoriesWithChildrenAsync(int? parentId, 
            CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .Where(c => c.ParentId == parentId)
                .Include(c => c.Children)
                .OrderBy(c => c.Order)
                .ThenBy(c => c.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Category>> GetCategoriesByIdsAsync(IEnumerable<int> ids, 
            CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .Where(c => ids.Contains(c.Id))
                .Include(c => c.Children)
                .Include(c => c.ParentNavigation)
                .OrderBy(c => c.Order)
                .ThenBy(c => c.Name)
                .ToListAsync(cancellationToken);
        }
    }
}
