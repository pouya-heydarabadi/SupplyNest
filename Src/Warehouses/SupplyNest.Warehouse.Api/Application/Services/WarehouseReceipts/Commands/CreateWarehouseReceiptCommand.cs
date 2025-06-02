using DispatchR.Requests.Send;
using SupplyNest.Warehouse.Api.Application.Services.WarehouseReceipts.Dtos;

namespace SupplyNest.Warehouse.Api.Application.Services.WarehouseReceipts.Commands;

public sealed record CreateWarehouseReceiptCommand:IRequest<CreateWarehouseReceiptCommand, ValueTask<bool>>
{
    public CreateWarehouseReceiptDto CreateWarehouseReceiptDto { get; set; }
}
