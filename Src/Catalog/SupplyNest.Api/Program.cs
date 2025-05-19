using DispatchR;
using DispatchR.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Scalar.AspNetCore;
using SupplyNest.Domain.Application.Services;
using SupplyNest.Domain.Application.Services.Commands;
using SupplyNest.Domain.Application.Services.Commands.Update;
using SupplyNest.Domain.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.Configure<ApplicationOption>(builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.AddSingleton<IMongoClient>(config =>
{
    var setting = config.GetRequiredService<IOptions<ApplicationOption>>().Value;
    return new MongoClient(setting.ConnectionString);
});


builder.Services.AddSingleton<IMongoDatabase>(config =>
{
    var setting = config.GetRequiredService<IOptions<ApplicationOption>>().Value;
    var client = config.GetRequiredService<IMongoClient>();
    return client.GetDatabase(setting.DataBaseName);
});



builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddDispatchR(typeof(Program).Assembly, withPipelines: true);

var app = builder.Build();

app.MapPost("Create", async (IMediator mediator, [FromBody] CreateProductCommand  request, CancellationToken cancellation) =>
{
    var result = await mediator.Send(request, cancellation);
    return Results.Ok(result);
});

app.MapPost("Update", async (IMediator mediator, [FromBody] UpdateProductCommand  request, CancellationToken cancellation) =>
{
    var result = await mediator.Send(request, cancellation);
    return Results.Ok(result);
});

if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();