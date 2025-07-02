namespace BuldingBlocks.Application.ConsuleInterfaces;

public interface IConsulService
{
    Task<Uri> GetServiceUrl(string serviceName);
}