namespace MarketLinker.Domain.Entities.Tracking;

public class ShipmentTracking
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string Carrier { get; set; } = null!;
    public string TrackingCode { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime PostingDate { get; set; }
    public DateTime DeliveryDate { get; set; }
}