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
        builder.Property(x => x.CurrentInventoryQuantity).IsRequired();
        builder.Property(x => x.CurrentSaleQuantity).IsRequired();
        builder.HasKey(x =>new
        {
            x.WarehouseId, x.ProductId, x.FiscalYearId
        });
        
        
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