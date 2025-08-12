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
    private readonly HttpClient _httpClient;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };


    public MercadoLivreApiClient(IHttpClientFactory httpClientFactory, IConfiguration config, IMercadoLivreAuthRepository authRepo)
    {
        _config = config;
        _authRepo = authRepo;
        _httpClient = httpClientFactory.CreateClient("MercadoLivre");
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
    
    public async Task SaveAuthDataAsync(
        Guid userId, 
        MercadoLivreTokenResponse tokenResponse,
        CancellationToken cancellationToken = default)
    {
        var existingAuth = await GetMlAuthByUserId(userId, cancellationToken);
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

    public async Task<MercadoLivreTokenResponse> ExchangeCodeForTokenAsync(
        string code, CancellationToken cancellationToken = default)
    {
        var config = GetMlConfig();
        var form = new Dictionary<string, string>
        {
            { "grant_type", "authorization_code" },
            { "client_id", config.ClientId },
            { "client_secret", config.ClientSecret },
            { "code", code },
            { "redirect_uri", config.RedirectUri },
            { "code_verifier", "$CODE_VERIFIER" },
        };

        return await PostFormAsync<MercadoLivreTokenResponse>(
            "https://api.mercadolibre.com/oauth/token",
            form,
            cancellationToken);
    }
    
    public async Task<MercadoLivreTokenResponse> RefreshTokenAsync(Guid userId,
        CancellationToken cancellationToken = default)
    {
        var config = GetMlConfig();

        var auth  = await GetMlAuthByUserId(userId, cancellationToken)
                    ?? throw new NotFoundException("Mercado Livre auth data not found.");

        var refreshToken = GetRefreshTokenByMlAuth(auth);

        var form = new Dictionary<string, string>
        {
            { "grant_type", "refresh_token" },
            { "client_id", config.ClientId },
            { "client_secret", config.ClientSecret },
            { "refresh_token", refreshToken }
        };

        return await PostFormAsync<MercadoLivreTokenResponse>(
            "https://api.mercadolibre.com/oauth/token",
            form,
            cancellationToken);
    }

    private (string ClientId, string ClientSecret, string RedirectUri) GetMlConfig()
    {
        var clientSecret = _config["MercadoLivre:Client_Secret"];
        var clientId = _config["MercadoLivre:Client_Id"];
        var redirectUri = _config["MercadoLivre:Redirect_Uri"];

        if (string.IsNullOrEmpty(clientSecret) || string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(redirectUri))
            throw new InvalidOperationException("Invalid MercadoLivre configuration.");

        return (clientId, clientSecret, redirectUri);
    }

    private async Task<MercadoLivreAuth?> GetMlAuthByUserId(Guid userId, CancellationToken cancellationToken) =>
        await _authRepo.GetByUserIdAsync(userId, cancellationToken);

    private static string GetRefreshTokenByMlAuth(MercadoLivreAuth auth) =>
        string.IsNullOrEmpty(auth.RefreshToken)
            ? throw new InvalidOperationException("Refresh token not registered.")
            : auth.RefreshToken;

    private async Task<T> PostFormAsync<T>(
        string url,
        Dictionary<string, string> form,
        CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Headers = { { "accept", "application/json" } },
            Content = new FormUrlEncodedContent(form)
        };
        
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"Request failed: {body}");
        
        return JsonSerializer.Deserialize<T>(body, JsonOptions)
               ?? throw new InvalidOperationException("Invalid deserialization result.");
    }
}