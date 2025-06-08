using System;

namespace SupplyNest.SagaOrchestrator.Contracts
{
    public record ReverseInventoryCreationCommand(Guid CorrelationId, Guid InventoryId);
}
