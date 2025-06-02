using Grpc.Core;
using Grpc.Net.Client;
using SupplyNest.Inventory.Api.Presentations.Grpc.Structure;
using SupplyNest.Warehouse.Api.Application.Dtos;
using SupplyNest.Warehouse.Api.Application.Interfaces;

namespace SupplyNest.Warehouse.Api.Infrastructure.Services;

public sealed class InventoryService(IConsulService consulService):IInventoryService
{

    public async Task<bool> UpdateInventory(UpdateInventoryRequestDto request, CancellationToken cancellationToken)
    {
        var url = await consulService.GetServiceUrl("Inventory-Service");
        
        using var channel = GrpcChannel.ForAddress(url);
        var client = new UpdateInventory.UpdateInventoryClient(channel);
        
        UpdateInventoryRequest requestGrpcDto = (new UpdateInventoryRequest
        {
            InventoryId = request.inventoryId.ToString(),
            ChangeQuantity = request.quantity
        });

        await client.UpdateInventoryAsync(request: requestGrpcDto, cancellationToken: cancellationToken);

        return true;
    }
}