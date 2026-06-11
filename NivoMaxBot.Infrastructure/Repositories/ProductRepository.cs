using Microsoft.EntityFrameworkCore;
using NivoMaxBot.Domain.Entities;
using NivoMaxBot.Domain.Interfaces.Repositories;
using NivoMaxBot.Infrastructure.Data;
using NivoMaxBot.Infrastructure.Repositories.Base;
using NivoMaxBot.Shared.Pagination;

namespace NivoMaxBot.Infrastructure.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context) { }

        public async Task<Product?> GetByIdWithCategoriesAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .Include(p => p.Categories)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetAvailableProductsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .Where(p => p.IsAvailable)
                .Include(p => p.Categories)
                .OrderBy(p => p.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<PagedResult<Product>> GetAvailableProductsPagedAsync(PagedRequest request, CancellationToken cancellationToken = default)
        {
            var query = _context.Products
                .Where(p => p.IsAvailable)
                .Include(p => p.Categories)
                .OrderBy(p => p.Name)
                .AsQueryable();

            return await GetPagedAsync(query, request, cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .Where(p => p.Categories.Any(c => c.Id == categoryId) && p.IsAvailable)
                .Include(p => p.Categories)
                .OrderBy(p => p.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<PagedResult<Product>> GetByCategoryIdPagedAsync(int categoryId, PagedRequest request,
            bool includeUnavailable,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Products
                .Where(p => p.Categories.Any(c => c.Id == categoryId));

            if (!includeUnavailable)
                query = query.Where(p => p.IsAvailable);

            query.Include(p => p.Categories)
                .OrderBy(p => p.Name)
                .AsQueryable();

            return await GetPagedAsync(query, request, cancellationToken);
        }

        public async Task<List<string>> GetProductNamesInCategoryAsync(int categoryId, 
            CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .Where(p => p.Categories.Any(c => c.Id == categoryId))
                .Select(p => p.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsInCategoryAsync(int categoryId, 
            string name, CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .AnyAsync(p => p.Categories.Any(c => c.Id == categoryId) 
                && p.Name.ToLower() == name.ToLower(), cancellationToken);
        }
    }
}
