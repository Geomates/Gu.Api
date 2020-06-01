using System.Collections.Generic;
using Newtonsoft.Json;

namespace Gu.PaftaBulucu.Bot.Models.Telegram
{
    public class TelegramInlineKeyboardMarkup
    {
        [JsonProperty("inline_keyboard")]
        public IEnumerable<IEnumerable<TelegramInlineKeyboardButton>> InlineKeyboard { get; set; }
    }
}
