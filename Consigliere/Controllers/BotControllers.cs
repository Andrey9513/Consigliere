using Consigliere.Features.Weather;
using Consigliere.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Consigliere.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BotController : Controller
    {
        private readonly WeatherAPI _weatherAPI;
        private readonly TelegramBotClient _telegramBotClient;
        public BotController(WeatherAPI weatherAPI, IOptions<TelegramOptions> botOptions)
        {
            _weatherAPI = weatherAPI;
            _telegramBotClient = new TelegramBotClient(botOptions.Value.ApiKey);
        }

        [HttpPost]
        public async void Post([FromBody]Update update)
        {
            if (update is null) return;
            var message = update.Message;
            if (message?.Type == MessageType.Text)
            {
                var reply = message.Text switch
                {
                    "/weather" => $"Temperature {(await _weatherAPI.GetWeather()).Main.Temp}C, feels like {(await _weatherAPI.GetWeather()).Main.Feels_like}C",
                    _ => "I don't understand you, Don"
                };

                await _telegramBotClient.SendTextMessageAsync(message.Chat.Id, reply);
            }
        }
    }
}
