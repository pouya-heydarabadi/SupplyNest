using Grpc.Core;
using RedLockNet;
using SupplyNest.Inventory.Api.Application.Interfaces;
using SupplyNest.Inventory.Api.Presentations.Grpc.Structure;

namespace SupplyNest.Inventory.Api.Presentations.Grpc.Services;

public class InventoryUpdateGrpcService( IInventoryRepository repository, IDistributedLockFactory _lockFactory):UpdateInventory.UpdateInventoryBase
{
    public override async Task<UpdateInventoryResponse> UpdateInventory(UpdateInventoryRequest request, ServerCallContext context)
    {
        var resource = $"Inventory-Update:{request.InventoryId}";
        var expiry = TimeSpan.FromSeconds(5);         // قفل کوتاه‌تر
        var wait = TimeSpan.FromSeconds(5);           // نهایتاً ۵ ثانیه صبر کن
        var retry = TimeSpan.FromMilliseconds(50);   // تلاش سریع‌تر برای گرفتن قفل

        Guid id=Guid.Parse(request.InventoryId);
        try
        {
            using (var redLock = await _lockFactory.CreateLockAsync(resource, expiry, wait, retry))
            {
                if (redLock.IsAcquired)
                {
                    var inventory = await repository.GetByIdAsync(id);
                    if (inventory is null)
                        throw new RpcException(new Status(StatusCode.NotFound, "Inventory not found"));
                    
                    inventory.UpdateInventoryQuantity(request.ChangeQuantity);
                    inventory.UpdateSaleQuantity(request.ChangeQuantity);

                    await repository.UpdateAsync(inventory);
                    return new UpdateInventoryResponse
                    {
                        Result = true
                    };
                }
        }
        
        
        return new UpdateInventoryResponse
        {
            Result = false
        };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}