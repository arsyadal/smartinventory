using SmartInventory.Domain.Entities;

namespace SmartInventory.Application.Interfaces.Repositories;

public interface IPriceHistoryRepository : IRepository<PriceHistory>
{
    Task<IEnumerable<PriceHistory>> GetByKeywordAsync(string keyword);
    Task AddRangeAsync(IEnumerable<PriceHistory> records);
}
