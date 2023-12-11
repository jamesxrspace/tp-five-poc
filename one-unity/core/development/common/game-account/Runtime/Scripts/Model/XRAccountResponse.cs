using System;
using Newtonsoft.Json;

namespace TPFive.Game.Account
{
    [Serializable]
    public class XRAccountStatusResponse
    {
        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("ticket")]
        public string Ticket { get; set; }

        [JsonProperty("token")]
        public string AuthingToken { get; set; }
    }
}
