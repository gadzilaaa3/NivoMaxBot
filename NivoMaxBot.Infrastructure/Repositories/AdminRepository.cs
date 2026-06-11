using Microsoft.EntityFrameworkCore;
using NivoMaxBot.Domain.Entities;
using NivoMaxBot.Domain.Interfaces.Repositories;
using NivoMaxBot.Infrastructure.Data;
using NivoMaxBot.Infrastructure.Repositories.Base;

namespace NivoMaxBot.Infrastructure.Repositories
{
    public class AdminRepository : Repository<Admin>, IAdminRepository
    {
        public AdminRepository(AppDbContext context) : base(context) { }

        public async Task<Admin?> GetByMaxIdAsync(long maxId, CancellationToken cancellationToken = default)
        {
            return await _context.Admins
                .FirstOrDefaultAsync(a => a.MaxId == maxId, cancellationToken);
        }

        public async Task<IEnumerable<Admin>> GetSuperAdminsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Admins
                .Where(a => a.IsSuperAdmin)
                .ToListAsync(cancellationToken);
        }
    }
}
