using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupplyNest.Warehouse.Api.Domain.WarehouseReceipts.Entities;

namespace SupplyNest.Warehouse.Api.Infrastructure.SqlServerConfigs.EntityConfigurations.WarehouseReceipts;

public class WarehouseReceiptItemConfiguration : IEntityTypeConfiguration<WarehouseReceiptItem>
{
    public void Configure(EntityTypeBuilder<WarehouseReceiptItem> builder)
    {
        builder.ToTable("WarehouseReceiptItems");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.InventoryId)
            .IsRequired();

        builder.Property(x => x.WarehouseId)
            .IsRequired();

        builder.Property(x => x.WarehouseName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.ProductCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.ProductName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Quantity)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(x => x.Unit)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt);

        builder.Property(x => x.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Indexes
        builder.HasIndex(x => x.InventoryId);

        builder.HasIndex(x => x.WarehouseId);

        builder.HasIndex(x => x.ProductCode);

        // Query Filters
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
} 