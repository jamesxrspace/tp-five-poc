namespace TPFive.Game.Record.Entry
{
    using Newtonsoft.Json;

    public class AigcMotionResponse
    {
        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }
    }
}