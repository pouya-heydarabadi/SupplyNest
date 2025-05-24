using SupplyNest.Warehouse.Api.Domain.Common;
using SupplyNest.Warehouse.Api.Domain.WarehouseReceipts.Enums;

namespace SupplyNest.Warehouse.Api.Domain.WarehouseReceipts.Entities;

public sealed class WarehouseReceipt : BaseEntity
{
    public string ReceiptNumber { get; private set; }
    
    public Guid SupplierId { get; private set; }
    public string SupplierName { get; private set; }
    
    public Guid CreatorId { get; private set; }
    public string CreatorName { get; private set; }
    
    public Guid PurchaseOrderId { get; private set; }
    
    public DateTime ReceiptDate { get; private set; }
    public string Description { get; private set; }
    public ReceiptStatusEnum Status { get; private set; }

    public Guid ReceiverId { get; private set; }
    public string ReceiverName { get; private set; }
    
    public Guid ApproverId { get; private set; }
    public string ApproverName { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    
    public List<WarehouseReceiptItem> Items { get; private set; }

    private WarehouseReceipt() 
    {
        Items = new List<WarehouseReceiptItem>();
    }

    public static WarehouseReceipt Create(
        string receiptNumber,
        Guid supplierId,
        string supplierName,
        Guid creatorId,
        string creatorName,
        Guid purchaseOrderId,
        string description)
    {
        if (string.IsNullOrWhiteSpace(receiptNumber))
            throw new ArgumentException("Receipt number cannot be empty", nameof(receiptNumber));

        if (supplierId == Guid.Empty)
            throw new ArgumentException("Supplier ID cannot be empty", nameof(supplierId));

        if (string.IsNullOrWhiteSpace(supplierName))
            throw new ArgumentException("Supplier name cannot be empty", nameof(supplierName));

        if (creatorId == Guid.Empty)
            throw new ArgumentException("Creator ID cannot be empty", nameof(creatorId));

        if (string.IsNullOrWhiteSpace(creatorName))
            throw new ArgumentException("Creator name cannot be empty", nameof(creatorName));

        if (purchaseOrderId == Guid.Empty)
            throw new ArgumentException("Purchase Order ID cannot be empty", nameof(purchaseOrderId));
        
        return new WarehouseReceipt
        {
            ReceiptNumber = receiptNumber,
            SupplierId = supplierId,
            SupplierName = supplierName,
            CreatorId = creatorId,
            CreatorName = creatorName,
            PurchaseOrderId = purchaseOrderId,
            Description = description,
            ReceiptDate = DateTime.Now,
            Status = ReceiptStatusEnum.Draft,
            Items = new List<WarehouseReceiptItem>()
        };
    }

    public void AddItem(
        Guid inventoryId,
        Guid warehouseId,
        string warehouseName,
        string productCode,
        string productName,
        decimal quantity,
        string unit,
        string notes = null)
    {
        if (Status != ReceiptStatusEnum.Draft)
            throw new InvalidOperationException("Can only add items to a draft receipt");

        if (inventoryId == Guid.Empty)
            throw new ArgumentException("Inventory ID cannot be empty", nameof(inventoryId));

        if (warehouseId == Guid.Empty)
            throw new ArgumentException("Warehouse ID cannot be empty", nameof(warehouseId));

        if (string.IsNullOrWhiteSpace(warehouseName))
            throw new ArgumentException("Warehouse name cannot be empty", nameof(warehouseName));

        if (string.IsNullOrWhiteSpace(productCode))
            throw new ArgumentException("Product code cannot be empty", nameof(productCode));

        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Product name cannot be empty", nameof(productName));

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

        if (string.IsNullOrWhiteSpace(unit))
            throw new ArgumentException("Unit cannot be empty", nameof(unit));

        var item = new WarehouseReceiptItem(
            inventoryId,
            warehouseId,
            warehouseName,
            productCode,
            productName,
            quantity,
            unit,
            notes);

        Items.Add(item);
        Update();
    }

    public void RemoveItem(Guid itemId)
    {
        if (Status != ReceiptStatusEnum.Draft)
            throw new InvalidOperationException("Can only remove items from a draft receipt");

        var item = Items.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
            throw new ArgumentException("Item not found", nameof(itemId));

        Items.Remove(item);
        Update();
    }

    public void Submit(Guid receiverId, string receiverName)
    {
        if (Status != ReceiptStatusEnum.Draft)
            throw new InvalidOperationException("Can only submit a draft receipt");

        if (!Items.Any())
            throw new InvalidOperationException("Cannot submit a receipt without items");

        if (receiverId == Guid.Empty)
            throw new ArgumentException("Receiver ID cannot be empty", nameof(receiverId));

        if (string.IsNullOrWhiteSpace(receiverName))
            throw new ArgumentException("Receiver name cannot be empty", nameof(receiverName));

        Status = ReceiptStatusEnum.Submitted;
        ReceiverId = receiverId;
        ReceiverName = receiverName;
        Update();
    }

    public void Approve(Guid approverId, string approverName)
    {
        if (Status != ReceiptStatusEnum.Submitted)
            throw new InvalidOperationException("Can only approve a submitted receipt");

        if (approverId == Guid.Empty)
            throw new ArgumentException("Approver ID cannot be empty", nameof(approverId));

        if (string.IsNullOrWhiteSpace(approverName))
            throw new ArgumentException("Approver name cannot be empty", nameof(approverName));

        Status = ReceiptStatusEnum.Approved;
        ApproverId = approverId;
        ApproverName = approverName;
        ApprovedAt = DateTime.Now;
        Update();
    }

    public void Reject(Guid approverId, string approverName, string reason)
    {
        if (Status != ReceiptStatusEnum.Submitted)
            throw new InvalidOperationException("Can only reject a submitted receipt");

        if (approverId == Guid.Empty)
            throw new ArgumentException("Approver ID cannot be empty", nameof(approverId));

        if (string.IsNullOrWhiteSpace(approverName))
            throw new ArgumentException("Approver name cannot be empty", nameof(approverName));

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Rejection reason cannot be empty", nameof(reason));

        Status = ReceiptStatusEnum.Rejected;
        ApproverId = approverId;
        ApproverName = approverName;
        Description = $"{Description}\nRejection Reason: {reason}";
        Update();
    }

    public void Cancel(string reason)
    {
        if (Status == ReceiptStatusEnum.Approved)
            throw new InvalidOperationException("Cannot cancel an approved receipt");

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Cancellation reason cannot be empty", nameof(reason));

        Status = ReceiptStatusEnum.Cancelled;
        Description = $"{Description}\nCancellation Reason: {reason}";
        Update();
    }
}