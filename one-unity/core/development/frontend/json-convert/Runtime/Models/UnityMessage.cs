using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace CSharpDataModel
{
    public class UnityMessage
    {
        public string sessionId;
        public UnityMessageType type;
        public string data;
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum UnityMessageType
    {
        [EnumMember(Value = "UNITY")]
        Unity,
        [EnumMember(Value = "TO_AVATAR_EDIT")]
        ToAvatarEdit,
        [EnumMember(Value = "TO_SOCIAL_LOBBY")]
        ToSocialLobby,
    }
}
