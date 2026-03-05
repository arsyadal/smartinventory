using SmartInventory.Application.DTOs.Stocks;
using SmartInventory.Domain.Entities;

namespace SmartInventory.Application.Interfaces.Repositories;

public interface IStockRepository : IRepository<Stock>
{
    Task<Stock?> GetByProductIdAsync(int productId);
    Task<IEnumerable<LowStockDto>> GetLowStockAsync();
}
