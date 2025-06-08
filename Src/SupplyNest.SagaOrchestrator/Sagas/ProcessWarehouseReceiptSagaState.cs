using MassTransit;
using System;

namespace SupplyNest.SagaOrchestrator.Sagas;

public class ProcessWarehouseReceiptSagaState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = null!;
    public Guid ReceiptId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public DateTime ReceivedTimestamp { get; set; }
    public string? FailureReason { get; set; }
}
