using MarketLinker.Domain.Entities.User;
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
    }

    public async Task RevokeLastDeviceTokenAsync(Guid userId, string deviceName, CancellationToken cancellationToken = default)
    {
        var token = await GetByUserIdAndDeviceNameAsync(userId, deviceName, cancellationToken);
        
        if (token is not null && !token.IsRevoked)
        {
            token.IsRevoked = true;
            _dbContext.RefreshTokens.Update(token);
        }
    }

    private async Task<RefreshToken?> GetByUserIdAndDeviceNameAsync(Guid userId, string deviceName, CancellationToken cancellationToken)
    {
        return await _dbContext.RefreshTokens
            .Where(rf => rf.UserId == userId && rf.DeviceName == deviceName)
            .FirstOrDefaultAsync(cancellationToken);
    }
}