using MassTransit; // Added for MassTransit
using SupplyNest.Inventory.Api.Application.Interfaces;
// Assuming CreateInventoryCommand will now come from SagaOrchestrator.Contracts
// using SupplyNest.Inventory.Api.Application.Services.Command.Commands;
using SupplyNest.SagaOrchestrator.Contracts; // Added for Saga contracts
using System; // For Exception
using System.Threading.Tasks; // For Task

namespace SupplyNest.Inventory.Api.Application.Services.Command.CommandHandler;

public sealed class CreateInventoryCommandHandler : IConsumer<CreateInventoryCommand>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public CreateInventoryCommandHandler(IInventoryRepository inventoryRepository, IPublishEndpoint publishEndpoint)
    {
        _inventoryRepository = inventoryRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<CreateInventoryCommand> context)
    {
        try
        {
            var request = context.Message; // This is SupplyNest.SagaOrchestrator.Contracts.CreateInventoryCommand

            var findDuplicate = await _inventoryRepository.ExistsAsync(request.WarehouseId,
                request.ProductId,
                request.FiscalYearId, context.CancellationToken);

            if (findDuplicate)
            {
                // Publish failure event
                await _publishEndpoint.Publish(new InventoryCreationFailed(request.CorrelationId, "Inventory already exists"), context.CancellationToken);
                return;
            }

            Domain.Entities.Inventory inventory =
                Domain.Entities.Inventory.Create(request.ProductId, request.WarehouseId, request.FiscalYearId, 0);

            Domain.Entities.Inventory result = await _inventoryRepository.AddAsync(inventory, context.CancellationToken);

            // Publish success event
            // Assuming result.Id is the new Inventory's Id. If not, adjust accordingly.
            await _publishEndpoint.Publish(new InventoryCreated(request.CorrelationId, result.Id), context.CancellationToken);
        }
        catch (Exception ex)
        {
            // Log the exception ex?

            // Publish failure event
            await _publishEndpoint.Publish(new InventoryCreationFailed(context.Message.CorrelationId, $"Inventory creation failed due to an internal error: {ex.Message}"), context.CancellationToken);
            // Optionally, rethrow if you want MassTransit to handle the exception (e.g., move to _error queue)
            // throw;
        }
    }
}