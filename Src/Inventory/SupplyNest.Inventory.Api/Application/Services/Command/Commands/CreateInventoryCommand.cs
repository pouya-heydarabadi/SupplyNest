using DispatchR.Requests.Send;

namespace SupplyNest.Inventory.Api.Application.Services.Command.Commands;

public sealed record CreateInventoryCommand:IRequest<CreateInventoryCommand, ValueTask<Domain.Entities.Inventory>>
{
    public Guid WarehouseId { get; init; }
    public Guid ProductId { get; init; }
    public Guid FiscalYearId { get; init; }
}