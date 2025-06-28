public class OrderCreatedEvent
{
    public Guid OrderId { get; set; }
    public DateTime CreatedAt { get; set; }
}