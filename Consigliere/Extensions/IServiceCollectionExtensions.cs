using Consigliere.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Consigliere.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public const string HttpClientsSectionName = "HttpClients";

        public static IServiceCollection AddServiceHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            
            var clientsSection = configuration.GetSection(HttpClientsSectionName);
            foreach (var client in clientsSection.GetChildren())
            {
                services.Configure<BotClientsOptions>(client.Key, configuration.GetSection($"{HttpClientsSectionName}:{client.Key}"));
                services.AddHttpClient(client.Key, (sp, httpClient) =>
                {
                    var optionsMonitor = sp.GetRequiredService<IOptionsMonitor<BotClientsOptions>>();
                    var namedOptions = optionsMonitor.Get(client.Key);
                    namedOptions.ConfigureClient(httpClient);
                });
            }

            return services;
        }
    }
}
