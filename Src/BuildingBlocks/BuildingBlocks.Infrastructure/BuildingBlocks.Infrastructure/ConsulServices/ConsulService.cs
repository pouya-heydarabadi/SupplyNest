using BuldingBlocks.Application.ConsuleInterfaces;
using Consul;

namespace BuildingBlocks.Infrastructure.ConsulServices;

public class ConsulService:IConsulService
{
    private ConsulClient _consulClient { get; set; }
    public ConsulService()
    {
        _consulClient = new ConsulClient(x =>
            x.Address = new Uri("http://localhost:8500"));
    }

    public async Task<Uri> GetServiceUrl(string serviceName)
    {
        var services = await _consulClient.Agent.Services();
        var catalogServices = services.Response.Values.FirstOrDefault(x => x.ID == serviceName);
        
        ArgumentNullException.ThrowIfNull(catalogServices);
        
        string uri = "http://" + catalogServices.Address + ":" + catalogServices.Port;
        
        
        Uri resultUri = new Uri(uri);
        return resultUri;

        // return null;
    }
}