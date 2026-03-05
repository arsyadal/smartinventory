using Microsoft.EntityFrameworkCore;
using SmartInventory.Application.Interfaces.Repositories;
using SmartInventory.Domain.Entities;
using SmartInventory.Infrastructure.Persistence;

namespace SmartInventory.Infrastructure.Repositories;

public class StockMutationRepository : Repository<StockMutation>, IStockMutationRepository
{
    public StockMutationRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<StockMutation>> GetByProductIdAsync(int productId) =>
        await _dbSet.Where(m => m.ProductId == productId)
                    .OrderByDescending(m => m.CreatedAt)
                    .ToListAsync();

    public async Task<IEnumerable<StockMutation>> GetByDateRangeAsync(DateTime start, DateTime end) =>
        await _dbSet.Where(m => m.CreatedAt >= start && m.CreatedAt <= end)
                    .OrderByDescending(m => m.CreatedAt)
                    .ToListAsync();
}
