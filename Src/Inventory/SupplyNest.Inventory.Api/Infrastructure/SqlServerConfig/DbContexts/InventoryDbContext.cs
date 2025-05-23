using Microsoft.EntityFrameworkCore;

namespace SupplyNest.Inventory.Api.Infrastructure.SqlServerConfig.DbContexts;

public class InventoryDbContext:DbContext
{
    public InventoryDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InventoryDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
    
    
    public DbSet<Domain.Entities.Inventory> Inventories { get; set; }
}