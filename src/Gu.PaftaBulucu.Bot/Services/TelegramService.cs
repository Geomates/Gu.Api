using Gu.PaftaBulucu.Bot.Exceptions;
using Gu.PaftaBulucu.Bot.Models.Telegram;
using Microsoft.Extensions.Configuration;
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
        Task<bool> AnswerCallbackQuery(AnswerCallbackQuery answerCallbackQuery);
    }

    public class TelegramService : ITelegramService
    {
        private readonly string _apiToken;
        private const string TelegramApiUrl = "https://api.telegram.org";

        public TelegramService(IConfiguration configuration)
        {
            _apiToken = configuration["Telegram:Token"];
        }

        public async Task<bool> DeleteMessage(TelegramDeleteMessage telegramDeleteMessage)
        {
            var url = $"{TelegramApiUrl}/bot{_apiToken}/deleteMessage";
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
            var url = $"{TelegramApiUrl}/bot{_apiToken}/sendMessage";
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

        public async Task<bool> AnswerCallbackQuery(AnswerCallbackQuery answerCallbackQuery)
        {
            var url = $"{TelegramApiUrl}/bot{_apiToken}/answerCallbackQuery";
            var content = new StringContent(JsonConvert.SerializeObject(answerCallbackQuery, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            }), Encoding.UTF8, "application/json");

            using HttpClient client = new HttpClient();
            using HttpResponseMessage response = await client.PostAsync(url, content);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
            return true;
        }
    }
}
