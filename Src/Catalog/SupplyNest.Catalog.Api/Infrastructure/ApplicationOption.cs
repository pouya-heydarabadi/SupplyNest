namespace SupplyNest.Domain.Infrastructure;

public class ApplicationOptions
{
    public ConnectionStrings ConnectionStrings { get; set; }
    public Logging Logging { get; set; }
    public string AllowedHosts { get; set; }
    public MongoDbSettings MongoDbSettings { get; set; }
    public ServiceRegister ServiceRegister { get; set; }
}

public class ConnectionStrings
{
    public string DefaultConnection { get; set; }
}

public class Logging
{
    public LogLevel LogLevel { get; set; }
}

public class LogLevel
{
    public string Default { get; set; }
    public string Microsoft_AspNetCore { get; set; }
}

public class MongoDbSettings
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
}

public class ServiceRegister
{
    public string ConsulHostName { get; set; }
    public string ServiceName { get; set; }
    public string ServiceId { get; set; }
}

