using System.Net;
using System.Threading.Channels;
using BuldingBlocks.Application.ConsuleInterfaces;
using Consul;
using DispatchR.Requests;
using DispatchR.Requests.Send;
using Grpc.Net.Client;
using SupplyNest.Inventory.Api.Application.Interfaces;
using SupplyNest.Inventory.Api.Application.Services.Command.Commands;
using SupplyNest.Inventory.Api.Application.Services.Queries;

namespace SupplyNest.Inventory.Api.Application.Services.Command.CommandHandler;

public sealed class CreateInventoryCommandHandler(IInventoryRepository _inventoryRepository, IConsulClient _consulClient,
    ILogger<CreateInventoryCommandHandler> _logger, HttpClient _httpClient,
    IMediator _mediator ,
    IConsulService consulService) : IRequestHandler<CreateInventoryCommand, ValueTask<Domain.Entities.Inventory>>
{
    public async ValueTask<Domain.Entities.Inventory> Handle(CreateInventoryCommand request, CancellationToken cancellationToken)
    {
        var findDuplicate = await _inventoryRepository.ExistsAsync(request.WarehouseId,
            request.ProductId, 
            request.FiscalYearId, cancellationToken);
        
        if (findDuplicate)
            throw new Exception("Inventory already exists");

        var services = await _consulClient.Agent.Services();
        var catalogServices = services.Response.Values.FirstOrDefault(x => x.Service == "catalog-Service");
        if (catalogServices is null)
        {
            _logger.LogCritical("Service Catalog Not Found!!!");
            throw new Exception("Service Catalog Not Found!!!");
        }

        var addressUrl = $"http://{catalogServices.Address}:{catalogServices.Port}";
        _httpClient.BaseAddress = new Uri(addressUrl);

        // var product = await _httpClient.GetAsync("/GetById/"+$"{request.ProductId}");

        var findWarehouse = await _mediator.Send(new GetWarehouseByIdQuery()
        {
            WarehouseId = request.WarehouseId
        }, cancellationToken);
        
        Domain.Entities.Inventory inventory =
            Domain.Entities.Inventory.Create(request.ProductId , request.WarehouseId,request.FiscalYearId, 0);
                
        Domain.Entities.Inventory result = await _inventoryRepository.AddAsync(inventory, cancellationToken);

        return inventory;
    }
}