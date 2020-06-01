using Gu.PaftaBulucu.Bot.Exceptions;
using Gu.PaftaBulucu.Bot.Models.Telegram;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Gu.PaftaBulucu.Bot.Services
{
    public interface ITelegramService
    {
        Task<bool> SendMessage(TelegramMessage message);
        Task<bool> DeleteMessage(TelegramDeleteMessage telegramDeleteMessage);
    }

    public class TelegramService : ITelegramService
    {
        private readonly IParameterService _parameterService;
        private const string TelegramApiUrl = "https://api.telegram.org";

        public TelegramService(IParameterService parameterService)
        {
            _parameterService = parameterService;
        }

        public async Task<bool> DeleteMessage(TelegramDeleteMessage telegramDeleteMessage)
        {
            var apiToken = await  _parameterService.GetTelegramToken();

            var url = $"{TelegramApiUrl}/bot{apiToken}/deleteMessage";
            var content = new StringContent(JsonConvert.SerializeObject(telegramDeleteMessage, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            }), Encoding.UTF8, "application/json");

            using var client = new HttpClient();
            using var response = await client.PostAsync(url, content);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
            return true;
        }

        public async Task<bool> SendMessage(TelegramMessage message)
        {
            var apiToken = await _parameterService.GetTelegramToken();

            var url = $"{TelegramApiUrl}/bot{apiToken}/sendMessage";
            var content = new StringContent(JsonConvert.SerializeObject(message, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            }), Encoding.UTF8, "application/json");

            using var client = new HttpClient();
            using var response = await client.PostAsync(url, content);
            if (!response.IsSuccessStatusCode)
            {
                throw new TelegramApiException(await response.Content.ReadAsStringAsync());
            }
            return true;
        }
    }
}
