namespace TPFive.Game.Account
{
    using System;
    using Newtonsoft.Json;

    [Serializable]
    public class Guest
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("nickname")]
        public string Nickname { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        public static Guest Parse(string guestJson)
        {
            return JsonConvert.DeserializeObject<Guest>(guestJson);
        }

        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}