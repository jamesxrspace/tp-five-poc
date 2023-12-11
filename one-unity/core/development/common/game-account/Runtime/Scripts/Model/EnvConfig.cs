namespace TPFive.Game.Account
{
    using System;
    using Newtonsoft.Json;

    [Serializable]
    public class EnvConfig
    {
        [JsonProperty("apiVersion")]
        public string ApiVersion { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("buildEnv")]
        public string BuildEnv { get; set; }

        [JsonProperty("data")]
        public EnvDataset[] EnvDatasetList { get; set; }

        [JsonProperty("currentDataset")]
        public EnvDataset CurrentDataset { get; set; }
    }
}