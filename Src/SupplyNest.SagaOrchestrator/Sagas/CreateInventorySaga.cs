using System;
using MassTransit;
using SupplyNest.SagaOrchestrator.Contracts;

namespace SupplyNest.SagaOrchestrator.Sagas
{
    public class CreateInventorySagaState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; }
        public Guid ProductId { get; set; }
        public Guid WarehouseId { get; set; }
        public Guid FiscalYearId { get; set; }
        public Guid InventoryId { get; set; }
        public DateTime? RequestTimestamp { get; set; }
        public DateTime? ProductValidationFailedTimestamp { get; set; }
        public DateTime? WarehouseValidationFailedTimestamp { get; set; }
        public DateTime? InventoryCreationFailedTimestamp { get; set; }
        public string FailureReason { get; set; }
    }

    public class CreateInventorySaga : MassTransitStateMachine<CreateInventorySagaState>
    {
        // Declare Events
        public Event<InventoryCreationRequested> InventoryCreationRequested { get; private set; }
        public Event<ProductValidated> ProductValidated { get; private set; }
        public Event<ProductValidationFailed> ProductValidationFailed { get; private set; }
        public Event<WarehouseValidated> WarehouseValidated { get; private set; }
        public Event<WarehouseValidationFailed> WarehouseValidationFailed { get; private set; }
        public Event<InventoryCreated> InventoryCreated { get; private set; }
        public Event<InventoryCreationFailed> InventoryCreationFailed { get; private set; }

        // Declare States
        public State AwaitingProductValidation { get; private set; }
        public State AwaitingWarehouseValidation { get; private set; }
        public State AwaitingInventoryCreation { get; private set; }
        // State Completed is implicitly defined by MassTransit (when Finalize() is called)
        // State Faulted is handled by transitioning to it.

        public CreateInventorySaga()
        {
            InstanceState(x => x.CurrentState);

            // Event Correlation
            Event(() => InventoryCreationRequested, x => x.CorrelateById(context => context.Message.CorrelationId));
            Event(() => ProductValidated, x => x.CorrelateById(context => context.Message.CorrelationId));
            Event(() => ProductValidationFailed, x => x.CorrelateById(context => context.Message.CorrelationId));
            Event(() => WarehouseValidated, x => x.CorrelateById(context => context.Message.CorrelationId));
            Event(() => WarehouseValidationFailed, x => x.CorrelateById(context => context.Message.CorrelationId));
            Event(() => InventoryCreated, x => x.CorrelateById(context => context.Message.CorrelationId)
                // If InventoryCreated message itself contains the InventoryId, we can use it for correlation too,
                // but CorrelationId is the primary key for the saga instance.
                // .CorrelateBy((saga, context) => saga.ProductId == context.Message.ProductId && saga.WarehouseId == context.Message.WarehouseId) // Example if no direct InventoryId
            );
            Event(() => InventoryCreationFailed, x => x.CorrelateById(context => context.Message.CorrelationId));

            // Saga Logic
            Initially(
                When(InventoryCreationRequested)
                    .Then(context =>
                    {
                        context.Saga.ProductId = context.Message.ProductId;
                        context.Saga.WarehouseId = context.Message.WarehouseId;
                        context.Saga.FiscalYearId = context.Message.FiscalYearId;
                        context.Saga.RequestTimestamp = DateTime.UtcNow;
                        LogContext.Info?.Log("Saga {CorrelationId}: InventoryCreationRequested for Product: {ProductId}, Warehouse: {WarehouseId}, FiscalYear: {FiscalYearId}",
                            context.Saga.CorrelationId, context.Saga.ProductId, context.Saga.WarehouseId, context.Saga.FiscalYearId);
                    })
                    .Publish(context => new ValidateProductCommand(context.Saga.CorrelationId, context.Saga.ProductId))
                    .TransitionTo(AwaitingProductValidation)
            );

            During(AwaitingProductValidation,
                When(ProductValidated)
                    .Then(context =>
                    {
                        LogContext.Info?.Log("Saga {CorrelationId}: ProductValidated for Product: {ProductId}", context.Saga.CorrelationId, context.Saga.ProductId);
                    })
                    .Publish(context => new ValidateWarehouseCommand(context.Saga.CorrelationId, context.Saga.WarehouseId))
                    .TransitionTo(AwaitingWarehouseValidation),

                When(ProductValidationFailed)
                    .Then(context =>
                    {
                        context.Saga.ProductValidationFailedTimestamp = DateTime.UtcNow;
                        context.Saga.FailureReason = context.Message.Reason;
                        LogContext.Warning?.Log("Saga {CorrelationId}: ProductValidationFailed for Product: {ProductId}. Reason: {Reason}",
                            context.Saga.CorrelationId, context.Saga.ProductId, context.Message.Reason);
                    })
                    .TransitionTo(Faulted) // No compensation, first step
            );

            During(AwaitingWarehouseValidation,
                When(WarehouseValidated)
                    .Then(context =>
                    {
                        LogContext.Info?.Log("Saga {CorrelationId}: WarehouseValidated for Warehouse: {WarehouseId}", context.Saga.CorrelationId, context.Saga.WarehouseId);
                    })
                    .Publish(context => new CreateInventoryCommand(context.Saga.CorrelationId, context.Saga.ProductId, context.Saga.WarehouseId, context.Saga.FiscalYearId))
                    .TransitionTo(AwaitingInventoryCreation),

                When(WarehouseValidationFailed)
                    .Then(context =>
                    {
                        context.Saga.WarehouseValidationFailedTimestamp = DateTime.UtcNow;
                        context.Saga.FailureReason = context.Message.Reason;
                        LogContext.Warning?.Log("Saga {CorrelationId}: WarehouseValidationFailed for Warehouse: {WarehouseId}. Reason: {Reason}",
                            context.Saga.CorrelationId, context.Saga.WarehouseId, context.Message.Reason);
                    })
                    .TransitionTo(Faulted) // No compensation for product validation as it was a read
            );

            During(AwaitingInventoryCreation,
                When(InventoryCreated)
                    .Then(context =>
                    {
                        context.Saga.InventoryId = context.Message.InventoryId; // Assuming InventoryCreated event carries InventoryId
                        LogContext.Info?.Log("Saga {CorrelationId}: InventoryCreated with Id: {InventoryId}", context.Saga.CorrelationId, context.Saga.InventoryId);
                    })
                    .Finalize(), // Marks saga as Completed and removes it by default

                When(InventoryCreationFailed)
                    .Then(context =>
                    {
                        context.Saga.InventoryCreationFailedTimestamp = DateTime.UtcNow;
                        context.Saga.FailureReason = context.Message.Reason;
                        LogContext.Error?.Log("Saga {CorrelationId}: InventoryCreationFailed. Reason: {Reason}",
                            context.Saga.CorrelationId, context.Message.Reason);
                        // No compensation needed for prior validation steps as they were reads.
                        // If a more complex saga had prior writes, this is where you might publish compensation commands.
                        // For example, if InventoryCreationFailed might leave some partial state that needs cleanup,
                        // we could publish ReverseInventoryCreationCommand here.
                        // However, for this specific message, it implies creation itself failed, so there might be nothing to reverse *from this specific command*.
                        // The ReverseInventoryCreationCommand is typically for when InventoryCreated *succeeded*, but a *later* step in an overall business process fails.
                    })
                    .TransitionTo(Faulted)
            );

            // Define what happens when the saga is completed or faulted.
            // MassTransit automatically removes finalized sagas.
            // Faulted sagas remain by default unless configured otherwise.
            SetCompletedWhenFinalized();
        }
    }
}
