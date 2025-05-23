using Carter;
using RedLockNet;
using RedLockNet.SERedis;
using SupplyNest.Inventory.Api.Application.Interfaces;

namespace SupplyNest.Inventory.Api.Api.Inventories.Endpoints;

public sealed class UpdateInventoryQuantity:ICarterModule
{

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/Api/Inventory", async (Guid id, int quantity, IInventoryRepository repository, IDistributedLockFactory _lockFactory,
            CancellationToken cancellationToken) =>
        {
            var resource = $"Inventory-Update:{id}";
            var expiry = TimeSpan.FromSeconds(5);         // قفل کوتاه‌تر
            var wait = TimeSpan.FromSeconds(5);           // نهایتاً ۵ ثانیه صبر کن
            var retry = TimeSpan.FromMilliseconds(50);   // تلاش سریع‌تر برای گرفتن قفل


            using (var redLock = await _lockFactory.CreateLockAsync(resource, expiry, wait, retry))
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