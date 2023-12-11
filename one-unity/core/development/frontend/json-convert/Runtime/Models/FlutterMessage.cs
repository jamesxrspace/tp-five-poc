using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace CSharpDataModel
{
    public class FlutterMessage
    {
        public string sessionId;
        public FlutterMessageType type;
        public string data;
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum FlutterMessageType
    {
        [EnumMember(Value = "FLUTTER")]
        Flutter,
        [EnumMember(Value = "LOGIN_SUCCESS")]
        LoginSuccess,
        [EnumMember(Value = "TO_REEL")]
        ToReel,
    }
}
