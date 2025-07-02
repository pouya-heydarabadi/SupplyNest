using DispatchR.Requests.Send;

namespace SupplyNest.Inventory.Api.Application.Services.Queries;

public sealed class GetWarehouseByIdQuery:IRequest<GetWarehouseByIdQuery, ValueTask<bool>>
{
    public Guid WarehouseId { get; set; }
}