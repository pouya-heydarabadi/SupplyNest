using System;

namespace SupplyNest.SagaOrchestrator.Contracts
{
    public record ProductValidationFailed(Guid CorrelationId, string Reason);
}
