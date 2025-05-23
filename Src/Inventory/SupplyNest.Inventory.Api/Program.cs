using Microsoft.Extensions.Options;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using Scalar.AspNetCore;
using StackExchange.Redis;
using SupplyNest.Inventory.Api.Infrastructure;
using SupplyNest.Inventory.Api.Infrastructure.SqlServerConfig;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.Configure<ApplicationOptions>(builder.Configuration.GetSection("ApplicationOptions"));

builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<ApplicationOptions>>().Value);


//Redis Configuration
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var options = sp.GetRequiredService<IOptions<ApplicationOptions>>().Value;
    var redisConfig = options.RedisConfiguration;

    var configString = redisConfig.ConfigurationUrl;

    var configurationOptions = ConfigurationOptions.Parse(configString);
    configurationOptions.Ssl = redisConfig.UseSSL;
    if (!string.IsNullOrEmpty(redisConfig.PassWord))
    {
        configurationOptions.Password = redisConfig.PassWord;
    }

    return ConnectionMultiplexer.Connect(configurationOptions);
});

builder.Services.AddSingleton(sp =>
{
    var redLockMultiplexer = new RedLockMultiplexer(sp.GetRequiredService<IConnectionMultiplexer>());
    var redLockFactory = RedLockFactory.Create([redLockMultiplexer]);
    return redLockFactory;
});

builder.Services.AddSingleton<IDistributedLockFactory>(sp=>sp.GetRequiredService<RedLockFactory>());
    

// SqlServer
builder.Services.ConfigureSqlServer();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
int i = 0;
app.MapPost("/Test", async (IDistributedLockFactory distributedLockFactory, CancellationToken cancellationToken) =>
{
    var resource = "Test";
    var retryDelay = TimeSpan.FromMilliseconds(200);
    var expirationTime = TimeSpan.FromSeconds(10);
    
    var distributedLock = await distributedLockFactory.CreateLockAsync(resource, expirationTime, expirationTime, retryDelay, cancellationToken);
    if (distributedLock.IsAcquired)
    {
        
        Console.WriteLine("Success: "+ i);
        i++;
        await distributedLock.DisposeAsync();
    }
});

app.UseHttpsRedirection();

app.Run();
