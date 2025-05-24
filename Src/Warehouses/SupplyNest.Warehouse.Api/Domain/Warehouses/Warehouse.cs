using SupplyNest.Warehouse.Api.Domain.Common;

namespace SupplyNest.Warehouse.Api.Domain.Warehouses;

public sealed class Warehouse : BaseEntity
{
    public string Name { get; private set; }
    public string Code { get; private set; }
    public string Address { get; private set; }
    public string PhoneNumber { get; private set; }
    public string Email { get; private set; }
    public bool IsActive { get; private set; }
    public decimal Capacity { get; private set; }
    public decimal CurrentOccupancy { get; private set; }

    private Warehouse() { } // For EF Core

    public static Warehouse Create(
        string name,
        string code,
        string address,
        string phoneNumber,
        string email,
        decimal capacity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Warehouse name cannot be empty", nameof(name));
        
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Warehouse code cannot be empty", nameof(code));

        if (capacity <= 0)
            throw new ArgumentException("Capacity must be greater than zero", nameof(capacity));

        return new Warehouse
        {
            Name = name,
            Code = code,
            Address = address,
            PhoneNumber = phoneNumber,
            Email = email,
            Capacity = capacity,
            CurrentOccupancy = 0,
            IsActive = true
        };
    }

    public void UpdateDetails(
        string name,
        string address,
        string phoneNumber,
        string email)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Warehouse name cannot be empty", nameof(name));

        Name = name;
        Address = address;
        PhoneNumber = phoneNumber;
        Email = email;
        
        Update();
    }

    public void UpdateCapacity(decimal newCapacity)
    {
        if (newCapacity <= 0)
            throw new ArgumentException("Capacity must be greater than zero", nameof(newCapacity));

        if (newCapacity < CurrentOccupancy)
            throw new InvalidOperationException("New capacity cannot be less than current occupancy");

        Capacity = newCapacity;
        Update();
    }

    public void UpdateOccupancy(decimal newOccupancy)
    {
        if (newOccupancy < 0)
            throw new ArgumentException("Occupancy cannot be negative", nameof(newOccupancy));

        if (newOccupancy > Capacity)
            throw new InvalidOperationException("Occupancy cannot exceed capacity");

        CurrentOccupancy = newOccupancy;
        Update();
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new InvalidOperationException("Warehouse is already deactivated");

        IsActive = false;
        Update();
    }

    public void Activate()
    {
        if (IsActive)
            throw new InvalidOperationException("Warehouse is already active");

        IsActive = true;
        Update();
    }

    public decimal GetAvailableCapacity()
    {
        return Capacity - CurrentOccupancy;
    }
}