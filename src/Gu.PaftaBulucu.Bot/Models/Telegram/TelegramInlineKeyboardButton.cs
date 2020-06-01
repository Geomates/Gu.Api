using Newtonsoft.Json;

namespace Gu.PaftaBulucu.Bot.Models.Telegram
{
    public class TelegramInlineKeyboardButton
    {
        [JsonProperty("text")]
        public string Text { get; set; }
        
        [JsonProperty("callback_data")]
        public string CallBackData { get; set; }
    }
}
