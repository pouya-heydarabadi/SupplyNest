using DispatchR.Requests.Send;
using SupplyNest.Inventory.Api.Application.Interfaces;
using SupplyNest.Inventory.Api.Application.Services.Command.Commands;

namespace SupplyNest.Inventory.Api.Application.Services.Command.CommandHandler;

public sealed class CreateInventoryCommandHandler(IInventoryRepository inventoryRepository) : IRequestHandler<CreateInventoryCommand, ValueTask<Domain.Entities.Inventory>>
{

    public async ValueTask<Domain.Entities.Inventory> Handle(CreateInventoryCommand request, CancellationToken cancellationToken)
    {
        var findDuplicate = await inventoryRepository.ExistsAsync(request.WarehouseId,
            request.ProductId, 
            request.FiscalYearId, cancellationToken);
        
        if (findDuplicate)
            throw new Exception("Inventory already exists");
                
        Domain.Entities.Inventory inventory =
            Domain.Entities.Inventory.Create(request.ProductId , request.WarehouseId,request.FiscalYearId, 0);
                
        Domain.Entities.Inventory result = await inventoryRepository.AddAsync(inventory, cancellationToken);

        return inventory;
    }
}