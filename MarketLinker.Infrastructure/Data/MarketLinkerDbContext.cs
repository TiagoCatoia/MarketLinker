using MarketLinker.Domain.Entities.Marketplace;
using MarketLinker.Domain.Entities.Marketplace.Auth;
using MarketLinker.Domain.Entities.Marketplace.Enum;
using MarketLinker.Domain.Entities.Order;
using MarketLinker.Domain.Entities.Product;
using MarketLinker.Domain.Entities.Tracking;
using MarketLinker.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;

namespace MarketLinker.Infrastructure.Data;

public class MarketLinkerDbContext(DbContextOptions<MarketLinkerDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<ProductMarketplace> ProductMarketplaces { get; set; } = null!;
    public DbSet<Marketplace> Marketplaces { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;
    public DbSet<ShipmentTracking> ShipmentTrackings { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<MarketplaceAuthBase>()
            .HasOne(ma => ma.User)
            .WithMany(u => u.MarketplaceAuths)
            .HasForeignKey(ma => ma.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Order>()
            .HasOne(o => o.ShipmentTracking)
            .WithOne(st => st.Order)
            .HasForeignKey<ShipmentTracking>(st => st.OrderId);
            
        modelBuilder.Entity<MarketplaceAuthBase>()
            .HasDiscriminator<MarketplaceType>("MarketplaceType")
            .HasValue<MercadoLivreAuth>(MarketplaceType.MercadoLivre)
            .HasValue<ShopeeAuth>(MarketplaceType.Shopee);
    }
}