using Microsoft.EntityFrameworkCore;
using SmartInventory.Application.Interfaces.Repositories;
using SmartInventory.Domain.Entities;
using SmartInventory.Infrastructure.Persistence;

namespace SmartInventory.Infrastructure.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext context) : base(context) { }

    public async Task<Product?> GetBySkuAsync(string sku) =>
        await _dbSet.FirstOrDefaultAsync(p => p.SKU == sku);

    public async Task<bool> SkuExistsAsync(string sku, int? excludeId = null)
    {
        // Use IgnoreQueryFilters to check even against inactive products
        var query = _context.Products.IgnoreQueryFilters()
            .Where(p => p.SKU == sku && p.IsActive);
        if (excludeId.HasValue)
            query = query.Where(p => p.Id != excludeId.Value);
        return await query.AnyAsync();
    }
}
