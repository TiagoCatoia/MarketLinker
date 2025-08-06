using MarketLinker.Application.DTOs.Auth;

namespace MarketLinker.Api.Services;

public interface IAuthService
{
    string GenerateAccessToken(string userId);
    string GenerateRefreshToken();
    Task<TokenResponseDto> RefreshTokenAsync(string refreshToken, string deviceName, CancellationToken cancellationToken);
    Task<TokenResponseDto> GenerateAndSaveTokensAsync(Guid userId, string deviceName, CancellationToken cancellationToken);
}