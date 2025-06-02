using DispatchR.Requests.Send;
using SupplyNest.Warehouse.Api.Application.Dtos;
using SupplyNest.Warehouse.Api.Application.Interfaces;
using SupplyNest.Warehouse.Api.Domain.WarehouseReceipts.Entities;
using SupplyNest.Warehouse.Api.Infrastructure.SqlServerConfigs.DbContexts;
using System.Collections.Generic;

namespace SupplyNest.Warehouse.Api.Application.Services.WarehouseReceipts.Commands
{
    public sealed class CreateWarehouseReceiptCommandHandler(IInventoryService inventoryService,
    WarehouseDbContext warehouseDbContext) : IRequestHandler<CreateWarehouseReceiptCommand, ValueTask<bool>>
    {
        public async ValueTask<bool> Handle(CreateWarehouseReceiptCommand request, CancellationToken cancellationToken)
        {
            // Create warehouse receipt with a single allocation
            var warehouseReceipt = WarehouseReceipt.Create(
                request.CreateWarehouseReceiptDto.ReceiptNumber,
                request.CreateWarehouseReceiptDto.SupplierId,
                request.CreateWarehouseReceiptDto.SupplierName,
                request.CreateWarehouseReceiptDto.CreatorId,
                request.CreateWarehouseReceiptDto.CreatorName,
                request.CreateWarehouseReceiptDto.PurchaseOrderId,
                request.CreateWarehouseReceiptDto.Description);

            // Pre-allocate capacity for items to avoid resizing
            var items = request.CreateWarehouseReceiptDto.Items;
            var inventoryUpdates = new List<UpdateInventoryRequestDto>(items.Count);

            // Process all items in a single loop
            foreach (var item in items)
            {
                warehouseReceipt.AddItem(
                    item.InventoryId,
                    item.WarehouseId,
                    item.WarehouseName,
                    item.ProductCode,
                    item.ProductName,
                    item.Quantity,
                    item.Unit,
                    item.Notes);

                inventoryUpdates.Add(new UpdateInventoryRequestDto(item.InventoryId, (int)item.Quantity));
            }

            // Add warehouse receipt to context
            await warehouseDbContext.AddAsync(warehouseReceipt);

            // Batch process inventory updates
            var updateResults = await Task.WhenAll(inventoryUpdates.Select(update => 
                inventoryService.UpdateInventory(update, cancellationToken)));
                
            if (updateResults.Any(result => !result))
            {
                throw new Exception("Failed to update inventory for one or more items");
            }

            // Save all changes in a single transaction
            await warehouseDbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}