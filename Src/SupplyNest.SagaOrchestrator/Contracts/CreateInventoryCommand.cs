using System;

namespace SupplyNest.SagaOrchestrator.Contracts
{
    public record CreateInventoryCommand(Guid CorrelationId, Guid ProductId, Guid WarehouseId, Guid FiscalYearId);
}
