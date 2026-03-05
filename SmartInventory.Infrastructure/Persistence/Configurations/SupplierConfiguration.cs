using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartInventory.Domain.Entities;

namespace SmartInventory.Infrastructure.Persistence.Configurations;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name).HasMaxLength(200).IsRequired();
        builder.Property(s => s.Contact).HasMaxLength(200);
        builder.Property(s => s.Address).HasMaxLength(500);

        builder.HasMany(s => s.PurchaseOrders)
               .WithOne(po => po.Supplier)
               .HasForeignKey(po => po.SupplierId);
    }
}
