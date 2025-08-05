using MarketLinker.Domain.Entities.Tracking;

namespace MarketLinker.Domain.Entities.Order;

public class Order
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid MarketplaceId { get; set; }
    public string ExternalOrderCode { get; set; } = null!;
    public DateTime Date { get; set; }
    public OrderStatus Status { get; set; }
    public ShipmentTracking ShipmentTracking { get; set; } = null!;
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}