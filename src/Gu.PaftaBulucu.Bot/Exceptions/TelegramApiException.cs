using System;
using Gu.PaftaBulucu.Bot.Models.Telegram;
using Newtonsoft.Json;

namespace Gu.PaftaBulucu.Bot.Exceptions
{
    public class TelegramApiException : Exception
    {
        public TelegramResponse Response { get; }

        public TelegramApiException(string response)
            :base(response)
        {
            Response = JsonConvert.DeserializeObject<TelegramResponse>(response);
        }
    }
}
