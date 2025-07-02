using BuldingBlocks.Application.ConsuleInterfaces;
using DispatchR.Requests.Send;
using Grpc.Net.Client;
using SupplyNest.Inventory.Api.Application.Services.Queries;
using SupplyNest.Inventory.Api.Presentations.Grpc.Structure.Client;

namespace SupplyNest.Inventory.Api.Application.Services.Query.QueryHandlers;

public class GetWarehouseByIdQueryHandler(IConsulService consulService):IRequestHandler<GetWarehouseByIdQuery, ValueTask<bool>>
{

    public async ValueTask<bool> Handle(GetWarehouseByIdQuery request, CancellationToken cancellationToken)
    {
        var url = await consulService.GetServiceUrl("Warehouse-Service-1");
        using var channel = GrpcChannel.ForAddress(url);
        var client = new GetWarehouseById.GetWarehouseByIdClient(channel);


        GetWarehouseByIdRequest requestGrpc = new()
        {
            WarehouseId = request.WarehouseId.ToString()
        };

        var res = await client.GetWarehouseByIdAsync(requestGrpc, cancellationToken: cancellationToken);

        return res.Result;
    }
}