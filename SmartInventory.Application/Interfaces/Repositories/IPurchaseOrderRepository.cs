using SmartInventory.Domain.Entities;

namespace SmartInventory.Application.Interfaces.Repositories;

public interface IPurchaseOrderRepository : IRepository<PurchaseOrder>
{
    Task<PurchaseOrder?> GetWithDetailsAsync(int id);
    Task<IEnumerable<PurchaseOrder>> GetActiveAsync();
}
