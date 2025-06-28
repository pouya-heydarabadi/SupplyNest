using Consul;

namespace SupplyNest.Domain.Infrastructure.Consul;

public static class ServiceRegister
{
    public static WebApplication RegisterConsul(this WebApplication app, 
         ApplicationOptions applicationOptions, WebApplicationBuilder builder)
    {
        var baseUrl = app.Urls.FirstOrDefault() ?? builder.Configuration["ASPNETCORE_URLS"] ?? "http://localhost:5044/";
        var uri = new Uri(baseUrl);
        var host = uri.Host;
        var servicePort = uri.Port;

        var consulClient = new ConsulClient(config =>
        {
            config.Address = new Uri(applicationOptions.ServiceRegister.ConsulHostName);
        });

        var healthCheckUrl = $"{baseUrl.TrimEnd('/')}/agent/checks";
        healthCheckUrl = healthCheckUrl.Replace("http://", "").Trim();
        
        var registration = new AgentServiceRegistration()
        {
            ID = applicationOptions.ServiceRegister.ServiceId,
            Name = applicationOptions.ServiceRegister.ServiceName,
            Address = host,
            Port = servicePort,
            // Check = new AgentServiceCheck()
            // {
            //     Interval = TimeSpan.FromSeconds(2),
            //     Timeout = TimeSpan.FromSeconds(5),
            //     DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(30),
            //     HTTP = healthCheckUrl
            // }
        };
        
        try
        {
            consulClient.Agent.ServiceRegister(registration).Wait();
            app.Logger.LogInformation($"Service {registration.Name} registered with Consul successfully");
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, $"Failed to register service {registration.Name} with Consul");
        }

        app.Lifetime.ApplicationStopped.Register(() =>
        {
            try
            {
                consulClient.Agent.ServiceDeregister(applicationOptions.ServiceRegister.ServiceId).Wait();
                app.Logger.LogInformation($"Service {registration.Name} deregistered from Consul successfully");
            }
            catch (Exception ex)
            {
                app.Logger.LogError(ex, $"Failed to deregister service {registration.Name} from Consul");
            }
        });
        

        return app;
    }
}