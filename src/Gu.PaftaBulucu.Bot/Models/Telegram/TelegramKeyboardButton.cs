using Newtonsoft.Json;

namespace Gu.PaftaBulucu.Bot.Models.Telegram
{
    public class TelegramKeyboardButton
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("request_location")]
        public bool RequestLocation { get; set; }
    }
}
