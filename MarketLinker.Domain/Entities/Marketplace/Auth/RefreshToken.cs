using UserEntity = MarketLinker.Domain.Entities.User.User;

namespace MarketLinker.Domain.Entities.Marketplace.Auth;

public class RefreshToken
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Token { get; set; } = null!;
    public DateTime ExpiryDate { get; set; }
    public bool IsRevoked { get; set; }
    public string DeviceName { get; set; } = null!;
    
    public Guid UserId { get; set; }
    public UserEntity User { get; set; } = null!;
    
    public bool IsExpired() => ExpiryDate < DateTime.UtcNow;
}