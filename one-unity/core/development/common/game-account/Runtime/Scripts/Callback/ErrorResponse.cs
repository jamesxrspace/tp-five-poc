namespace TPFive.Game.Account
{
    using System;
    using Newtonsoft.Json;

    [Serializable]
    public class ErrorResponse
    {
        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("error_description")]
        public string ErrorDescription { get; set; }
    }
}