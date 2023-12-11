using System;
using Newtonsoft.Json;

namespace TPFive.Game.Account
{
    [Serializable]
    public class CreateGuestResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("message")]
        public Guest GuestInfo { get; set; }
    }
}