using System;

namespace SupplyNest.SagaOrchestrator.Contracts
{
    public record InventoryCreationRequested(Guid CorrelationId, Guid ProductId, Guid WarehouseId, Guid FiscalYearId);
}
