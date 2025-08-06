using MarketLinker.Domain.Entities.Marketplace.Auth;
using MarketLinker.Domain.Repositories;
using MarketLinker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MarketLinker.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly MarketLinkerDbContext _dbContext;
    
    public RefreshTokenRepository(MarketLinkerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(rf => rf.Token == token, cancellationToken);
    }

    public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        await _dbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokeAsync(Guid tokenId, CancellationToken cancellationToken = default)
    {
        var refreshToken = await _dbContext.RefreshTokens.FindAsync(new object[] { tokenId }, cancellationToken);
        if (refreshToken is not null && !refreshToken.IsRevoked)
        {
            refreshToken.IsRevoked = true;
            _dbContext.RefreshTokens.Update(refreshToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}