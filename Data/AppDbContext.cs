using Microsoft.EntityFrameworkCore;
using WholesaleBase.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

public class AppDbContext : IdentityDbContext<IdentityUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Delivery> Deliveries { get; set; }
    public DbSet<DeliveryContent> DeliveryContents { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderContent> OrderContents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Устанавливаем точность и масштаб для поля TotalCost
        modelBuilder.Entity<Order>()
            .Property(o => o.TotalCost)
            .HasColumnType("decimal(18, 2)");
    }

  
}
