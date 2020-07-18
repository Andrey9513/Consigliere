using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Consigliere.Options
{
    public class WeatherOptions
    {
        public string ApiKey { get; set; } = string.Empty;
        public int CityId { get; set; }
    }
}
