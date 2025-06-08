namespace SupplyNest.SagaOrchestrator.Contracts;

public record InventoryDecreaseFailed(
    Guid CorrelationId,
    Guid ProductId,
    int QuantityAttempted,
    string Reason
);
