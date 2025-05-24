using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupplyNest.Warehouse.Api.Domain.WarehouseReceipts.Entities;

namespace SupplyNest.Warehouse.Api.Infrastructure.SqlServerConfigs.EntityConfigurations.WarehouseReceipts;

public class WarehouseReceiptConfiguration : IEntityTypeConfiguration<WarehouseReceipt>
{
    public void Configure(EntityTypeBuilder<WarehouseReceipt> builder)
    {
        builder.ToTable("WarehouseReceipts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ReceiptNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.SupplierName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.SupplierId)
            .IsRequired();

        builder.Property(x => x.CreatorId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.CreatorName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.PurchaseOrderId)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.ReceiverId)
            .HasMaxLength(50);

        builder.Property(x => x.ReceiverName)
            .HasMaxLength(200);

        builder.Property(x => x.ApproverId)
            .HasMaxLength(50);

        builder.Property(x => x.ApproverName)
            .HasMaxLength(200);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(x => x.ReceiptDate)
            .IsRequired();

        builder.Property(x => x.ApprovedAt);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt);

        builder.Property(x => x.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Relationships
        builder.HasMany(x => x.Items)
            .WithOne()
            .HasForeignKey("WarehouseReceiptId")
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(x => x.ReceiptNumber)
            .IsUnique();

        builder.HasIndex(x => x.SupplierId);

        builder.HasIndex(x => x.PurchaseOrderId);

        builder.HasIndex(x => x.Status);

        builder.HasIndex(x => x.CreatedAt);

        // Query Filters
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
} 