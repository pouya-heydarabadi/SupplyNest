namespace SupplyNest.SagaOrchestrator.Contracts;

public record InventoryDecreased(
    Guid CorrelationId,
    Guid ProductId,
    int QuantityDecreased
);
