using BuildingBlocks.Infrastructure.ConsulServices;
using BuldingBlocks.Application.ConsuleInterfaces;
using DispatchR;
using DispatchR.Requests;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Scalar.AspNetCore;
using SupplyNest.Warehouse.Api.Application.Interfaces;
using SupplyNest.Warehouse.Api.Application.Services.WarehouseReceipts.Commands;
using SupplyNest.Warehouse.Api.Infrastructure;
using SupplyNest.Warehouse.Api.Infrastructure.Services;
using SupplyNest.Warehouse.Api.Infrastructure.SqlServerConfigs.DbContexts;
using SupplyNest.Warehouse.Api.Presentation.Grpc.Services.Warehouses;

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



//Kestrel
builder.WebHost.ConfigureKestrel(options =>
{ ;
    options.ListenLocalhost(5280, o => o.Protocols = HttpProtocols.Http2);
});

// DispatchR
builder.Services.AddDispatchR(typeof(Program).Assembly);

//SQL Server Configuration
AppOption options = builder.Services.BuildServiceProvider().GetRequiredService<IOptions<AppOption>>().Value;
builder.Services.AddDbContext<WarehouseDbContext>(config =>
{
    config.UseSqlServer(options.SqlServerConfiguration.ConnectionString);  
});

builder.Services.AddScoped<IInventoryService, InventoryService>();

var app = builder.Build();

AppOption applicationOptions = app.Services.GetRequiredService<AppOption>();
app.RegisterConsul((applicationOptions.ServiceRegister.ServiceName,
    applicationOptions.ServiceRegister.ServiceId,applicationOptions.ServiceRegister.ConsulHostName));

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


// grpc
app.MapGrpcService<GetWarehouseByIdGrpcServer>();


app.UseHttpsRedirection();

app.Run();
