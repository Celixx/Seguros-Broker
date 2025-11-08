using Microsoft.Extensions.Configuration;

namespace Seguros_Broker
{
    public sealed class Settings
    {
        public required string connectionString { get; set; }
    }

    static class Configuration
    {
        static IConfigurationRoot config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddEnvironmentVariables()
        .Build();

        public static Settings Get()
        {
            Settings? settings = config.GetRequiredSection("Settings").Get<Settings>();

            return settings!;
        }
    }
}
