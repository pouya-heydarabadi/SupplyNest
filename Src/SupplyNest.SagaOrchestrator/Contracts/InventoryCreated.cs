using System;

namespace SupplyNest.SagaOrchestrator.Contracts
{
    public record InventoryCreated(Guid CorrelationId);
}
