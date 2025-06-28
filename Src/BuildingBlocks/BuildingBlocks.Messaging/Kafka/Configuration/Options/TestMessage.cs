using MassTransit;

namespace BuildingBlocks.Messaging.Kafka.Configuration;

public class TestMessage
{
    public string Message { get; set; }
}

public class TestMessageConsumer:IConsumer<TestMessage>
{
    public Task Consume(ConsumeContext<TestMessage> context)
    {
        throw new NotImplementedException();
    }
}