 using System;
using System.Collections.Generic;
using SupplyNest.Inventory.Api.Domain.Events;
using SupplyNest.Inventory.Api.Domain.Events.Base;
using SupplyNest.Inventory.Api.Domain.Exceptions;
using SupplyNest.Inventory.Api.Domain.Exceptions.Base;
using SupplyNest.Inventory.Api.Domain.ValueObjects;

namespace SupplyNest.Inventory.Api.Domain;

public sealed class Inventory
{
    private readonly List<InventoryDomainEvent> _domainEvents = new();
    
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid WarehouseId { get; private set; }
    public Guid FiscalYearId { get; private set; }
    
    public InventoryQuantity CurrentSaleQuantity { get; private set; }
    public InventoryQuantity CurrentInventoryQuantity { get; private set; }
    
    public DateTime CreatedAtTime { get; private set; }
    public DateTime? LastModifiedTime { get; private set; }

    public IReadOnlyCollection<InventoryDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    // For EF Core
    private Inventory() 
    {
        CurrentSaleQuantity = InventoryQuantity.FromInt(0);
        CurrentInventoryQuantity = InventoryQuantity.FromInt(0);
    }

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
            CreatedAtTime = DateTime.UtcNow
        };

        inventory.AddDomainEvent(new InventoryCreatedEvent(inventory.Id, productId, warehouseId, initialQuantity));
        return inventory;
    }

    public void UpdateInventory(int newQuantity)
    {
        if (newQuantity < 0)
            throw new DomainException("Inventory quantity cannot be negative");

        var oldQuantity = CurrentInventoryQuantity;
        CurrentInventoryQuantity = InventoryQuantity.FromInt(newQuantity);
        LastModifiedTime = DateTime.UtcNow;

        AddDomainEvent(new InventoryQuantityUpdatedEvent(Id, oldQuantity.Value, newQuantity));
    }

    public void UpdateSales(int newSaleQuantity)
    {
        if (newSaleQuantity < 0)
            throw new DomainException("Sale quantity cannot be negative");

        if (newSaleQuantity > CurrentInventoryQuantity.Value)
            throw new DomainException("Sale quantity cannot exceed current inventory");

        var oldQuantity = CurrentSaleQuantity;
        CurrentSaleQuantity = InventoryQuantity.FromInt(newSaleQuantity);
        LastModifiedTime = DateTime.UtcNow;

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