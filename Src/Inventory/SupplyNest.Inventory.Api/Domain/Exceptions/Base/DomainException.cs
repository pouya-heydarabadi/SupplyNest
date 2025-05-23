namespace SupplyNest.Inventory.Api.Domain.Exceptions.Base
{
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message)
        {
        }

        public DomainException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
} 