using MarketLinker.Domain.Entities.Order;

namespace MarketLinker.Domain.Entities.Product;

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public int TotalStock { get; set; }
    public ICollection<ProductMarketplace> ProductMarketplaces { get; set; } = new List<ProductMarketplace>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}