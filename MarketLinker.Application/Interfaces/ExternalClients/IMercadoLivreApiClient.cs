using MarketLinker.Application.DTOs.Marketplace.MercadoLivre;

namespace MarketLinker.Application.Interfaces.ExternalClients;

public interface IMercadoLivreApiClient
{
    void ValidateState(string? state, string? expectedState);
    Task<MercadoLivreTokenResponse>  ExchangeCodeForTokenAsync(string code, CancellationToken cancellationToken = default);
    Task SaveAuthDataAsync(Guid userId, MercadoLivreTokenResponse tokenData, CancellationToken cancellationToken = default);
    Task<MercadoLivreTokenResponse> RefreshTokenAsync(Guid userId, CancellationToken cancellationToken = default);
}