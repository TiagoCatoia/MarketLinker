namespace MarketLinker.Domain.Entities.Marketplace.Auth;

public abstract class MarketplaceAuthBase
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User.User User { get; set; } = null!;
    public Guid MarketplaceId { get; set; }

    public string AccessToken { get; set; } = null!;
    public string? RefreshToken { get; set; }
    public DateTime TokenExpiration { get; set; }

    public string? ExternalUserId { get; set; }
}