using BuldingBlocks.Application.ConsuleInterfaces;
using Grpc.Net.Client;
using SupplyNest.Warehouse.Api.Application.Dtos;
using SupplyNest.Warehouse.Api.Application.Interfaces;
using SupplyNest.Warehouse.Api.Presentations.Grpc.Structure.Client;

namespace SupplyNest.Warehouse.Api.Infrastructure.Services;

public sealed class InventoryService(IConsulService consulService):IInventoryService
{

    public async Task<bool> UpdateInventory(UpdateInventoryRequestDto request, CancellationToken cancellationToken)
    {
        var url = await consulService.GetServiceUrl("Inventory-Service");
        
        using var channel = GrpcChannel.ForAddress(url);
        var client = new UpdateInventoryFromWarehouseService.UpdateInventoryFromWarehouseServiceClient(channel);
        
        UpdateInventoryFromWarehouseServiceRequest requestGrpcDto = (new UpdateInventoryFromWarehouseServiceRequest
        {
            InventoryId = request.inventoryId.ToString(),
            ChangeQuantity = request.quantity
        });
        
        await client.UpdateInventoryAsync(requestGrpcDto, cancellationToken: cancellationToken);

        return true;
    }
}