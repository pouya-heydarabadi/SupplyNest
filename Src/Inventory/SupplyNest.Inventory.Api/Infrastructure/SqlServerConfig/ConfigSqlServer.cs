using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SupplyNest.Inventory.Api.Infrastructure.SqlServerConfig.DbContexts;

namespace SupplyNest.Inventory.Api.Infrastructure.SqlServerConfig;

public static class ConfigSqlServer

{
    public static IServiceCollection ConfigureSqlServer(this IServiceCollection service)
    {
        ApplicationOptions options = service.BuildServiceProvider().GetRequiredService<IOptions<ApplicationOptions>>().Value;
        
        service.AddDbContextPool<InventoryDbContext>(config =>
        {
            config.UseSqlServer(options.SqlServerConfiguration.ConnectionString);  
        });

        return service;
    }
}