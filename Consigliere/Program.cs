using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Consigliere.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Telegram.Bot;

namespace Consigliere
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Directory.SetCurrentDirectory(root);

            var configBuilder = new ConfigurationBuilder();
            var config = ConfigureConfiguration(configBuilder);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .CreateLogger();

            new TelegramBotClient(config.GetSection("TelegramOptions")["ApiKey"])
                .SetWebhookAsync($"{config.GetSection("TelegramOptions")["WebhookBaseAddress"]}/api/Bot").Wait();

            try
            {
                Log.ForContext(typeof(Program)).Information("Starting web host. Assembly name: {Name}, version: {Version}",
                    Assembly.GetExecutingAssembly()?.GetName().Name,
                    Assembly.GetExecutingAssembly()?.GetName().Version);

                var host = CreateHostBuilder(args, config).Build();
                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args, IConfiguration config) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, confBuilder) => confBuilder.AddConfiguration(config))
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        public static IConfiguration ConfigureConfiguration(IConfigurationBuilder config) 
            => new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.test.json", optional: true)
            .AddJsonFile("appsettings.dev.json", optional: true)
            .Build();
    }
}
