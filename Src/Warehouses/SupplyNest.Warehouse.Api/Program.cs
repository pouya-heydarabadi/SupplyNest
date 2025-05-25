using DispatchR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Scalar.AspNetCore;
using SupplyNest.Warehouse.Api.Application.Interfaces;
using SupplyNest.Warehouse.Api.Infrastructure;
using SupplyNest.Warehouse.Api.Infrastructure.ConsulServices;
using SupplyNest.Warehouse.Api.Infrastructure.SqlServerConfigs.DbContexts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AppOption>(builder.Configuration.GetSection("AppOption"));
builder.Services.AddSingleton<AppOption>(sp => sp.GetRequiredService<IOptions<AppOption>>().Value);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Grpc
builder.Services.AddGrpc();

// Consul
builder.Services.AddScoped<IConsulService, ConsulService>();

// DispatchR
builder.Services.AddDispatchR(typeof(Program).Assembly);

//SQL Server Configuration
AppOption options = builder.Services.BuildServiceProvider().GetRequiredService<IOptions<AppOption>>().Value;
builder.Services.AddDbContextPool<WarehouseDbContext>(config =>
{
    config.UseSqlServer(options.SqlServerConfiguration.ConnectionString);  
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}


app.MapGet("test", async (IConsulService ConsulService) =>
{
    var url = await ConsulService.GetServiceUrl("Inventory-Service");
    return Results.Ok();
});

app.UseHttpsRedirection();

app.Run();
