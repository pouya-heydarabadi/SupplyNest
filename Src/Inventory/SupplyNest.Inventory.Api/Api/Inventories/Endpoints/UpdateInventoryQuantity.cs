using Carter;
using RedLockNet.SERedis;
using SupplyNest.Inventory.Api.Application.Interfaces;

namespace SupplyNest.Inventory.Api.Api.Inventories.Endpoints;

public sealed class UpdateInventoryQuantity:ICarterModule
{

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/Api/Inventory", async (Guid id, int quantity, IInventoryRepository repository, RedLockFactory RedLockFactory,
            CancellationToken cancellationToken) =>
        {
            const string lockObject = "inventory-update";
            await using(var redLock = await RedLockFactory.CreateLockAsync(lockObject, TimeSpan.FromSeconds(10)))
            {
                if (redLock.IsAcquired)
                {
                    var inventory = await repository.GetByIdAsync(id, cancellationToken);
                    if (inventory is null)
                        return Results.NotFound();
                    inventory.UpdateInventoryQuantity(quantity);
                    inventory.UpdateSaleQuantity(quantity);

                    await repository.UpdateAsync(inventory, cancellationToken);
                    return Results.Ok();
                }
            }
            return Results.Ok();
        });
    }
}