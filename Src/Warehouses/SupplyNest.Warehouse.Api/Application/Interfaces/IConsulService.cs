namespace SupplyNest.Warehouse.Api.Application.Interfaces;

public interface IConsulService
{
    Task<Uri> GetServiceUrl(string serviceName);
}