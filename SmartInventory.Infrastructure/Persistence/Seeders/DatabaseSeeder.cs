using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartInventory.Domain.Entities;
using SmartInventory.Domain.Enums;

namespace SmartInventory.Infrastructure.Persistence.Seeders;

public class DatabaseSeeder
{
    private readonly AppDbContext _context;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(AppDbContext context, ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        await SeedUsersAsync();
        await SeedSuppliersAsync();
        await SeedProductsAsync();
        await SeedPriceHistoryAsync();
        await SeedPurchaseOrdersAsync();
        await ApplyStoredProceduresAsync();
        await ApplyIndexesAsync();
        _logger.LogInformation("Database seeding completed.");
    }

    private async Task SeedUsersAsync()
    {
        if (await _context.Users.AnyAsync()) return;

        _context.Users.AddRange(
            new User { Username = "admin", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@2026!"), Role = UserRole.Admin },
            new User { Username = "staff", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Staff@2026!"), Role = UserRole.Staff }
        );
        await _context.SaveChangesAsync();
        _logger.LogInformation("Users seeded.");
    }

    private async Task SeedSuppliersAsync()
    {
        if (await _context.Suppliers.AnyAsync()) return;

        _context.Suppliers.AddRange(
            new Supplier { Name = "PT Teknologi Maju", Contact = "081234567890", Address = "Jl. Sudirman No.1, Jakarta" },
            new Supplier { Name = "CV Digital Nusantara", Contact = "085678901234", Address = "Jl. Gatot Subroto No.5, Bandung" },
            new Supplier { Name = "UD Elektronik Sejahtera", Contact = "089012345678", Address = "Jl. Diponegoro No.12, Surabaya" }
        );
        await _context.SaveChangesAsync();
        _logger.LogInformation("Suppliers seeded.");
    }

    private async Task SeedProductsAsync()
    {
        if (await _context.Products.IgnoreQueryFilters().AnyAsync()) return;

        var products = new List<Product>
        {
            new() { Name = "Laptop Gaming Asus ROG", SKU = "LAP-001", Category = "Laptop", Price = 15_000_000, MinStock = 5, CreatedAt = DateTime.UtcNow },
            new() { Name = "Laptop Lenovo ThinkPad", SKU = "LAP-002", Category = "Laptop", Price = 12_000_000, MinStock = 3, CreatedAt = DateTime.UtcNow },
            new() { Name = "Smartphone Samsung Galaxy S24", SKU = "PHN-001", Category = "Smartphone", Price = 8_500_000, MinStock = 10, CreatedAt = DateTime.UtcNow },
            new() { Name = "Smartphone iPhone 15", SKU = "PHN-002", Category = "Smartphone", Price = 14_000_000, MinStock = 5, CreatedAt = DateTime.UtcNow },
            new() { Name = "Headphone Sony WH-1000XM5", SKU = "AUD-001", Category = "Audio", Price = 3_500_000, MinStock = 8, CreatedAt = DateTime.UtcNow },
            new() { Name = "Headphone JBL Tune 760", SKU = "AUD-002", Category = "Audio", Price = 1_200_000, MinStock = 15, CreatedAt = DateTime.UtcNow },
            new() { Name = "Keyboard Mechanical Logitech", SKU = "KBD-001", Category = "Peripherals", Price = 900_000, MinStock = 20, CreatedAt = DateTime.UtcNow },
            new() { Name = "Mouse Gaming Razer DeathAdder", SKU = "MOU-001", Category = "Peripherals", Price = 650_000, MinStock = 20, CreatedAt = DateTime.UtcNow },
            new() { Name = "Monitor LG 27\" 4K", SKU = "MON-001", Category = "Monitor", Price = 5_500_000, MinStock = 3, CreatedAt = DateTime.UtcNow },
            new() { Name = "SSD Samsung 1TB NVMe", SKU = "SSD-001", Category = "Storage", Price = 1_800_000, MinStock = 10, CreatedAt = DateTime.UtcNow }
        };

        _context.Products.AddRange(products);
        await _context.SaveChangesAsync();

        // Seed stocks — some intentionally below MinStock
        var stockQuantities = new[] { 2, 8, 4, 10, 3, 20, 25, 30, 5, 7 }; // items 0,2,4 are below min
        for (int i = 0; i < products.Count; i++)
        {
            _context.Stocks.Add(new Stock
            {
                ProductId = products[i].Id,
                Quantity = stockQuantities[i],
                LastUpdated = DateTime.UtcNow
            });
        }

        // Add some stock mutations
        _context.StockMutations.AddRange(
            new StockMutation { ProductId = products[0].Id, Type = MutationType.In, Quantity = 10, Note = "Initial stock", CreatedAt = DateTime.UtcNow.AddDays(-7) },
            new StockMutation { ProductId = products[0].Id, Type = MutationType.Out, Quantity = 8, Note = "Sale to customer", CreatedAt = DateTime.UtcNow.AddDays(-3) },
            new StockMutation { ProductId = products[2].Id, Type = MutationType.In, Quantity = 20, Note = "Initial stock", CreatedAt = DateTime.UtcNow.AddDays(-7) },
            new StockMutation { ProductId = products[2].Id, Type = MutationType.Out, Quantity = 16, Note = "Bulk order", CreatedAt = DateTime.UtcNow.AddDays(-2) },
            new StockMutation { ProductId = products[4].Id, Type = MutationType.In, Quantity = 15, Note = "Initial stock", CreatedAt = DateTime.UtcNow.AddDays(-7) },
            new StockMutation { ProductId = products[4].Id, Type = MutationType.Out, Quantity = 12, Note = "Sales", CreatedAt = DateTime.UtcNow.AddDays(-1) }
        );

        await _context.SaveChangesAsync();
        _logger.LogInformation("Products, stocks, and mutations seeded.");
    }

    private async Task SeedPriceHistoryAsync()
    {
        if (await _context.PriceHistories.AnyAsync()) return;

        var rng = new Random(42);
        var keywords = new[] { "laptop gaming", "smartphone", "headphone wireless" };
        var sources = new[] { "Tokopedia", "Shopee", "Lazada" };
        var records = new List<PriceHistory>();

        foreach (var keyword in keywords)
        {
            for (int day = 6; day >= 0; day--)
            {
                for (int j = 0; j < 3; j++)
                {
                    records.Add(new PriceHistory
                    {
                        ProductName = keyword,
                        Price = Math.Round((decimal)(rng.NextDouble() * 900_000 + 100_000), 2),
                        Source = sources[rng.Next(sources.Length)],
                        ScrapedAt = DateTime.UtcNow.AddDays(-day).AddHours(-rng.Next(0, 24))
                    });
                }
            }
        }

        _context.PriceHistories.AddRange(records);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Price history seeded ({Count} records).", records.Count);
    }

    private async Task SeedPurchaseOrdersAsync()
    {
        if (await _context.PurchaseOrders.AnyAsync()) return;

        var supplier = await _context.Suppliers.FirstOrDefaultAsync();
        var products = await _context.Products.IgnoreQueryFilters().Take(3).ToListAsync();
        if (supplier == null || products.Count == 0) return;

        // Draft PO
        var draftPo = new PurchaseOrder
        {
            SupplierId = supplier.Id,
            Status = PurchaseOrderStatus.Draft,
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            Details = new List<PODetail>
            {
                new() { ProductId = products[0].Id, Quantity = 10, UnitPrice = products[0].Price * 0.85m }
            }
        };
        draftPo.TotalAmount = draftPo.Details.Sum(d => d.Quantity * d.UnitPrice);

        // Received PO
        var receivedPo = new PurchaseOrder
        {
            SupplierId = supplier.Id,
            Status = PurchaseOrderStatus.Received,
            CreatedAt = DateTime.UtcNow.AddDays(-14),
            ReceivedAt = DateTime.UtcNow.AddDays(-10),
            Details = new List<PODetail>
            {
                new() { ProductId = products[1].Id, Quantity = 5, UnitPrice = products[1].Price * 0.85m },
                new() { ProductId = products[2].Id, Quantity = 20, UnitPrice = products[2].Price * 0.85m }
            }
        };
        receivedPo.TotalAmount = receivedPo.Details.Sum(d => d.Quantity * d.UnitPrice);

        _context.PurchaseOrders.AddRange(draftPo, receivedPo);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Purchase orders seeded.");
    }

    private async Task ApplyStoredProceduresAsync()
    {
        try
        {
            var spDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SqlScripts", "StoredProcedures");
            if (!Directory.Exists(spDir)) return;

            foreach (var file in Directory.GetFiles(spDir, "*.sql"))
            {
                var sql = await File.ReadAllTextAsync(file);
                // Split on GO keyword (SQL Server batch separator)
                var batches = sql.Split("\nGO", StringSplitOptions.RemoveEmptyEntries);
                foreach (var batch in batches)
                {
                    var trimmed = batch.Trim();
                    if (!string.IsNullOrWhiteSpace(trimmed))
                        await _context.Database.ExecuteSqlRawAsync(trimmed);
                }
            }
            _logger.LogInformation("Stored procedures applied.");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not apply stored procedures (non-fatal).");
        }
    }

    private async Task ApplyIndexesAsync()
    {
        try
        {
            var indexFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SqlScripts", "Indexes", "CreateIndexes.sql");
            if (!File.Exists(indexFile)) return;

            var sql = await File.ReadAllTextAsync(indexFile);
            // Split into individual statements
            var statements = sql.Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (var stmt in statements)
            {
                var trimmed = stmt.Trim();
                if (!string.IsNullOrWhiteSpace(trimmed))
                    await _context.Database.ExecuteSqlRawAsync(trimmed);
            }
            _logger.LogInformation("Indexes applied.");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not apply indexes (non-fatal).");
        }
    }
}
