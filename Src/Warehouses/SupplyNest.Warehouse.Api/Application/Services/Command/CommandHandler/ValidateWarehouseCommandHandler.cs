using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using SupplyNest.Warehouse.Api.Application.Interfaces; // Assuming an IWarehouseRepository or similar exists here
using SupplyNest.SagaOrchestrator.Contracts; // For command and events

namespace SupplyNest.Warehouse.Api.Application.Services.Command.CommandHandler
{
    // Assuming you have an IWarehouseRepository or a similar interface for data access
    // If not, this will need to be created or adjusted to use DbContext directly or another service.
    // For now, I'll define it based on IProductRepository pattern from Catalog service.
    // public interface IWarehouseRepository { Task<Domain.Entities.Warehouse> GetByIdAsync(Guid id); /* other methods */ }

    public class ValidateWarehouseCommandHandler : IConsumer<ValidateWarehouseCommand>
    {
        // Replace IWarehouseRepository with your actual repository or service for warehouse data
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<ValidateWarehouseCommandHandler> _logger;

        public ValidateWarehouseCommandHandler(
            IWarehouseRepository warehouseRepository, // Adjust type as needed
            IPublishEndpoint publishEndpoint,
            ILogger<ValidateWarehouseCommandHandler> logger)
        {
            _warehouseRepository = warehouseRepository;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ValidateWarehouseCommand> context)
        {
            var command = context.Message;
            _logger.LogInformation("Received ValidateWarehouseCommand for CorrelationId: {CorrelationId}, WarehouseId: {WarehouseId}",
                command.CorrelationId, command.WarehouseId);

            try
            {
                // Assuming IWarehouseRepository has a method like GetByIdAsync or ExistsAsync
                // This is a placeholder for actual validation logic.
                // var warehouse = await _warehouseRepository.GetByIdAsync(command.WarehouseId); // CancellationToken can be passed

                // **** Placeholder Logic: Replace with actual validation ****
                // For now, let's assume validation passes if WarehouseId is not Guid.Empty
                // and fails otherwise for demonstration.
                bool isValid = command.WarehouseId != Guid.Empty;
                // In a real scenario:
                // var warehouse = await _warehouseRepository.GetByIdAsync(command.WarehouseId);
                // isValid = warehouse != null && warehouse.IsActive && warehouse.HasCapacity ... etc.

                if (isValid)
                {
                    _logger.LogInformation("Warehouse with Id: {WarehouseId} validated successfully. CorrelationId: {CorrelationId}",
                        command.WarehouseId, command.CorrelationId);
                    await _publishEndpoint.Publish(new WarehouseValidated(command.CorrelationId), context.CancellationToken);
                }
                else
                {
                    _logger.LogWarning("Warehouse with Id: {WarehouseId} failed validation or not found. CorrelationId: {CorrelationId}",
                        command.WarehouseId, command.CorrelationId);
                    await _publishEndpoint.Publish(new WarehouseValidationFailed(command.CorrelationId, $"Warehouse with Id {command.WarehouseId} failed validation or not found."), context.CancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating warehouse for CorrelationId: {CorrelationId}, WarehouseId: {WarehouseId}. Error: {ErrorMessage}",
                    command.CorrelationId, command.WarehouseId, ex.Message);
                await _publishEndpoint.Publish(new WarehouseValidationFailed(command.CorrelationId, $"Error validating warehouse: {ex.Message}"), context.CancellationToken);
                // No rethrow here, as the saga expects either WarehouseValidated or WarehouseValidationFailed
            }
        }
    }
}
