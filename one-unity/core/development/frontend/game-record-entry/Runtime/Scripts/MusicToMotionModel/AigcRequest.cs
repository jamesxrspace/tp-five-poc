namespace TPFive.Game.Record.Entry
{
    using Newtonsoft.Json;

    public class AigcMotionRequest
    {
        [JsonProperty("input_url")]
        public string InputUrl { get; set; }
    }
}