using SmartInventory.Domain.Entities;

namespace SmartInventory.Application.Interfaces.Repositories;

public interface IStockMutationRepository : IRepository<StockMutation>
{
    Task<IEnumerable<StockMutation>> GetByProductIdAsync(int productId);
    Task<IEnumerable<StockMutation>> GetByDateRangeAsync(DateTime start, DateTime end);
}
