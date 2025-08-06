namespace MarketLinker.Application.DTOs.Auth;

public class TokenResponseDto
{
    public string AccessToken { get; init; } = null!;
    public string RefreshToken { get; init; } = null!;
}