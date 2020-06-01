using Newtonsoft.Json;

namespace Gu.PaftaBulucu.Bot.Models.Telegram
{
    public class TelegramDeleteMessage
    {
        [JsonProperty("chat_id")]
        public int ChatId { get; set; }

        [JsonProperty("message_id")]
        public int MessageId { get; set; }
    }
}
