using System;

namespace SupplyNest.SagaOrchestrator.Contracts
{
    public record InventoryCreationFailed(Guid CorrelationId, string Reason);
}
