using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Infrastructure.ConsulServices;

public static class ServiceRegister
{
    public static WebApplication RegisterConsul(this WebApplication app, 
        (string serviceName, string serviceId, string consulHostName) service)
    {
        // دریافت ILogger از طریق DI
        var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(ServiceRegister));
        var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

        // دریافت URL پایه
        var baseUrl = app.Urls.FirstOrDefault() ?? app.Configuration["ASPNETCORE_URLS"] ?? "http://localhost:5044/";
        var uri = new Uri(baseUrl);
        var host = uri.Host;
        var servicePort = uri.Port;

        // ایجاد کلاینت Consul
        using var consulClient = new ConsulClient(config =>
        {
            config.Address = new Uri(service.consulHostName);
        });

        // تنظیم URL بررسی سلامت
        var healthCheckUrl = $"{baseUrl.TrimEnd('/')}/agent/checks";
        
        // تنظیم ثبت سرویس در Consul
        var registration = new AgentServiceRegistration
        {
            ID = service.serviceId,
            Name = service.serviceName,
            Address = host,
            Port = servicePort,
            // Check = new AgentServiceCheck
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
            logger.LogInformation("سرویس {ServiceName} با موفقیت در Consul ثبت شد", registration.Name);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "خطا در ثبت سرویس {ServiceName} در Consul", registration.Name);
            throw;
        }

        // ثبت رویداد توقف برنامه برای حذف سرویس از Consul
        lifetime.ApplicationStopped.Register(() =>
        {
            try
            {
                consulClient.Agent.ServiceDeregister(service.serviceId).GetAwaiter().GetResult();
                logger.LogInformation("سرویس {ServiceName} با موفقیت از Consul حذف شد", registration.Name);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "خطا در حذف سرویس {ServiceName} از Consul", registration.Name);
            }
        });

        return app;
    }
}