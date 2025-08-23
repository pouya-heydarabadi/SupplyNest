using Microsoft.EntityFrameworkCore;
using SupplyNest.Warehouse.Api.Domain.WarehouseReceipts.Entities;

namespace SupplyNest.Warehouse.Api.Infrastructure.SqlServerConfigs.DbContexts;

public sealed class WarehouseDbContext:DbContext
{

    public WarehouseDbContext(DbContextOptions options) : base(options)
    {
    }

    // Warehouse
    public DbSet<Domain.Warehouses.Warehouse> Warehouses { get; set; }
    
    // Warehouse Receipt
    public DbSet<WarehouseReceipt> WarehouseReceipts { get; set; }
    public DbSet<WarehouseReceiptItem> WarehouseReceiptItems { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WarehouseDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}