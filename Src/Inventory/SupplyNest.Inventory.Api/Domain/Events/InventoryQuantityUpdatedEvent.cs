using System;
using SupplyNest.Inventory.Api.Domain.Events.Base;

namespace SupplyNest.Inventory.Api.Domain.Events
{
    public record InventoryQuantityUpdatedEvent : InventoryDomainEvent
    {
        public long OldQuantity { get; }
        public long NewQuantity { get; }

        public InventoryQuantityUpdatedEvent(Guid inventoryId, long oldQuantity, long newQuantity)
            : base(inventoryId)
        {
            OldQuantity = oldQuantity;
            NewQuantity = newQuantity;
        }
    }
} 