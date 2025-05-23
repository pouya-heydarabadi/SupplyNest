using SupplyNest.Inventory.Api.Domain.Exceptions.Base;

namespace SupplyNest.Inventory.Api.Domain.Exceptions
{
    public class InventoryCannotBeNegativeException : DomainException
    {
        public InventoryCannotBeNegativeException() : base("inventory cannot be negative")
        {
        }
    }
}