using System;

namespace SupplyNest.SagaOrchestrator.Contracts
{
    public record ValidateWarehouseCommand(Guid CorrelationId, Guid WarehouseId);
}
