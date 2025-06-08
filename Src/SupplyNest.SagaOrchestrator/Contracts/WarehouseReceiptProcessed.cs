namespace SupplyNest.SagaOrchestrator.Contracts;

public record WarehouseReceiptProcessed(
    Guid CorrelationId, // Saga correlation ID, can be initiated by the Warehouse service or the Saga itself
    Guid ReceiptId,
    Guid ProductId,
    int Quantity,
    DateTime Timestamp
);
