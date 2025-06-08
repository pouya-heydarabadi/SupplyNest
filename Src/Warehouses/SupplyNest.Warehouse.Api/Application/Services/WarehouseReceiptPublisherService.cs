using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using SupplyNest.SagaOrchestrator.Contracts;

namespace SupplyNest.Warehouse.Api.Application.Services
{
    public interface IWarehouseReceiptPublisherService
    {
        Task FinalizeReceiptAndPublishEvent(Guid receiptId, Guid productId, int quantity, DateTime processingTimestamp);
    }

    public class WarehouseReceiptPublisherService : IWarehouseReceiptPublisherService
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<WarehouseReceiptPublisherService> _logger;

        public WarehouseReceiptPublisherService(IPublishEndpoint publishEndpoint, ILogger<WarehouseReceiptPublisherService> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task FinalizeReceiptAndPublishEvent(Guid receiptId, Guid productId, int quantity, DateTime processingTimestamp)
        {
            // This method would be called after a warehouse receipt is successfully processed by other internal logic.
            _logger.LogInformation("Warehouse receipt {ReceiptId} processed. Publishing WarehouseReceiptProcessed event.", receiptId);

            var correlationId = NewId.NextGuid(); // Warehouse service initiates the saga with a new CorrelationId

            var WRPevent = new WarehouseReceiptProcessed(
                correlationId,
                receiptId,
                productId,
                quantity,
                processingTimestamp
            );

            await _publishEndpoint.Publish(WRPevent);

            _logger.LogInformation("WarehouseReceiptProcessed event published with CorrelationId: {CorrelationId}, ReceiptId: {ReceiptId}",
                correlationId, receiptId);
        }
    }
}
