namespace MarketLinker.Domain.Entities.Marketplace.Auth;

public abstract class MarketplaceAuthBase
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public User.User User { get; set; } = null!;
    public string AccessToken { get; set; } = null!;
    public string? RefreshToken { get; set; }
    public DateTime TokenExpiration { get; set; }
}