using System.Text.Json.Serialization;

namespace MarketLinker.Application.DTOs.Marketplace.MercadoLivre;

public class MercadoLivreTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; } = null!;

    [JsonPropertyName("token_type")]
    public string TokenType { get; init; } = null!;

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; init; }

    [JsonPropertyName("scope")]
    public string Scope { get; init; } = null!;

    [JsonPropertyName("user_id")]
    public long UserId { get; init; }

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; init; } = null!;
}