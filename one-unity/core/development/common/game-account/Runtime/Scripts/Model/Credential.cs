using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace TPFive.Game.Account
{
    [Serializable]
    public class Credential
    {
        private static readonly string TAG = "[XRAccount][Credential] ";

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("id_token")]
        public string IdToken { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("expired_at")]
        public long ExpiredAt { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }

        public static Credential Parse(string json)
        {
            return JsonConvert.DeserializeObject<Credential>(json);
        }

        public static bool IsValid(Credential credential)
        {
            Debug.Log($"{TAG} Check credential IsValid()");

            // If access token is null, return not valid.
            if (credential == null || string.IsNullOrEmpty(credential.AccessToken))
            {
                Debug.Log($"{TAG} No access_token. Credential is not valid.");
                return false;
            }

            // If access token not expired, return valid.
            if (GetTokenExpirationTime(credential) > DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            {
                Debug.Log($"{TAG} The access token is valid");
                return true;
            }

            // FIXME 要檢查refresh_token的效期
            if (!string.IsNullOrEmpty(credential.RefreshToken))
            {
                Debug.Log($"{TAG} Although access token is expired but can be refresh.(valid)");
                return true;
            }

            Debug.Log($"{TAG} Credential is not valid.");
            return false;
        }

        public static Dictionary<string, object> GetTokenPayload(string token)
        {
            var payload = token.Split('.')[1];

            // Decode Base64 string. 解碼前，在每個Base64塊的末尾填充適量的字元「=」，確保Base64編碼長度是4的倍數，以符合 Base64 的規範。
            payload = payload.PadRight(4 * ((payload.Length + 3) / 4), '=');
            var payloadJson = Encoding.UTF8.GetString(Convert.FromBase64String(payload));
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(payloadJson);
        }

        public static long GetTokenExpirationTime(Credential credentials)
        {
            if (credentials == null || credentials.AccessToken == null)
            {
                return 0;
            }

            long exp = 0;
            var data = GetTokenPayload(credentials.AccessToken);
            if (data.ContainsKey("exp"))
            {
                exp = (long)Convert.ToInt64(data["exp"]);
            }
            else
            {
                Debug.LogWarning($"{TAG} expiration time is not found in access_token.");
            }

            return exp;
        }

        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}