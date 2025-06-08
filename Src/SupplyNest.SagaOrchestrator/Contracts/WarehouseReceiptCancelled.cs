namespace SupplyNest.SagaOrchestrator.Contracts;

public record WarehouseReceiptCancelled(
    Guid CorrelationId,
    Guid ReceiptId
);
