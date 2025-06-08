using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using SupplyNest.SagaOrchestrator.Sagas;
using SupplyNest.SagaOrchestrator.Contracts;

namespace SupplyNest.SagaOrchestrator.Tests
{
    public class ProcessWarehouseReceiptSagaTests : IAsyncLifetime
    {
        ITestHarness _harness;
        ISagaStateMachineTestHarness<ProcessWarehouseReceiptSaga, ProcessWarehouseReceiptSagaState> _sagaHarness;

        public async Task InitializeAsync()
        {
            _harness = new ServiceCollection()
                .AddMassTransitTestHarness(cfg =>
                {
                    cfg.AddSagaStateMachine<ProcessWarehouseReceiptSaga, ProcessWarehouseReceiptSagaState>();
                    // If you had other sagas or consumers needed for these specific tests, register them here.
                })
                .BuildServiceProvider(true)
                .GetRequiredService<ITestHarness>();

            await _harness.Start();
            _sagaHarness = _harness.GetSagaStateMachineHarness<ProcessWarehouseReceiptSaga, ProcessWarehouseReceiptSagaState>();
        }

        public async Task DisposeAsync()
        {
            await _harness.Stop();
        }

        private async Task PublishWarehouseReceiptProcessed(Guid correlationId, Guid receiptId, Guid productId, int quantity, DateTime timestamp)
        {
            await _harness.Bus.Publish(new WarehouseReceiptProcessed(correlationId, receiptId, productId, quantity, timestamp));
        }

        private async Task PublishInventoryDecreased(Guid correlationId, Guid productId, int quantityDecreased)
        {
            await _harness.Bus.Publish(new InventoryDecreased(correlationId, productId, quantityDecreased));
        }

        private async Task PublishInventoryDecreaseFailed(Guid correlationId, Guid productId, int quantityAttempted, string reason)
        {
            await _harness.Bus.Publish(new InventoryDecreaseFailed(correlationId, productId, quantityAttempted, reason));
        }

        private async Task PublishWarehouseReceiptCancelled(Guid correlationId, Guid receiptId)
        {
            await _harness.Bus.Publish(new WarehouseReceiptCancelled(correlationId, receiptId));
        }


        [Fact]
        public async Task Should_complete_saga_when_inventory_decreased_successfully()
        {
            // Arrange
            var correlationId = NewId.NextGuid(); // Saga initiated by Warehouse Service, or use Guid.Empty for saga to init
            var receiptId = NewId.NextGuid();
            var productId = NewId.NextGuid();
            var quantity = 10;
            var timestamp = DateTime.UtcNow;

            // Act & Assert - Step 1: WarehouseReceiptProcessed
            await PublishWarehouseReceiptProcessed(correlationId, receiptId, productId, quantity, timestamp);

            Assert.True(await _sagaHarness.Consumed.Any<WarehouseReceiptProcessed>(x => x.Context.Message.CorrelationId == correlationId));
            var sagaInstance = await _sagaHarness.Exists(correlationId, x => x.AwaitingInventoryDecrease);
            Assert.NotNull(sagaInstance);
            Assert.Equal(receiptId, sagaInstance.Message.ReceiptId);
            Assert.Equal(productId, sagaInstance.Message.ProductId);
            Assert.Equal(quantity, sagaInstance.Message.Quantity);

            Assert.True(await _harness.Published.Any<DecreaseInventoryCommand>(x =>
                x.Context.Message.CorrelationId == correlationId &&
                x.Context.Message.ProductId == productId &&
                x.Context.Message.Quantity == quantity &&
                x.Context.Message.ReceiptId == receiptId));

            // Act & Assert - Step 2: InventoryDecreased
            await PublishInventoryDecreased(correlationId, productId, quantity);

            Assert.True(await _sagaHarness.Consumed.Any<InventoryDecreased>(x => x.Context.Message.CorrelationId == correlationId));
            Assert.True(await _sagaHarness.NotExists(correlationId)); // Saga should be finalized and removed

            // Verify commands
            Assert.True(await _harness.Published.SelectAsync<DecreaseInventoryCommand>().Any());
            // Ensure no compensation command was published
            Assert.False(await _harness.Published.SelectAsync<CancelWarehouseReceiptCommand>().Any());
        }

        [Fact]
        public async Task Should_compensate_and_complete_when_inventory_decrease_fails()
        {
            // Arrange
            var correlationId = NewId.NextGuid();
            var receiptId = NewId.NextGuid();
            var productId = NewId.NextGuid();
            var quantity = 10;
            var timestamp = DateTime.UtcNow;
            var failureReason = "Insufficient stock";

            // Act & Assert - Step 1: WarehouseReceiptProcessed
            await PublishWarehouseReceiptProcessed(correlationId, receiptId, productId, quantity, timestamp);

            Assert.True(await _sagaHarness.Consumed.Any<WarehouseReceiptProcessed>(x => x.Context.Message.CorrelationId == correlationId));
            var sagaInstance = await _sagaHarness.Exists(correlationId, x => x.AwaitingInventoryDecrease);
            Assert.NotNull(sagaInstance);
            Assert.True(await _harness.Published.Any<DecreaseInventoryCommand>(x => x.Context.Message.CorrelationId == correlationId));

            // Act & Assert - Step 2: InventoryDecreaseFailed
            await PublishInventoryDecreaseFailed(correlationId, productId, quantity, failureReason);

            Assert.True(await _sagaHarness.Consumed.Any<InventoryDecreaseFailed>(x => x.Context.Message.CorrelationId == correlationId));
            sagaInstance = await _sagaHarness.Exists(correlationId, x => x.AwaitingReceiptCancellation);
            Assert.NotNull(sagaInstance);
            Assert.Equal(failureReason, sagaInstance.Message.FailureReason);

            Assert.True(await _harness.Published.Any<CancelWarehouseReceiptCommand>(x =>
                x.Context.Message.CorrelationId == correlationId &&
                x.Context.Message.ReceiptId == receiptId &&
                x.Context.Message.Reason.Contains(failureReason))); // Check if reason is propagated

            // Act & Assert - Step 3: WarehouseReceiptCancelled (Compensation Succeeded)
            await PublishWarehouseReceiptCancelled(correlationId, receiptId);

            Assert.True(await _sagaHarness.Consumed.Any<WarehouseReceiptCancelled>(x => x.Context.Message.CorrelationId == correlationId));
            Assert.True(await _sagaHarness.NotExists(correlationId)); // Saga should be finalized and removed after compensation

            // Verify commands
            Assert.True(await _harness.Published.SelectAsync<DecreaseInventoryCommand>().Any());
            Assert.True(await _harness.Published.SelectAsync<CancelWarehouseReceiptCommand>().Any());
        }
    }
}
