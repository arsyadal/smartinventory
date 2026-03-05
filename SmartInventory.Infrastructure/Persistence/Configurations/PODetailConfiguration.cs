using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartInventory.Domain.Entities;

namespace SmartInventory.Infrastructure.Persistence.Configurations;

public class PODetailConfiguration : IEntityTypeConfiguration<PODetail>
{
    public void Configure(EntityTypeBuilder<PODetail> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.UnitPrice).HasColumnType("decimal(18,2)");
    }
}
