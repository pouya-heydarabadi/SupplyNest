using System;

namespace SupplyNest.SagaOrchestrator.Contracts
{
    public record WarehouseValidated(Guid CorrelationId);
}
