namespace MarketLinker.Domain.Entities.Order;

public class Order
{
    public Guid Id { get; set; }
    public Guid MarketplaceId { get; set; }
    public string ExternalOrderCode { get; set; } = null!;
    public DateTime Date { get; set; }
    public OrderStatus Status { get; set; }
}