using Microsoft.EntityFrameworkCore;
using SmartInventory.Application.Interfaces.Repositories;
using SmartInventory.Domain.Entities;
using SmartInventory.Infrastructure.Persistence;

namespace SmartInventory.Infrastructure.Repositories;

public class PriceHistoryRepository : Repository<PriceHistory>, IPriceHistoryRepository
{
    public PriceHistoryRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<PriceHistory>> GetByKeywordAsync(string keyword) =>
        await _dbSet.Where(ph => ph.ProductName.Contains(keyword))
                    .OrderByDescending(ph => ph.ScrapedAt)
                    .ToListAsync();

    public async Task AddRangeAsync(IEnumerable<PriceHistory> records)
    {
        await _dbSet.AddRangeAsync(records);
        await _context.SaveChangesAsync();
    }
}
