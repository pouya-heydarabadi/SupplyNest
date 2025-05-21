using DispatchR;
using DispatchR.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Scalar.AspNetCore;
using SupplyNest.Domain.Application.Services;
using SupplyNest.Domain.Application.Services.Commands;
using SupplyNest.Domain.Application.Services.Commands.Update;
using SupplyNest.Domain.Application.Services.Queries;
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

app.MapGet("/SearchByName/{searchTerm}", async (IProductRepository repository, [FromRoute] string searchTerm, CancellationToken cancellation) =>
{
    var products = await repository.SearchByNameAsync(searchTerm);
    return Results.Ok(products);
});

app.MapGet("/FuzzySearchByName/{searchTerm}", async (IProductRepository repository, [FromRoute] string searchTerm, [FromQuery] int? maxDistance, CancellationToken cancellation) =>
{
    var products = await repository.FuzzySearchByNameAsync(searchTerm, maxDistance ?? 2);
    return Results.Ok(products);
});

app.MapGet("/BooleanSearch", async (IProductRepository repository, [FromQuery] string searchTerm, [FromQuery] string searchOperator, CancellationToken cancellation) =>
{
    var products = await repository.BooleanSearchAsync(searchTerm, searchOperator);
    return Results.Ok(products);
});

app.MapGet("/WildcardSearch/{pattern}", async (IProductRepository repository, [FromRoute] string pattern, CancellationToken cancellation) =>
{
    var products = await repository.WildcardSearchAsync(pattern);
    return Results.Ok(products);
});

app.MapGet("/PhraseSearch/{phrase}", async (IProductRepository repository, [FromRoute] string phrase, CancellationToken cancellation) =>
{
    var products = await repository.PhraseSearchAsync(phrase);
    return Results.Ok(products);
});

app.MapGet("/ProximitySearch", async (IProductRepository repository, [FromQuery] string term1, [FromQuery] string term2, [FromQuery] int maxDistance, CancellationToken cancellation) =>
{
    var products = await repository.ProximitySearchAsync(term1, term2, maxDistance);
    return Results.Ok(products);
});

app.MapGet("/RangeSearch", async (IProductRepository repository, [FromQuery] decimal minPrice, [FromQuery] decimal maxPrice, CancellationToken cancellation) =>
{
    var products = await repository.RangeSearchAsync(minPrice, maxPrice);
    return Results.Ok(products);
});

app.MapGet("/FacetedSearch", async (IProductRepository repository, 
    [FromQuery] string? name,
    [FromQuery] string? category,
    [FromQuery] string? brand,
    [FromQuery] decimal? minPrice,
    [FromQuery] decimal? maxPrice,
    [FromQuery] bool? isActive,
    CancellationToken cancellation) =>
{
    var facets = new Dictionary<string, string>();
    if (!string.IsNullOrEmpty(name)) facets["name"] = name;
    if (!string.IsNullOrEmpty(category)) facets["category"] = category;
    if (!string.IsNullOrEmpty(brand)) facets["brand"] = brand;
    if (minPrice.HasValue) facets["minprice"] = minPrice.Value.ToString();
    if (maxPrice.HasValue) facets["maxprice"] = maxPrice.Value.ToString();
    if (isActive.HasValue) facets["isactive"] = isActive.Value.ToString();

    var products = await repository.FacetedSearchAsync(facets);
    return Results.Ok(products);
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