using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Gu.PaftaBulucu.Bot.Models
{
    public class Message
    {
        [JsonProperty("message_id")]
        public int MessageId { get; set; }

        [JsonProperty("chat")]
        public Chat Chat { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("location")]
        public Location Location { get; set; }
    }
}
