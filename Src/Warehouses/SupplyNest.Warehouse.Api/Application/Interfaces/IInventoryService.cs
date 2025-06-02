using SupplyNest.Inventory.Api.Presentations.Grpc.Structure;
using SupplyNest.Warehouse.Api.Application.Dtos;

namespace SupplyNest.Warehouse.Api.Application.Interfaces;

public interface IInventoryService
{
    public Task<bool> UpdateInventory(UpdateInventoryRequestDto request, CancellationToken cancellationToken);
}