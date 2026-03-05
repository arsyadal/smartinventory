using System.Reflection;
using Microsoft.EntityFrameworkCore;
using SmartInventory.Domain.Entities;

namespace SmartInventory.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Stock> Stocks => Set<Stock>();
    public DbSet<StockMutation> StockMutations => Set<StockMutation>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PODetail> PODetails => Set<PODetail>();
    public DbSet<PriceHistory> PriceHistories => Set<PriceHistory>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Global soft-delete filter: cascade the Product.IsActive filter to dependent entities
        // so EF Core doesn't warn about mismatched query filters on required relationships.
        modelBuilder.Entity<Product>().HasQueryFilter(p => p.IsActive);
        modelBuilder.Entity<Stock>().HasQueryFilter(s => s.Product!.IsActive);
        modelBuilder.Entity<StockMutation>().HasQueryFilter(m => m.Product!.IsActive);
        modelBuilder.Entity<PODetail>().HasQueryFilter(d => d.Product!.IsActive);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added)
            {
                if (entry.Entity is Product product && product.CreatedAt == default)
                    product.CreatedAt = DateTime.UtcNow;

                if (entry.Entity is StockMutation mutation && mutation.CreatedAt == default)
                    mutation.CreatedAt = DateTime.UtcNow;

                if (entry.Entity is PurchaseOrder po && po.CreatedAt == default)
                    po.CreatedAt = DateTime.UtcNow;

                if (entry.Entity is PriceHistory ph && ph.ScrapedAt == default)
                    ph.ScrapedAt = DateTime.UtcNow;
            }
        }
        return await base.SaveChangesAsync(cancellationToken);
    }
}
