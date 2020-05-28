using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Gu.PaftaBulucu.Bot.Models
{
    class WebhookMessage
    {
        [JsonProperty("message")]
        public Message Message { get; set; }

        [JsonProperty("callback_query")]
        public CallbackQuery CallbackQuery { get; set; }
    }
}
