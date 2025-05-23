using System;
using SupplyNest.Inventory.Api.Domain.Events.Base;

namespace SupplyNest.Inventory.Api.Domain.Events
{
    public record InventoryQuantityUpdatedEvent : InventoryDomainEvent
    {
        public int OldQuantity { get; }
        public int NewQuantity { get; }

        public InventoryQuantityUpdatedEvent(Guid inventoryId, int oldQuantity, int newQuantity)
            : base(inventoryId)
        {
            OldQuantity = oldQuantity;
            NewQuantity = newQuantity;
        }
    }
} 