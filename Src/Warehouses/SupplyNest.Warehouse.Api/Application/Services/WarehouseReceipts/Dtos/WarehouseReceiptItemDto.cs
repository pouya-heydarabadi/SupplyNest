namespace SupplyNest.Warehouse.Api.Application.Services.WarehouseReceipts.Dtos;

public class WarehouseReceiptItemDto
     {
         public Guid InventoryId { get; set; }
         public Guid WarehouseId { get; set; }
         public string WarehouseName { get; set; }
         public string ProductCode { get; set; }
         public string ProductName { get; set; }
         public decimal Quantity { get; set; }
         public string Unit { get; set; }
         public string Notes { get; set; }
     }