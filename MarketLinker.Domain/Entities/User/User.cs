using MarketLinker.Domain.Entities.Marketplace.Auth;

namespace MarketLinker.Domain.Entities.User;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;

    public ICollection<MarketplaceAuthBase> MarketplaceAuths { get; set; } = new List<MarketplaceAuthBase>();
}