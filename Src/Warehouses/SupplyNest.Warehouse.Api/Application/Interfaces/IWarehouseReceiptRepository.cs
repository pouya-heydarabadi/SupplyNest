using System;
using System.Threading;
using System.Threading.Tasks;
// Assuming a placeholder for the actual WarehouseReceipt entity
// namespace SupplyNest.Warehouse.Api.Domain.Entities { public class WarehouseReceipt { public Guid Id {get;set;} public string Status {get;set;} /* other properties */ } }

namespace SupplyNest.Warehouse.Api.Application.Interfaces
{
    // Placeholder for WarehouseReceipt entity. Replace with actual entity if available.
    public class WarehouseReceiptEntity // Placeholder
    {
        public Guid Id { get; set; }
        public string Status { get; set; } = string.Empty;
        // Add other relevant properties like ProductId, Quantity, etc.
    }

    public interface IWarehouseReceiptRepository
    {
        Task<WarehouseReceiptEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task UpdateAsync(WarehouseReceiptEntity receipt, CancellationToken cancellationToken = default);
        // Add other necessary methods like AddAsync, RemoveAsync if needed for other operations
    }
}
