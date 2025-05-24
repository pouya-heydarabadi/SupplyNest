using SupplyNest.Warehouse.Api.Domain.Common;

namespace SupplyNest.Warehouse.Api.Domain.WarehouseReceipts.Entities;

public class WarehouseReceiptItem : BaseEntity
{
    public Guid InventoryId { get; private set; }
    public Guid WarehouseId { get; private set; }
    public string WarehouseName { get; private set; }
    public string ProductCode { get; private set; }
    public string ProductName { get; private set; }
    public decimal Quantity { get; private set; }
    public string Unit { get; private set; }
    public string Notes { get; private set; }

    private WarehouseReceiptItem() { }

    public WarehouseReceiptItem(
        Guid inventoryId,
        Guid warehouseId,
        string warehouseName,
        string productCode,
        string productName,
        decimal quantity,
        string unit,
        string notes = null)
    {
        InventoryId = inventoryId;
        WarehouseId = warehouseId;
        WarehouseName = warehouseName;
        ProductCode = productCode;
        ProductName = productName;
        Quantity = quantity;
        Unit = unit;
        Notes = notes;
    }
}