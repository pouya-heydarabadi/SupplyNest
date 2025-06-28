namespace BuildingBlocks.Messaging.Kafka.Configuration;

public sealed class KafkaOption
{
    public string BootstrapServers { get; set; } = "localhost:9092";
    public string Topic { get; set; } = "my-topic";
    public string ConsumerGroup { get; set; } = "my-consumer-group";
}