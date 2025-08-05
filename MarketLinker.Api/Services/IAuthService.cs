namespace MarketLinker.Api.Services;

public interface IAuthService
{
    string GenerateToken(string userId);
}