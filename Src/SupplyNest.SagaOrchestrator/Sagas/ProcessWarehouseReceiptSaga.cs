using MassTransit;
using SupplyNest.SagaOrchestrator.Contracts; // Assuming contracts are in this namespace
using System; // For Guid, DateTime

namespace SupplyNest.SagaOrchestrator.Sagas;

public class ProcessWarehouseReceiptSaga : MassTransitStateMachine<ProcessWarehouseReceiptSagaState>
{
    // Events
    public Event<WarehouseReceiptProcessed> WarehouseReceiptProcessed { get; private set; } = null!;
    public Event<InventoryDecreased> InventoryDecreased { get; private set; } = null!;
    public Event<InventoryDecreaseFailed> InventoryDecreaseFailed { get; private set; } = null!;
    public Event<WarehouseReceiptCancelled> WarehouseReceiptCancelled { get; private set; } = null!;
    // Potentially: public Event<WarehouseReceiptCancellationFailed> WarehouseReceiptCancellationFailed { get; private set; } = null!;


    // States
    public State AwaitingInventoryDecrease { get; private set; } = null!;
    public State AwaitingReceiptCancellation { get; private set; } = null!;
    // public State Compensated { get; private set; } // Optional specific state for clarity
    // public State Faulted { get; private set; } // Implicitly available

    public ProcessWarehouseReceiptSaga()
    {
        InstanceState(x => x.CurrentState);

        Event(() => WarehouseReceiptProcessed, x =>
            x.CorrelateById(context => context.Message.CorrelationId) // Or initiate if not provided
             .SelectId(context => context.Message.CorrelationId == Guid.Empty ? NewId.NextGuid() : context.Message.CorrelationId)
        );
        Event(() => InventoryDecreased, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => InventoryDecreaseFailed, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => WarehouseReceiptCancelled, x => x.CorrelateById(context => context.Message.CorrelationId));

        Initially(
            When(WarehouseReceiptProcessed)
                .Then(context => {
                    context.Saga.ReceiptId = context.Message.ReceiptId;
                    context.Saga.ProductId = context.Message.ProductId;
                    context.Saga.Quantity = context.Message.Quantity;
                    context.Saga.ReceivedTimestamp = context.Message.Timestamp;
                    LogContext.Info?.Log("Saga {CorrelationId}: ProcessWarehouseReceiptSaga started for ReceiptId: {ReceiptId}, ProductId: {ProductId}, Quantity: {Quantity}",
                        context.Saga.CorrelationId, context.Saga.ReceiptId, context.Saga.ProductId, context.Saga.Quantity);
                })
                .Publish(context => new DecreaseInventoryCommand(
                    context.Saga.CorrelationId,
                    context.Saga.ReceiptId,
                    context.Saga.ProductId,
                    context.Saga.Quantity))
                .TransitionTo(AwaitingInventoryDecrease)
        );

        During(AwaitingInventoryDecrease,
            When(InventoryDecreased)
                .Then(context => {
                    LogContext.Info?.Log("Saga {CorrelationId}: InventoryDecreased successfully for ProductId: {ProductId}, Quantity: {Quantity}",
                        context.Saga.CorrelationId, context.Message.ProductId, context.Message.QuantityDecreased);
                })
                .Finalize(), // Saga completes successfully

            When(InventoryDecreaseFailed)
                .Then(context => {
                    context.Saga.FailureReason = context.Message.Reason;
                    LogContext.Warning?.Log("Saga {CorrelationId}: InventoryDecreaseFailed for ProductId: {ProductId}, Quantity: {Quantity}. Reason: {Reason}. Initiating compensation.",
                        context.Saga.CorrelationId, context.Message.ProductId, context.Message.QuantityAttempted, context.Message.Reason);
                })
                .Publish(context => new CancelWarehouseReceiptCommand(
                    context.Saga.CorrelationId,
                    context.Saga.ReceiptId,
                    $"Inventory decrease failed: {context.Message.Reason}")) // Provide a clear reason for cancellation
                .TransitionTo(AwaitingReceiptCancellation)
        );

        During(AwaitingReceiptCancellation,
            When(WarehouseReceiptCancelled)
                .Then(context => {
                    LogContext.Info?.Log("Saga {CorrelationId}: WarehouseReceiptCancelled successfully for ReceiptId: {ReceiptId} (Compensation complete).",
                        context.Saga.CorrelationId, context.Message.ReceiptId);
                })
                .Finalize(), // Saga completes after compensation

            // Optional: Handle if cancellation itself fails (e.g., transition to a Faulted state)
            // When(WarehouseReceiptCancellationFailed)
            //     .Then(context => {
            //         LogContext.Error?.Log("Saga {CorrelationId}: CRITICAL - WarehouseReceiptCancellationFailed for ReceiptId: {ReceiptId}. Reason: {Reason}. Manual intervention likely required.",
            //             context.Saga.CorrelationId, context.Saga.ReceiptId, context.Message.Reason);
            //         context.Saga.FailureReason = $"Compensation failed: {context.Message.Reason}";
            //     })
            //     .TransitionTo(Faulted)
        );

        SetCompletedWhenFinalized();
    }
}
