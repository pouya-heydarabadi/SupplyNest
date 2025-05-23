namespace SupplyNest.Inventory.Api.Domain.Events.Base
{
    public abstract record InventoryDomainEvent
    {
        public Guid InventoryId { get; }
        public DateTime OccurredOn { get; }

        protected InventoryDomainEvent(Guid inventoryId)
        {
            InventoryId = inventoryId;
            OccurredOn = DateTime.UtcNow;
        }
    }
} 