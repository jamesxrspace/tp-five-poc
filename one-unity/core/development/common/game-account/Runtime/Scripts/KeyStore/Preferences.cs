using System;
using Newtonsoft.Json;

namespace TPFive.Game.Account
{
    [Serializable]
    public class Preferences
    {
        [JsonProperty("credentials")]
        public Credential Credential { get; set; }

        [JsonProperty("guest")]
        public Guest Guest { get; set; }
    }
}