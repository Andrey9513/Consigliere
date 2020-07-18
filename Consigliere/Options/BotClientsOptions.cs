using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Consigliere.Options
{
    public class BotClientsOptions
    {
        public Uri? BaseAddress { get; set; }

        public int Timeout { get; set; }

        /// <summary>
        /// Configures passed HttpClient according with own options
        /// </summary>
        /// <param name="httpClient">HttpClient</param>
        public virtual void ConfigureClient(HttpClient httpClient)
        {
            if (httpClient is null)
            {
                throw new ArgumentNullException(nameof(httpClient));
            }

            if (BaseAddress is null)
            {
                var message = $"The configuration value {nameof(BaseAddress)} is not specified for HTTP client";
                throw new OptionsValidationException(typeof(BotClientsOptions).Name, typeof(BotClientsOptions), new[] { message });
            }

            httpClient.BaseAddress = BaseAddress;
            httpClient.Timeout = TimeSpan.FromMilliseconds(Timeout);
        }
    }
}
