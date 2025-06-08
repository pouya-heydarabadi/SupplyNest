using System;

namespace SupplyNest.SagaOrchestrator.Contracts
{
    public record WarehouseValidationFailed(Guid CorrelationId, string Reason);
}
