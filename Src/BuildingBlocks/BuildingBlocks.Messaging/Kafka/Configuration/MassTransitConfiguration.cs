// using System.Security.Cryptography.X509Certificates;
// using MassTransit;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.DependencyInjection;
//
// namespace BuildingBlocks.Messaging.Kafka.Configuration;
//
// public static class MassTransitConfiguration
// {
//     public static IServiceCollection AddMassTransitService(this IServiceCollection services, KafkaOption kafkaOption)
//     {
//         services.AddMassTransit(x =>
//         {
//             x.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
//             x.AddRider(rider =>
//             {
//                 // Existing producer and consumer
//                 rider.AddProducer<TestMessage>(kafkaOption.Topic);
//                 rider.AddConsumer<TestMessageConsumer>();
//
//                 // New producer and consumer
//                 rider.AddProducer<OrderCreatedEvent>(kafkaOption.Topic);
//                 rider.AddConsumer<OrderCreatedEventConsumer>();
//
//                 rider.UsingKafka((context, config) =>
//                 {
//                     config.Host(kafkaOption.BootstrapServers);
//                     
//                     // Existing topic endpoint
//                     config.TopicEndpoint<KafkaOption>(kafkaOption.Topic, kafkaOption.ConsumerGroup,
//                         e =>
//                         {
//                             e.ConfigureConsumer<TestMessageConsumer>();
//                             e.ConfigureConsumer<OrderCreatedEventConsumer>();
//                         });
//                 });
//             });
//         });
//
//         return services;
//     }
// }