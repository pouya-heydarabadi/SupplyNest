using System.ComponentModel.DataAnnotations.Schema;
using SupplyNest.Inventory.Api.Domain.Events;
using SupplyNest.Inventory.Api.Domain.Events.Base;
using SupplyNest.Inventory.Api.Domain.Exceptions;
using SupplyNest.Inventory.Api.Domain.Exceptions.Base;
using SupplyNest.Inventory.Api.Domain.ValueObjects;

namespace SupplyNest.Inventory.Api.Domain.Entities
{
    public sealed class Inventory
    {
        [NotMapped]
        private readonly List<InventoryDomainEvent> _domainEvents = new();
        [NotMapped]
        public IReadOnlyCollection<InventoryDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public Guid Id { get; private set; }
        public Guid ProductId { get; private set; }
        public Guid WarehouseId { get; private set; }
        public Guid FiscalYearId { get; private set; }
    
        public InventoryQuantity CurrentSaleQuantity { get; private set; }
        public InventoryQuantity CurrentInventoryQuantity { get; private set; }
    
        public DateTime CreatedAtTime { get; private set; }
        public DateTime? LastModifiedTime { get; private set; }


        private Inventory() { } // For EF Core

        public static Inventory Create(
            Guid productId,
            Guid warehouseId,
            Guid fiscalYearId,
            int initialQuantity)
        {
            if (initialQuantity < 0)
                throw new DomainException("Initial quantity cannot be negative");

            var inventory = new Inventory
            {
                Id = Guid.NewGuid(),
                ProductId = productId,
                WarehouseId = warehouseId,
                FiscalYearId = fiscalYearId,
                CurrentInventoryQuantity = InventoryQuantity.FromInt(initialQuantity),
                CurrentSaleQuantity = InventoryQuantity.FromInt(0),
                CreatedAtTime = DateTime.Now
            };

            inventory.AddDomainEvent(new InventoryCreatedEvent(inventory.Id, productId, warehouseId, initialQuantity));
            return inventory;
        }

        public void UpdateInventoryQuantity(int newQuantity)
        {
            var oldQuantity = CurrentInventoryQuantity;
            CurrentInventoryQuantity += InventoryQuantity.FromInt(newQuantity);

            if (CurrentInventoryQuantity.Value < 0)
                throw new InventoryCannotBeNegativeException();
        
            LastModifiedTime = DateTime.Now;

            AddDomainEvent(new InventoryQuantityUpdatedEvent(Id, oldQuantity.Value, newQuantity));
        }

        public void UpdateSaleQuantity(int newSaleQuantity)
        {
            var oldQuantity = CurrentSaleQuantity;
            CurrentSaleQuantity += InventoryQuantity.FromInt(newSaleQuantity);
        
            if(CurrentSaleQuantity.Value < 0)
                throw new InventoryCannotBeNegativeException();
        
            LastModifiedTime = DateTime.Now;

            AddDomainEvent(new SalesQuantityUpdatedEvent(Id, oldQuantity.Value, newSaleQuantity));
        }

        private void AddDomainEvent(InventoryDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}