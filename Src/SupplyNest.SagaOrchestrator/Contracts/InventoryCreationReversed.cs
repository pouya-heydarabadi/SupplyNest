using System;

namespace SupplyNest.SagaOrchestrator.Contracts
{
    public record InventoryCreationReversed(Guid CorrelationId);
}
