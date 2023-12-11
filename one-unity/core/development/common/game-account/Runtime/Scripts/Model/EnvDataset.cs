namespace TPFive.Game.Account
{
    using System;
    using Newtonsoft.Json;

    [Serializable]
    public class EnvDataset
    {
        [JsonProperty("build")]
        public string Build { get; set; }

        [JsonProperty("domain")]
        public string Domain { get; set; }

        [JsonProperty("build")]
        public string ClientId { get; set; }

        [JsonProperty("audDomain")]
        public string AudDomain { get; set; }
    }
}