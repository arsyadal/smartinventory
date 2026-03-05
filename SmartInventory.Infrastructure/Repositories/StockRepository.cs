using Microsoft.EntityFrameworkCore;
using SmartInventory.Application.DTOs.Stocks;
using SmartInventory.Application.Interfaces.Repositories;
using SmartInventory.Domain.Entities;
using SmartInventory.Infrastructure.Persistence;

namespace SmartInventory.Infrastructure.Repositories;

public class StockRepository : Repository<Stock>, IStockRepository
{
    public StockRepository(AppDbContext context) : base(context) { }

    public async Task<Stock?> GetByProductIdAsync(int productId) =>
        await _dbSet.FirstOrDefaultAsync(s => s.ProductId == productId);

    public async Task<IEnumerable<LowStockDto>> GetLowStockAsync()
    {
        return await _context.Database
            .SqlQueryRaw<LowStockDto>("EXEC sp_GetLowStockProducts")
            .ToListAsync();
    }
}
