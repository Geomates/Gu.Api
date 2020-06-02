using Newtonsoft.Json;

namespace Gu.PaftaBulucu.Bot.Models.Telegram
{
    public class AnswerCallbackQuery
    {
        [JsonProperty("callback_query_id")]
        public string CallbackQueryId { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("show_alert")]
        public bool ShowAlert { get; set; }
    }
}
