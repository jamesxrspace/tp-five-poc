using System;
using Newtonsoft.Json;

namespace TPFive.Game.Account
{
    [Serializable]
    public class ErrorResponseJson
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("codes")]
        public string Codes { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}