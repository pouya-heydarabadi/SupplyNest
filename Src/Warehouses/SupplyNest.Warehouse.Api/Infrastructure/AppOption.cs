using System.Text.Json.Serialization;

namespace SupplyNest.Warehouse.Api.Infrastructure
{
    public class AppOption
    {
        public LoggingOptions Logging { get; set; }
        public string AllowedHosts { get; set; }
        public SqlServerConfiguration SqlServerConfiguration { get; set; } 
        public ServiceRegister ServiceRegister { get; set; }
    }

    public class LoggingOptions
    {
        public LogLevelOptions LogLevel { get; set; }
    }

    public class LogLevelOptions
    {
        public string Default { get; set; }

        [JsonPropertyName("Microsoft.AspNetCore")]
        public string MicrosoftAspNetCore { get; set; }
    }


    public class SqlServerConfiguration
    {
        public string ConnectionString { get; set; }
    }
    public class ServiceRegister
    {
        public string ConsulHostName { get; set; }
        public string ServiceName { get; set; }
        public string ServiceId { get; set; }
    }
}   