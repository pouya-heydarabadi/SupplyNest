using MassTransit;

public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
{
    public Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var message = context.Message;
        // Handle the order created event
        return Task.CompletedTask;
    }
}