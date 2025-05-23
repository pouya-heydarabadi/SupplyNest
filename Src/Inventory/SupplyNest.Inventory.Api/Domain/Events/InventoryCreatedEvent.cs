using System;
using SupplyNest.Inventory.Api.Domain.Events.Base;

namespace SupplyNest.Inventory.Api.Domain.Events
{
    public record InventoryCreatedEvent : InventoryDomainEvent
    {
        public Guid ProductId { get; }
        public Guid WarehouseId { get; }
        public int InitialQuantity { get; }

        public InventoryCreatedEvent(Guid inventoryId, Guid productId, Guid warehouseId, int initialQuantity)
            : base(inventoryId)
        {
            ProductId = productId;
            WarehouseId = warehouseId;
            InitialQuantity = initialQuantity;
        }
    }
} 