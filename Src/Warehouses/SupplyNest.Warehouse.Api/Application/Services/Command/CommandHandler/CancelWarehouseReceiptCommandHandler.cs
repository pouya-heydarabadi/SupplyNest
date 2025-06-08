using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using SupplyNest.SagaOrchestrator.Contracts;
using SupplyNest.Warehouse.Api.Application.Interfaces; // For IWarehouseReceiptRepository

namespace SupplyNest.Warehouse.Api.Application.Services.Command.CommandHandler
{
    public class CancelWarehouseReceiptCommandHandler : IConsumer<CancelWarehouseReceiptCommand>
    {
        private readonly IWarehouseReceiptRepository _receiptRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<CancelWarehouseReceiptCommandHandler> _logger;

        public CancelWarehouseReceiptCommandHandler(
            IWarehouseReceiptRepository receiptRepository,
            IPublishEndpoint publishEndpoint,
            ILogger<CancelWarehouseReceiptCommandHandler> logger)
        {
            _receiptRepository = receiptRepository;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CancelWarehouseReceiptCommand> context)
        {
            var command = context.Message;
            _logger.LogInformation("Received CancelWarehouseReceiptCommand for CorrelationId: {CorrelationId}, ReceiptId: {ReceiptId}, Reason: {Reason}",
                command.CorrelationId, command.ReceiptId, command.Reason);

            try
            {
                var receipt = await _receiptRepository.GetByIdAsync(command.ReceiptId, context.CancellationToken);

                if (receipt == null)
                {
                    _logger.LogWarning("Warehouse receipt with Id: {ReceiptId} not found for cancellation. CorrelationId: {CorrelationId}. Assuming already compensated or non-existent.",
                        command.ReceiptId, command.CorrelationId);
                    // Publish WarehouseReceiptCancelled to allow saga to proceed, as the desired state (cancelled) is met.
                    await _publishEndpoint.Publish(new WarehouseReceiptCancelled(command.CorrelationId, command.ReceiptId), context.CancellationToken);
                    return;
                }

                // Perform cancellation logic (e.g., update status)
                // This is a placeholder for actual cancellation logic.
                receipt.Status = "Cancelled"; // Example: updating a status property
                // Add more fields to update if necessary, e.g., cancellation reason, cancelled date.
                await _receiptRepository.UpdateAsync(receipt, context.CancellationToken);

                _logger.LogInformation("Warehouse receipt with Id: {ReceiptId} cancelled successfully. CorrelationId: {CorrelationId}",
                    command.ReceiptId, command.CorrelationId);

                await _publishEndpoint.Publish(new WarehouseReceiptCancelled(command.CorrelationId, command.ReceiptId), context.CancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling warehouse receipt for CorrelationId: {CorrelationId}, ReceiptId: {ReceiptId}. Error: {ErrorMessage}",
                    command.CorrelationId, command.ReceiptId, ex.Message);

                // Optional: Publish a WarehouseReceiptCancellationFailed event
                // await _publishEndpoint.Publish(new WarehouseReceiptCancellationFailed(command.CorrelationId, command.ReceiptId, ex.Message), context.CancellationToken);

                // Rethrow to allow MassTransit to handle retries/error queue for the compensation command.
                // Compensations are often critical, so ensuring they eventually succeed or get attention is important.
                throw;
            }
        }
    }
}
