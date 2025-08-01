namespace MarketLinker.Domain.Entities.Product;

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public int TotalStock { get; set; }
}