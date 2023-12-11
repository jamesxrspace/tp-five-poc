using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TPFive.Game.Resource
{
    /// <summary>
    /// XRObject is an asset data structure used to define Scene or Decoration, etc.
    /// </summary>
    [Serializable]
    public partial class XRObject
    {
        [JsonProperty("uid")]
        public string Uid { get; set; }

        [JsonProperty("owner")]
        public string Owner { get; set; }

        [JsonProperty("object_name")]
        public string ObjectName { get; set; }

        [JsonProperty("object_type")]
        public string ObjectType { get; set; }

        [JsonProperty("file_format")]
        public string FileFormat { get; set; }

        [JsonProperty("bundle_id")]
        public string BundleId { get; set; }

        [JsonProperty("build_in_key")]
        public string BuildInKey { get; set; }

        [JsonProperty("components")]
        public Dictionary<string, object> Components { get; set; }

        public override string ToString()
        {
            return $"Uid={Uid}, Instantiator={Owner}, ObjectName={ObjectName}, ObjectType={ObjectType}, FileFormat={FileFormat}, BundleId={BundleId}";
        }
    }
}