using MarketLinker.Domain.Entities.Product;
using OrderEntity = MarketLinker.Domain.Entities.Order.Order;

namespace MarketLinker.Domain.Entities.Marketplace;

public class Marketplace
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<ProductMarketplace> ProductMarketplaces { get; set; } = new List<ProductMarketplace>();
    public ICollection<OrderEntity> Orders { get; set; } = new List<OrderEntity>();
}