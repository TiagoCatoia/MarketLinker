namespace MarketLinker.Domain.Entities.Product;

public class ProductMarketplace
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid MarketplaceId { get; set; }
    public decimal Price { get; set; }
    public int ChannelStock { get; set; }
    public string Url { get; set; } = null!;
}