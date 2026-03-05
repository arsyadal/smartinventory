using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartInventory.Domain.Entities;

namespace SmartInventory.Infrastructure.Persistence.Configurations;

public class StockMutationConfiguration : IEntityTypeConfiguration<StockMutation>
{
    public void Configure(EntityTypeBuilder<StockMutation> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Type).HasConversion<string>();
        builder.Property(m => m.Note).HasMaxLength(500);
    }
}
