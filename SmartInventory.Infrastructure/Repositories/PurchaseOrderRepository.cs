using Microsoft.EntityFrameworkCore;
using SmartInventory.Application.Interfaces.Repositories;
using SmartInventory.Domain.Entities;
using SmartInventory.Domain.Enums;
using SmartInventory.Infrastructure.Persistence;

namespace SmartInventory.Infrastructure.Repositories;

public class PurchaseOrderRepository : Repository<PurchaseOrder>, IPurchaseOrderRepository
{
    public PurchaseOrderRepository(AppDbContext context) : base(context) { }

    public async Task<PurchaseOrder?> GetWithDetailsAsync(int id) =>
        await _dbSet.Include(po => po.Details)
                    .FirstOrDefaultAsync(po => po.Id == id);

    public async Task<IEnumerable<PurchaseOrder>> GetActiveAsync() =>
        await _dbSet.Where(po =>
                po.Status == PurchaseOrderStatus.Draft ||
                po.Status == PurchaseOrderStatus.Confirmed)
            .ToListAsync();
}
