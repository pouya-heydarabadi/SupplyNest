    using BuildingBlocks.Infrastructure.ConsulServices;
    using BuldingBlocks.Application.ConsuleInterfaces;
    using Carter;
    using Consul;
    using DispatchR;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
    using Microsoft.AspNetCore.Server.Kestrel.Core;
    using Microsoft.Extensions.Options;
    using RedLockNet;
    using RedLockNet.SERedis;
    using RedLockNet.SERedis.Configuration;
    using Scalar.AspNetCore;
    using StackExchange.Redis;
    using SupplyNest.Inventory.Api.Infrastructure;
    using SupplyNest.Inventory.Api.Infrastructure.SqlServerConfig;
    using SupplyNest.Inventory.Api.Presentations.Grpc.Services;

    var builder = WebApplication.CreateBuilder(args);

    
    
    builder.WebHost.ConfigureKestrel(options =>
    { ;
        options.ListenLocalhost(5044, o => o.Protocols = HttpProtocols.Http1AndHttp2);
    });

    // Add services to the container.
    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();

    builder.Services.Configure<ApplicationOptions>(builder.Configuration.GetSection("ApplicationOptions"));

    builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<ApplicationOptions>>().Value);

            
        
    // Register Consul Service
    builder.Services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(config =>
    {
        config.Address = new Uri("http://localhost:8500");
    }));

    builder.Services.AddScoped<IConsulService, ConsulService>();

    
    //DispatchR
    builder.Services.AddDispatchR(typeof(Program).Assembly);

    //Carter
    builder.Services.AddCarter();

    //Kestrel Configs
    builder.WebHost.ConfigureKestrel(serverOptions =>
    {
        serverOptions.Limits.MaxConcurrentConnections = 1000;
        serverOptions.Limits.MaxConcurrentUpgradedConnections = 1000;
    });


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


    builder.Services.AddSingleton<IDistributedLockFactory>(sp =>
    {
        var redisOptions = sp.GetRequiredService<IOptions<ApplicationOptions>>().Value;

        var connection = ConnectionMultiplexer.Connect(redisOptions.RedisConfiguration.ConfigurationUrl);

        var multiplexers = new List<RedLockMultiplexer>
        {
            connection
        };

        return RedLockFactory.Create(multiplexers);
    });


    // SqlServer
    builder.Services.ConfigureSqlServer();
    builder.Services.ConfigRepositories();

    // HealthCheck
    builder.Services.AddHealthChecks();

    builder.Services.AddGrpc();

    builder.Services.AddHttpClient();

    var app = builder.Build();

    //GRPC
    app.MapGrpcService<InventoryUpdateGrpcService>();

    ApplicationOptions applicationOptions = app.Services.GetRequiredService<ApplicationOptions>();
    app.RegisterConsul((applicationOptions.ServiceRegister.ServiceName,
        applicationOptions.ServiceRegister.ServiceId,applicationOptions.ServiceRegister.ConsulHostName));
    
    app.MapCarter();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
    }


    // HealthCheck
    app.MapHealthChecks("/agent/checks");

    app.UseHttpsRedirection();

    app.Run();
