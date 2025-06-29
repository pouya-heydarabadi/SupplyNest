﻿using System.Text.Json.Serialization;

namespace SupplyNest.Inventory.Api.Infrastructure
{
    public class ApplicationOptions
    {
        public LoggingOptions Logging { get; set; }
        public string AllowedHosts { get; set; }
        public RedisConfiguration RedisConfiguration { get; set; }
        public DbConfiguration DbConfiguration { get; set; } 
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

    public class RedisConfiguration
    {
        public string ConfigurationUrl { get; set; }
        public string InstanceName { get; set; }
        public string PassWord { get; set; }
        public bool UseSSL { get; set; }
    }

    public class DbConfiguration
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

