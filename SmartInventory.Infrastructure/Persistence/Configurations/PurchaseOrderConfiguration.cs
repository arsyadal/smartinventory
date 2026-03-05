using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartInventory.Domain.Entities;

namespace SmartInventory.Infrastructure.Persistence.Configurations;

public class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.HasKey(po => po.Id);
        builder.Property(po => po.TotalAmount).HasColumnType("decimal(18,2)");
        builder.Property(po => po.Status).HasConversion<string>();

        builder.HasMany(po => po.Details)
               .WithOne(d => d.PurchaseOrder)
               .HasForeignKey(d => d.POId);
    }
}
