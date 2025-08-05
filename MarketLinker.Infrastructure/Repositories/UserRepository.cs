using MarketLinker.Domain.Entities.User;
using MarketLinker.Domain.Repositories;
using MarketLinker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MarketLinker.Infrastructure.Repositories;

public class UserRepository : BaseRepository<User>,IUserRepository
{
    public UserRepository(MarketLinkerDbContext dbContext) : base(dbContext) { }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
}