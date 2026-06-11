using Microsoft.EntityFrameworkCore;
using NivoMaxBot.Domain.Entities;
using NivoMaxBot.Domain.Interfaces.Repositories;
using NivoMaxBot.Infrastructure.Data;
using NivoMaxBot.Infrastructure.Repositories.Base;

namespace NivoMaxBot.Infrastructure.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public async Task<User?> GetByMaxIdAsync(long maxId, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.MaxId == maxId, cancellationToken);
        }
    }
}
