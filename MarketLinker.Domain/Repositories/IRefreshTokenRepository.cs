using MarketLinker.Domain.Entities.Marketplace.Auth;

namespace MarketLinker.Domain.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
    Task RevokeLastDeviceTokenAsync(Guid userId, string deviceName, CancellationToken cancellationToken = default);
}