using DispatchR;
using DispatchR.Requests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Scalar.AspNetCore;
using SupplyNest.Warehouse.Api.Application.Interfaces;
using SupplyNest.Warehouse.Api.Application.Services.WarehouseReceipts.Commands;
using SupplyNest.Warehouse.Api.Infrastructure;
using SupplyNest.Warehouse.Api.Infrastructure.ConsulServices;
using SupplyNest.Warehouse.Api.Infrastructure.Services;
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
builder.Services.AddDispatchR(typeof(Program).Assembly); // This will be an issue if DispatchR was removed from csproj

//SQL Server Configuration
AppOption options = builder.Services.BuildServiceProvider().GetRequiredService<IOptions<AppOption>>().Value;
builder.Services.AddDbContextPool<WarehouseDbContext>(config =>
{
    config.UseSqlServer(options.SqlServerConfiguration.ConnectionString);  
});

builder.Services.AddScoped<IInventoryService, InventoryService>();

// MassTransit Configuration
builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    // Automatically discover and register consumers from the assembly
    x.AddConsumers(typeof(Program).Assembly);

    x.UsingRabbitMq((context, cfg) =>
    {
        // Replace with actual configuration later
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ConfigureEndpoints(context);
    });
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


app.MapPost("create-warehouse-receipt", async (CreateWarehouseReceiptCommand command, IMediator mediator, CancellationToken cancellationToken) =>
{
    var result = await mediator.Send(command, cancellationToken);
    return Results.Ok(result);
});


app.UseHttpsRedirection();

app.Run();
