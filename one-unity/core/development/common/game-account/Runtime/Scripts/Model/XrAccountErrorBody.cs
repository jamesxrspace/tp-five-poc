using System;
using Newtonsoft.Json;

namespace TPFive.Game.Account
{
    [Serializable]
    public class XrAccountErrorBody
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("error")]
        public ErrorResponseJson Error { get; set; }
    }
}