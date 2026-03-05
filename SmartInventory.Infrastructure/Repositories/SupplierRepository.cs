using Microsoft.EntityFrameworkCore;
using SmartInventory.Application.Interfaces.Repositories;
using SmartInventory.Domain.Entities;
using SmartInventory.Infrastructure.Persistence;

namespace SmartInventory.Infrastructure.Repositories;

public class SupplierRepository : Repository<Supplier>, ISupplierRepository
{
    public SupplierRepository(AppDbContext context) : base(context) { }

    public override async Task<IEnumerable<Supplier>> GetAllAsync() =>
        await _dbSet.Where(s => s.IsActive).ToListAsync();
}
