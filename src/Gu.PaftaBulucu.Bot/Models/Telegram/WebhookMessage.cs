using Gu.PaftaBulucu.Bot.Models.Telegram;
using Newtonsoft.Json;

namespace Gu.PaftaBulucu.Bot.Models
{
    public class WebhookMessage
    {
        [JsonProperty("message")]
        public Message Message { get; set; }

        [JsonProperty("callback_query")]
        public CallbackQuery CallbackQuery { get; set; }
    }
}
