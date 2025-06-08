using System;
using System.Threading.Tasks;
using MassTransit;
using SupplyNest.Inventory.Api.Application.Interfaces;
using SupplyNest.SagaOrchestrator.Contracts;
using Microsoft.Extensions.Logging; // Added for logging

namespace SupplyNest.Inventory.Api.Application.Services.Command.CommandHandler
{
    public class ReverseInventoryCreationCommandHandler : IConsumer<ReverseInventoryCreationCommand>
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<ReverseInventoryCreationCommandHandler> _logger;

        public ReverseInventoryCreationCommandHandler(
            IInventoryRepository inventoryRepository,
            IPublishEndpoint publishEndpoint,
            ILogger<ReverseInventoryCreationCommandHandler> logger)
        {
            _inventoryRepository = inventoryRepository;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ReverseInventoryCreationCommand> context)
        {
            var command = context.Message;
            _logger.LogInformation("Received ReverseInventoryCreationCommand for CorrelationId: {CorrelationId}, InventoryId: {InventoryId}",
                command.CorrelationId, command.InventoryId);

            try
            {
                var inventoryItem = await _inventoryRepository.GetByIdAsync(command.InventoryId, context.CancellationToken);

                if (inventoryItem == null)
                {
                    _logger.LogWarning("Inventory item with Id: {InventoryId} not found for reversal. CorrelationId: {CorrelationId}",
                        command.InventoryId, command.CorrelationId);
                    // Decide if to publish a specific "NotFound" event or just complete without error for idempotency.
                    // For now, we'll assume completing is fine, as the state is "as if" it was reversed.
                    // However, it might be better to publish a specific failure event if strict tracking is needed.
                    // For this task, we'll publish InventoryCreationReversed to indicate the desired state is achieved (or wasn't needed).
                    await _publishEndpoint.Publish(new InventoryCreationReversed(command.CorrelationId), context.CancellationToken);
                    return;
                }

                // Perform the reversal action. For this example, let's assume deletion.
                // If it's a soft delete, the method on repository might be 'SoftDeleteAsync' or 'UpdateStatusAsync'.
                // Assuming a hard delete for now via RemoveAsync:
                await _inventoryRepository.RemoveAsync(inventoryItem, context.CancellationToken);

                _logger.LogInformation("Inventory item with Id: {InventoryId} successfully reversed (deleted). CorrelationId: {CorrelationId}",
                    command.InventoryId, command.CorrelationId);

                await _publishEndpoint.Publish(new InventoryCreationReversed(command.CorrelationId), context.CancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reversing inventory creation for CorrelationId: {CorrelationId}, InventoryId: {InventoryId}. Error: {ErrorMessage}",
                    command.CorrelationId, command.InventoryId, ex.Message);

                // Here, you might publish a specific InventoryReversalFailed event.
                // For example: await _publishEndpoint.Publish(new InventoryReversalFailed(command.CorrelationId, command.InventoryId, ex.Message));
                // For now, as per instruction, we are not defining InventoryReversalFailed.
                // Depending on desired saga behavior, you might rethrow to let MassTransit move to _error queue,
                // or handle it as a business exception that the saga might react to differently.
                // If we don't rethrow, and don't publish a failure event, the saga might not know this specific attempt failed.
                // For a compensation, it's often critical to ensure it completes or is retried.
                // Let's rethrow to utilize MassTransit's retry and error queue mechanisms.
                throw;
            }
        }
    }
}
