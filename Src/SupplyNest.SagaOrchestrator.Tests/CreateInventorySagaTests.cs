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
    public class CreateInventorySagaTests : IAsyncLifetime
    {
        ITestHarness _harness;
        ISagaStateMachineTestHarness<CreateInventorySaga, CreateInventorySagaState> _sagaHarness;

        public async Task InitializeAsync()
        {
            _harness = new ServiceCollection()
                .AddMassTransitTestHarness(cfg =>
                {
                    cfg.AddSagaStateMachine<CreateInventorySaga, CreateInventorySagaState>();
                })
                .BuildServiceProvider(true)
                .GetRequiredService<ITestHarness>();

            await _harness.Start();
            _sagaHarness = _harness.GetSagaStateMachineHarness<CreateInventorySaga, CreateInventorySagaState>();
        }

        public async Task DisposeAsync()
        {
            await _harness.Stop();
        }

        [Fact]
        public async Task Should_complete_saga_when_all_steps_succeed()
        {
            // Arrange
            var correlationId = NewId.NextGuid(); // MassTransit NewId creates sequential Guids, good for tests
            var productId = NewId.NextGuid();
            var warehouseId = NewId.NextGuid();
            var fiscalYearId = NewId.NextGuid();
            var inventoryId = NewId.NextGuid();

            // Act & Assert - Step 1: InventoryCreationRequested
            await _harness.Bus.Publish(new InventoryCreationRequested(correlationId, productId, warehouseId, fiscalYearId));

            Assert.True(await _sagaHarness.Consumed.Any<InventoryCreationRequested>(x => x.Context.Message.CorrelationId == correlationId));
            Assert.True(await _harness.Published.Any<ValidateProductCommand>(x => x.Context.Message.CorrelationId == correlationId && x.Context.Message.ProductId == productId));
            var sagaInstance = await _sagaHarness.Exists(correlationId, x => x.AwaitingProductValidation);
            Assert.NotNull(sagaInstance);
            Assert.Equal(productId, sagaInstance.Message.ProductId);

            // Act & Assert - Step 2: ProductValidated
            await _harness.Bus.Publish(new ProductValidated(correlationId));

            Assert.True(await _sagaHarness.Consumed.Any<ProductValidated>(x => x.Context.Message.CorrelationId == correlationId));
            Assert.True(await _harness.Published.Any<ValidateWarehouseCommand>(x => x.Context.Message.CorrelationId == correlationId && x.Context.Message.WarehouseId == warehouseId));
            sagaInstance = await _sagaHarness.Exists(correlationId, x => x.AwaitingWarehouseValidation);
            Assert.NotNull(sagaInstance);

            // Act & Assert - Step 3: WarehouseValidated
            await _harness.Bus.Publish(new WarehouseValidated(correlationId));

            Assert.True(await _sagaHarness.Consumed.Any<WarehouseValidated>(x => x.Context.Message.CorrelationId == correlationId));
            Assert.True(await _harness.Published.Any<CreateInventoryCommand>(x =>
                x.Context.Message.CorrelationId == correlationId &&
                x.Context.Message.ProductId == productId &&
                x.Context.Message.WarehouseId == warehouseId &&
                x.Context.Message.FiscalYearId == fiscalYearId));
            sagaInstance = await _sagaHarness.Exists(correlationId, x => x.AwaitingInventoryCreation);
            Assert.NotNull(sagaInstance);

            // Act & Assert - Step 4: InventoryCreated
            await _harness.Bus.Publish(new InventoryCreated(correlationId, inventoryId)); // Saga expects InventoryId in this event

            Assert.True(await _sagaHarness.Consumed.Any<InventoryCreated>(x => x.Context.Message.CorrelationId == correlationId));
            // Assert that the saga is completed (Finalized)
            Assert.Null(await _sagaHarness.Exists(correlationId, x => x.Completed)); // Finalized sagas are removed
            Assert.True(await _sagaHarness.NotExists(correlationId)); // More explicit check for non-existence

            // Verify all expected commands were published
            Assert.True(await _harness.Published.SelectAsync<ValidateProductCommand>().Any());
            Assert.True(await _harness.Published.SelectAsync<ValidateWarehouseCommand>().Any());
            Assert.True(await _harness.Published.SelectAsync<CreateInventoryCommand>().Any());
        }

        [Fact]
        public async Task Should_fault_saga_when_product_validation_fails()
        {
            // Arrange
            var correlationId = NewId.NextGuid();
            var productId = NewId.NextGuid();
            var warehouseId = NewId.NextGuid();
            var fiscalYearId = NewId.NextGuid();

            // Act
            await _harness.Bus.Publish(new InventoryCreationRequested(correlationId, productId, warehouseId, fiscalYearId));
            // Ensure the saga processes the initial request before publishing the failure
            await _sagaHarness.Consumed.Any<InventoryCreationRequested>();

            await _harness.Bus.Publish(new ProductValidationFailed(correlationId, "Product does not exist"));

            // Assert
            Assert.True(await _sagaHarness.Consumed.Any<ProductValidationFailed>(x => x.Context.Message.CorrelationId == correlationId));
            var sagaInstance = await _sagaHarness.Exists(correlationId, x => x.Faulted);
            Assert.NotNull(sagaInstance);
            Assert.Equal("Product does not exist", sagaInstance.Message.FailureReason);
        }

        [Fact]
        public async Task Should_fault_saga_when_warehouse_validation_fails()
        {
            // Arrange
            var correlationId = NewId.NextGuid();
            var productId = NewId.NextGuid();
            var warehouseId = NewId.NextGuid();
            var fiscalYearId = NewId.NextGuid();

            // Act
            await _harness.Bus.Publish(new InventoryCreationRequested(correlationId, productId, warehouseId, fiscalYearId));
            await _sagaHarness.Consumed.Any<InventoryCreationRequested>(); // Wait for initial processing

            await _harness.Bus.Publish(new ProductValidated(correlationId));
            await _sagaHarness.Consumed.Any<ProductValidated>(); // Wait for product validation processing

            await _harness.Bus.Publish(new WarehouseValidationFailed(correlationId, "Warehouse is inactive"));

            // Assert
            Assert.True(await _sagaHarness.Consumed.Any<WarehouseValidationFailed>(x => x.Context.Message.CorrelationId == correlationId));
            var sagaInstance = await _sagaHarness.Exists(correlationId, x => x.Faulted);
            Assert.NotNull(sagaInstance);
            Assert.Equal("Warehouse is inactive", sagaInstance.Message.FailureReason);
        }

        [Fact]
        public async Task Should_fault_saga_when_inventory_creation_fails()
        {
            // Arrange
            var correlationId = NewId.NextGuid();
            var productId = NewId.NextGuid();
            var warehouseId = NewId.NextGuid();
            var fiscalYearId = NewId.NextGuid();

            // Act
            await _harness.Bus.Publish(new InventoryCreationRequested(correlationId, productId, warehouseId, fiscalYearId));
            await _sagaHarness.Consumed.Any<InventoryCreationRequested>();

            await _harness.Bus.Publish(new ProductValidated(correlationId));
            await _sagaHarness.Consumed.Any<ProductValidated>();

            await _harness.Bus.Publish(new WarehouseValidated(correlationId));
            await _sagaHarness.Consumed.Any<WarehouseValidated>();

            await _harness.Bus.Publish(new InventoryCreationFailed(correlationId, "Database error during inventory creation"));

            // Assert
            Assert.True(await _sagaHarness.Consumed.Any<InventoryCreationFailed>(x => x.Context.Message.CorrelationId == correlationId));
            var sagaInstance = await _sagaHarness.Exists(correlationId, x => x.Faulted);
            Assert.NotNull(sagaInstance);
            Assert.Equal("Database error during inventory creation", sagaInstance.Message.FailureReason);
        }
    }
}
