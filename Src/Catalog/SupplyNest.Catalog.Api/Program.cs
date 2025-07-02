using BuildingBlocks.Infrastructure.ConsulServices;
using Consul;
using DispatchR;
using DispatchR.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Scalar.AspNetCore;
using SupplyNest.Domain.Application.Services;
using SupplyNest.Domain.Application.Services.Commands;
using SupplyNest.Domain.Application.Services.Commands.Create;
using SupplyNest.Domain.Application.Services.Commands.Update;
using SupplyNest.Domain.Application.Services.Interfaces;
using SupplyNest.Domain.Application.Services.Queries;
using SupplyNest.Domain.Infrastructure;
using SupplyNest.Domain.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.Configure<ApplicationOptions>(builder.Configuration.GetSection("ApplicationOptions"));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<ApplicationOptions>>().Value);


// Register Consul Service
builder.Services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(config =>
{
    config.Address = new Uri("http://localhost:8500");
}));

builder.Services.AddSingleton<IMongoClient>(config =>
{
    var setting = config.GetRequiredService<IOptions<ApplicationOptions>>().Value;
    return new MongoClient(setting.MongoDbSettings.ConnectionString);
});


builder.Services.AddSingleton<IMongoDatabase>(config =>
{
    var setting = config.GetRequiredService<IOptions<ApplicationOptions>>().Value;
    var client = config.GetRequiredService<IMongoClient>();
    return client.GetDatabase(setting.MongoDbSettings.DatabaseName);
});


builder.WebHost.ConfigureKestrel(options =>
{ ;
    options.ListenLocalhost(5185, o => o.Protocols = HttpProtocols.Http1AndHttp2);
});


builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddDispatchR(typeof(Program).Assembly, withPipelines: true);

var app = builder.Build();

app.MapPost("/Create/", async (IMediator mediator, [FromBody] List<CreateProductCommand> request, CancellationToken cancellation) =>
{
    foreach (var createProductCommand in request)
    {
        var result = await mediator.Send(createProductCommand, cancellation);
    }
    return Results.Ok(true);
});

app.MapPost("/Update/", async (IMediator mediator, [FromBody] UpdateProductCommand  request, CancellationToken cancellation) =>
{
    var result = await mediator.Send(request, cancellation);
    return Results.Ok(result);
});

app.MapGet("/GetById/{id}", async (IMediator mediator, [FromRoute] Guid id, CancellationToken cancellation) =>
{
    var result = await mediator.Send(new GetProductByIdQuery()
    {
        Id = id
    }, cancellation);
    return Results.Ok(result);
});

if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
}

// Consul
var applicationOption = builder.Services.BuildServiceProvider().GetRequiredService<ApplicationOptions>();

app.RegisterConsul((applicationOption.ServiceRegister.ServiceName,
    applicationOption.ServiceRegister.ServiceId,applicationOption.ServiceRegister.ConsulHostName));

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Add search endpoints
app.MapPost("/SearchByName", async (IProductRepository repository, string searchTerm) =>
{
    var results = await repository.SearchByNameAsync(searchTerm);
    return Results.Ok(results);
});

app.MapPost("/BooleanSearch", async (IProductRepository repository, string searchTerm, string searchOperator) =>
{
    var results = await repository.BooleanSearchAsync(searchTerm, searchOperator);
    return Results.Ok(results);
});

app.MapPost("/FuzzySearch", async (IProductRepository repository, string searchTerm, int maxDistance = 2) =>
{
    var results = await repository.FuzzySearchByNameAsync(searchTerm, maxDistance);
    return Results.Ok(results);
});

app.MapPost("/WildcardSearch", async (IProductRepository repository, string pattern) =>
{
    var results = await repository.WildcardSearchAsync(pattern);
    return Results.Ok(results);
});

app.MapPost("/PhraseSearch", async (IProductRepository repository, string phrase) =>
{
    var results = await repository.PhraseSearchAsync(phrase);
    return Results.Ok(results);
});

app.MapPost("/ProximitySearch", async (IProductRepository repository, string term1, string term2, int maxDistance) =>
{
    var results = await repository.ProximitySearchAsync(term1, term2, maxDistance);
    return Results.Ok(results);
});

app.MapPost("/RangeSearch", async (IProductRepository repository, decimal minPrice, decimal maxPrice) =>
{
    var results = await repository.RangeSearchAsync(minPrice, maxPrice);
    return Results.Ok(results);
});

app.MapPost("/FacetedSearch", async (IProductRepository repository, Dictionary<string, string> facets) =>
{
    var results = await repository.FacetedSearchAsync(facets);
    return Results.Ok(results);
});


app.Run();