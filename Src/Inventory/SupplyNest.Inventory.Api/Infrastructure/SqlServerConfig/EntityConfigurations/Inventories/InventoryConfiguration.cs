using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SupplyNest.Inventory.Api.Infrastructure.SqlServerConfig.EntityConfigurations.Inventories;

public class InventoryConfiguration:IEntityTypeConfiguration<Domain.Entities.Inventory>
{

    public void Configure(EntityTypeBuilder<Domain.Entities.Inventory> builder)
    {
        builder.ToTable("Inventories");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.HasIndex(x =>new
        {
            x.WarehouseId, x.ProductId, x.FiscalYearId
        }).IsUnique();

        builder.HasIndex(x => x.FiscalYearId);
        builder.HasIndex(x => x.WarehouseId);
        builder.HasIndex(x => x.ProductId);

        
        builder.ComplexProperty(x => x.CurrentInventoryQuantity, config =>
        {
            config.Property(q => q.Value).HasColumnName(("CurrentInventoryQuantity"));
        });
        builder.ComplexProperty(x => x.CurrentSaleQuantity, config =>
        {
            config.Property(q => q.Value).HasColumnName(("CurrentSaleQuantity"));
        });
    }
}