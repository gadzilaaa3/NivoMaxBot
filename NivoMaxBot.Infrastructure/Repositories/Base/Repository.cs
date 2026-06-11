using Microsoft.EntityFrameworkCore;
using NivoMaxBot.Domain.Entities.Base;
using NivoMaxBot.Domain.Interfaces.Repositories.Base;
using NivoMaxBot.Infrastructure.Data;
using NivoMaxBot.Shared.Pagination;

namespace NivoMaxBot.Infrastructure.Repositories.Base
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        protected Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        protected async Task<PagedResult<T>> GetPagedAsync(IQueryable<T> query, PagedRequest request, CancellationToken cancellationToken)
        {
            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        }

        public virtual async Task<IEnumerable<T>> GetByIdsAsync(IEnumerable<int> ids, 
            CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(e => ids.Contains(e.Id)).ToListAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.ToListAsync(cancellationToken);
        }

        public virtual async Task<PagedResult<T>> GetAllPagedAsync(PagedRequest request, CancellationToken cancellationToken = default)
        {
            return await GetPagedAsync(_dbSet.AsQueryable(), request, cancellationToken);
        }

        public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
        }

        public virtual void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public virtual void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public virtual async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(e => e.Id == id, cancellationToken);
        }

        public virtual async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
