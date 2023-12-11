using Newtonsoft.Json;

namespace TPFive.OpenApi.GameServer
{
    internal static class JsonUtils
    {
        private static JsonSerializerSettings toStringJsonSerializerSettings;

        public static JsonSerializerSettings ToStringJsonSerializerSettings
        {
            get
            {
                if (toStringJsonSerializerSettings == null)
                {
                    toStringJsonSerializerSettings = new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Objects,
                    };
                }

                return toStringJsonSerializerSettings;
            }
        }
    }
}
