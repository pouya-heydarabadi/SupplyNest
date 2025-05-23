using SupplyNest.Inventory.Api.Application.Interfaces;
using SupplyNest.Inventory.Api.Infrastructure.SqlServerConfig.Persistence;

namespace SupplyNest.Inventory.Api.Infrastructure.SqlServerConfig;

public static class ConfigureSqlServerServices
{
    public static IServiceCollection ConfigRepositories(this IServiceCollection services)
    {
        services.AddScoped<IInventoryRepository, InventoryRepository>();

        return services;
    }
}