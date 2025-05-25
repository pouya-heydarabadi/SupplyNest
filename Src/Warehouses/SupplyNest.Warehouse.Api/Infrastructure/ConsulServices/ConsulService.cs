using Consul;
using SupplyNest.Warehouse.Api.Application.Interfaces;

namespace SupplyNest.Warehouse.Api.Infrastructure.ConsulServices;

public class ConsulService:IConsulService
{
    private ConsulClient _consulClient { get; set; }
    public ConsulService(AppOption appOption)
    {
        _consulClient = new ConsulClient(x =>
            x.Address = new Uri(appOption.ServiceRegister.ConsulHostName));
    }

    public async Task<Uri> GetServiceUrl(string serviceName)
    {
        var result = await _consulClient.Health.Service(serviceName);
        var service = result.Response.FirstOrDefault();
        ArgumentNullException.ThrowIfNull(service);
        
        string uri = "http://" + service.Service.Address + ":" + service.Service.Port;


        Uri resultUri = new Uri(uri);
        return resultUri;

    }
}