using System;

namespace SupplyNest.SagaOrchestrator.Contracts
{
    public record ValidateProductCommand(Guid CorrelationId, Guid ProductId);
}
