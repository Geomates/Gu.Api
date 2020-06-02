using Newtonsoft.Json;

namespace Gu.PaftaBulucu.Bot.Models.Telegram
{
    public class TelegramResponse
    {
        [JsonProperty("error_code")]
        public int ErrorCode { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
