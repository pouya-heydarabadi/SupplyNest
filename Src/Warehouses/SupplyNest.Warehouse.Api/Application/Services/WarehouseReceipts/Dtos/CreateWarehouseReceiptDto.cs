namespace SupplyNest.Warehouse.Api.Application.Services.WarehouseReceipts.Dtos;

public class CreateWarehouseReceiptDto
    {
        public string ReceiptNumber { get; set; }
        public Guid SupplierId { get; set; }
        public string SupplierName { get; set; }
        public Guid CreatorId { get; set; }
        public string CreatorName { get; set; }
        public Guid PurchaseOrderId { get; set; }
        public string Description { get; set; }

        public List<WarehouseReceiptItemDto> Items { get; set; }
    }
