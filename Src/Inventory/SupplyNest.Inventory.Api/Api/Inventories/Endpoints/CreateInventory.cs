using Carter;
using SupplyNest.Inventory.Api.Application.Interfaces;

namespace SupplyNest.Inventory.Api.Api.Inventories.Endpoints;

public sealed class CreateInventory:ICarterModule
{

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("Api/Inventory", 
            async (IInventoryRepository inventoryRepository, (Guid warehouseId, Guid productId,
                    Guid fiscalYearId) request ) =>
            {
                // var findDuplicate = await inventoryRepository.ExistsAsync();
                return Results.Ok();
            });
    }
}