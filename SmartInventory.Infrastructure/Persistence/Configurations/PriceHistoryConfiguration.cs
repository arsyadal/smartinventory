using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartInventory.Domain.Entities;

namespace SmartInventory.Infrastructure.Persistence.Configurations;

public class PriceHistoryConfiguration : IEntityTypeConfiguration<PriceHistory>
{
    public void Configure(EntityTypeBuilder<PriceHistory> builder)
    {
        builder.HasKey(ph => ph.Id);
        builder.Property(ph => ph.ProductName).HasMaxLength(200).IsRequired();
        builder.Property(ph => ph.Price).HasColumnType("decimal(18,2)");
        builder.Property(ph => ph.Source).HasMaxLength(100);
    }
}
