namespace MarketLinker.Domain.Entities.Tracking;
using OrderEntity = MarketLinker.Domain.Entities.Order.Order;

public class ShipmentTracking
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid OrderId { get; set; }
    public string Carrier { get; set; } = null!;
    public string TrackingCode { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime PostingDate { get; set; }
    public DateTime DeliveryDate { get; set; }
    public OrderEntity Order { get; set; } = null!;
}