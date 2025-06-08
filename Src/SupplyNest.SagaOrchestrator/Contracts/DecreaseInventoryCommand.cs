namespace SupplyNest.SagaOrchestrator.Contracts;

public record DecreaseInventoryCommand(
    Guid CorrelationId,
    Guid ReceiptId, // Keep track of the original receipt
    Guid ProductId,
    int Quantity
);
