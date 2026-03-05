using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartInventory.Domain.Entities;

namespace SmartInventory.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).HasMaxLength(200).IsRequired();
        builder.Property(p => p.SKU).HasMaxLength(50).IsRequired();
        builder.Property(p => p.Category).HasMaxLength(100).IsRequired();
        builder.Property(p => p.Price).HasColumnType("decimal(18,2)");

        builder.HasIndex(p => p.SKU)
               .IsUnique()
               .HasFilter("[IsActive] = 1");

        builder.HasOne(p => p.Stock)
               .WithOne(s => s.Product)
               .HasForeignKey<Stock>(s => s.ProductId);

        builder.HasMany(p => p.StockMutations)
               .WithOne(m => m.Product)
               .HasForeignKey(m => m.ProductId);

        builder.HasMany(p => p.PODetails)
               .WithOne(d => d.Product)
               .HasForeignKey(d => d.ProductId);
    }
}
