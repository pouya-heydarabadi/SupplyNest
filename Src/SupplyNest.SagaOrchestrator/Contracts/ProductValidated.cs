using System;

namespace SupplyNest.SagaOrchestrator.Contracts
{
    public record ProductValidated(Guid CorrelationId);
}
