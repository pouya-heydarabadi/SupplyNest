namespace SupplyNest.Inventory.Api.Application.Interfaces;

public interface IInventoryRepository
{
    Task<Domain.Entities.Inventory> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Domain.Entities.Inventory> GetByProductAndWarehouseAsync(Guid productId, Guid warehouseId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Domain.Entities.Inventory>> GetByWarehouseAsync(Guid warehouseId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Domain.Entities.Inventory>> GetByProductAsync(Guid productId, CancellationToken cancellationToken = default);
    Task AddAsync(Domain.Entities.Inventory inventory, CancellationToken cancellationToken = default);
    Task UpdateAsync(Domain.Entities.Inventory inventory, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid warehouseId, Guid productId, Guid fiscalYearId, CancellationToken cancellationToken = default);
}
