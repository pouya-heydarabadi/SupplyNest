using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupplyNest.Warehouse.Api.Domain.WarehouseReceipts.Entities;

namespace SupplyNest.Warehouse.Api.Infrastructure.SqlServerConfigs.EntityConfigurations.Warehouses;

public class WarehouseConfiguration : IEntityTypeConfiguration<Domain.Warehouses.Warehouse>
{
    public void Configure(EntityTypeBuilder<Domain.Warehouses.Warehouse> builder)
    {
        builder.ToTable("Warehouses");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Address)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(x => x.Email)
            .HasMaxLength(100);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.Capacity)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(x => x.CurrentOccupancy)
            .IsRequired()
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt);

        builder.Property(x => x.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Relationships
        builder.HasMany<WarehouseReceipt>()
            .WithOne()
            .HasForeignKey("WarehouseId")
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.HasIndex(x => x.Name);

        builder.HasIndex(x => x.IsActive);

        builder.HasIndex(x => x.CreatedAt);

        // Query Filters
        builder.HasQueryFilter(x => !x.IsDeleted);

        // Check Constraints
        builder.HasCheckConstraint("CK_Warehouses_Capacity", "Capacity > 0");
        builder.HasCheckConstraint("CK_Warehouses_CurrentOccupancy", "CurrentOccupancy >= 0");
        builder.HasCheckConstraint("CK_Warehouses_Occupancy", "CurrentOccupancy <= Capacity");
    }
} 