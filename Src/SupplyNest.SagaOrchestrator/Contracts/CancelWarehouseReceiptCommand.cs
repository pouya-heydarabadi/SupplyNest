namespace SupplyNest.SagaOrchestrator.Contracts;

public record CancelWarehouseReceiptCommand(
    Guid CorrelationId,
    Guid ReceiptId,
    string Reason // Reason for cancellation
);
