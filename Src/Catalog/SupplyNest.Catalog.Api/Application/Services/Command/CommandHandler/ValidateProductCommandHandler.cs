using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using SupplyNest.Domain.Application.Services.Interfaces; // Assuming IProductRepository is here
using SupplyNest.SagaOrchestrator.Contracts; // For command and events

namespace SupplyNest.Domain.Application.Services.Command.CommandHandler
{
    public class ValidateProductCommandHandler : IConsumer<ValidateProductCommand>
    {
        private readonly IProductRepository _productRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<ValidateProductCommandHandler> _logger;

        public ValidateProductCommandHandler(
            IProductRepository productRepository,
            IPublishEndpoint publishEndpoint,
            ILogger<ValidateProductCommandHandler> logger)
        {
            _productRepository = productRepository;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ValidateProductCommand> context)
        {
            var command = context.Message;
            _logger.LogInformation("Received ValidateProductCommand for CorrelationId: {CorrelationId}, ProductId: {ProductId}",
                command.CorrelationId, command.ProductId);

            try
            {
                // Assuming IProductRepository has a method like GetByIdAsync or ExistsAsync
                // For this example, let's assume GetByIdAsync returns the product or null
                var product = await _productRepository.GetByIdAsync(command.ProductId); // CancellationToken can be passed if method supports it

                if (product != null)
                {
                    // Additional validation logic can be added here if needed
                    // For example, check product.IsActive, product.IsStocked, etc.
                    _logger.LogInformation("Product with Id: {ProductId} validated successfully. CorrelationId: {CorrelationId}",
                        command.ProductId, command.CorrelationId);
                    await _publishEndpoint.Publish(new ProductValidated(command.CorrelationId), context.CancellationToken);
                }
                else
                {
                    _logger.LogWarning("Product with Id: {ProductId} not found. CorrelationId: {CorrelationId}",
                        command.ProductId, command.CorrelationId);
                    await _publishEndpoint.Publish(new ProductValidationFailed(command.CorrelationId, $"Product with Id {command.ProductId} not found."), context.CancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating product for CorrelationId: {CorrelationId}, ProductId: {ProductId}. Error: {ErrorMessage}",
                    command.CorrelationId, command.ProductId, ex.Message);
                await _publishEndpoint.Publish(new ProductValidationFailed(command.CorrelationId, $"Error validating product: {ex.Message}"), context.CancellationToken);
                // No rethrow here, as the saga expects either ProductValidated or ProductValidationFailed
            }
        }
    }
}
