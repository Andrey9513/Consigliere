using Consigliere.Features.Weather.Models;
using Consigliere.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Consigliere.Features.Weather
{
    public class WeatherAPI
    {
        private readonly HttpClient _weatherClient;
        private readonly IOptions<WeatherOptions> _weatherOptions;

        public WeatherAPI(IServiceProvider serviceProvider, IHttpClientFactory clientFactory, IOptions<WeatherOptions> weatherOptions)
        {
            _weatherClient = clientFactory.CreateClient("WeatherClient");
            _weatherOptions = weatherOptions;
        }

        public async Task<WeatherInfo> GetWeather()
        {
            var response = await _weatherClient.GetAsync($"?id={_weatherOptions.Value.CityId}&appid={_weatherOptions.Value.ApiKey}&units=metric");
            var content = await response.Content.ReadAsStringAsync();
            var weatherInfo = JsonConvert.DeserializeObject<WeatherInfo>(content);
            return weatherInfo;
        }
    }
}
