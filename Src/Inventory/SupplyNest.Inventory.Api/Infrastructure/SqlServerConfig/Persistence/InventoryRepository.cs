using Microsoft.EntityFrameworkCore;
using SupplyNest.Inventory.Api.Application.Interfaces;
using SupplyNest.Inventory.Api.Infrastructure.SqlServerConfig.DbContexts;

namespace SupplyNest.Inventory.Api.Infrastructure.SqlServerConfig.Persistence;

public class InventoryRepository(InventoryDbContext _context) : IInventoryRepository
{
    
    public async Task<Domain.Entities.Inventory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Inventories
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Domain.Entities.Inventory?> GetByProductAndWarehouseAsync(Guid productId, Guid warehouseId, CancellationToken cancellationToken = default)
    {
        return await _context.Inventories
            .FirstOrDefaultAsync(x => x.ProductId == productId && x.WarehouseId == warehouseId, cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.Inventory?>> GetByWarehouseAsync(Guid warehouseId, CancellationToken cancellationToken = default)
    {
        return await _context.Inventories
            .Where(x => x.WarehouseId == warehouseId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.Inventory?>> GetByProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _context.Inventories
            .Where(x => x.ProductId == productId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Domain.Entities.Inventory> AddAsync(Domain.Entities.Inventory? inventory, CancellationToken cancellationToken = default)
    {
        await _context.Inventories.AddAsync(inventory, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return inventory;
    }

    public async Task UpdateAsync(Domain.Entities.Inventory? inventory, CancellationToken cancellationToken = default)
    {
        if (inventory is not null)
        {
            _context.Inventories.Update(inventory!);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var inventory = await _context.Inventories.FindAsync(new object[] { id }, cancellationToken);
        if (inventory != null)
        {
            _context.Inventories.Remove(inventory);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Inventories
            .AnyAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid warehouseId, Guid productId, Guid fiscalYearId, CancellationToken cancellationToken = default)
    {
        return await _context.Inventories
            .AnyAsync(x => 
                x.WarehouseId == warehouseId && 
                x.ProductId == productId && 
                x.FiscalYearId == fiscalYearId, 
                cancellationToken);
    }
}