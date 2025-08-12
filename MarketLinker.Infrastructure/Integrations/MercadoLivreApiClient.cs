using System.Text.Json;
using MarketLinker.Application.DTOs.Marketplace.MercadoLivre;
using MarketLinker.Application.Interfaces.ExternalClients;
using MarketLinker.Domain.Entities.Marketplace.Auth;
using MarketLinker.Domain.Exceptions;
using MarketLinker.Domain.Repositories;
using Microsoft.Extensions.Configuration;

namespace MarketLinker.Infrastructure.Integrations;

public class MercadoLivreApiClient : IMercadoLivreApiClient
{
    private readonly IConfiguration _config;
    private readonly IMercadoLivreAuthRepository _authRepo;
    
    public MercadoLivreApiClient(IConfiguration config, IMercadoLivreAuthRepository authRepo)
    {
        _config = config;
        _authRepo = authRepo;
    }

    public void ValidateState(string? state, string? expectedState)
    {
        if (string.IsNullOrEmpty(state))
            throw new BadRequestException("Missing or invalid state parameter.");
        if (string.IsNullOrEmpty(expectedState))
            throw new InvalidOperationException("Expected state not defined.");
        if (state != expectedState)
            throw new BadRequestException("Invalid state.");
    }

    public async Task<MercadoLivreTokenResponse> ExchangeCodeForTokenAsync(string code, CancellationToken cancellationToken = default)
    {
        var clientSecret = _config["MercadoLivre:Client_Secret"];
        var clientId = _config["MercadoLivre:Client_Id"];
        var redirectUri = _config["MercadoLivre:Redirect_Uri"];
        if (string.IsNullOrEmpty(clientSecret) || string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(redirectUri))
            throw new InvalidOperationException("Invalid MercadoLivre configuration. Please check ClientId, RedirectUri, and ClientSecret.");
        
        using var client = new HttpClient();
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("https://api.mercadolibre.com/oauth/token"),
            Headers =
            {
                { "accept", "application/json" },
            },
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "code", code },
                { "redirect_uri", redirectUri },
                { "code_verifier", "$CODE_VERIFIER" },
            }),
        };
        using var response = await client.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"Failed to exchange code for token: {error}");
        }
        
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        
        var tokenData = JsonSerializer.Deserialize<MercadoLivreTokenResponse>(body, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        if (tokenData is null)
            throw new InvalidOperationException("Invalid token response.");
        
        return tokenData;
    }

    public async Task SaveAuthDataAsync(Guid userId, MercadoLivreTokenResponse tokenResponse, CancellationToken cancellationToken = default)
    {
        var existingAuth = await _authRepo.GetByUserIdAsync(userId, cancellationToken);
        if (existingAuth != null)
        {
            existingAuth.AccessToken = tokenResponse.AccessToken;
            existingAuth.RefreshToken = tokenResponse.RefreshToken;
            existingAuth.TokenExpiration = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);
            existingAuth.MercadoLivreUserId = tokenResponse.UserId;
            await _authRepo.UpdateAsync(existingAuth, cancellationToken);
        }
        else
        {
            var authEntity = new MercadoLivreAuth
            {
                UserId = userId,
                AccessToken = tokenResponse.AccessToken,
                RefreshToken = tokenResponse.RefreshToken,
                TokenExpiration = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn),
                MercadoLivreUserId = tokenResponse.UserId
            };
            await _authRepo.AddAsync(authEntity, cancellationToken);
        }
    }
}