using System.Collections.Generic;
using Newtonsoft.Json.UnityConverters.Math;
using UnityEngine;
using JsonConvert = Newtonsoft.Json.JsonConvert;
using JsonConverter = Newtonsoft.Json.JsonConverter;
using JsonSerializerSettings = Newtonsoft.Json.JsonSerializerSettings;

namespace TPFive.Game.Resource.Serialization
{
    public static class TransformJsonExtension
    {
        // Use Vector3Converter and QuaternionConverter to serialize a Transform's Position, Rotation, LocalScale.
        private static readonly JsonSerializerSettings Settings = new ()
        {
            Converters = new List<JsonConverter> { new Vector3Converter(), new QuaternionConverter(), },
        };

        public static void FromJson(this Transform transform, string json)
        {
            var data = JsonConvert.DeserializeObject<TransformData>(json, Settings);
            transform.SetPositionAndRotation(data.Position, data.Rotation);
            transform.localScale = data.Scale;
        }

        public static string ToJson(this Transform transform)
        {
            var data = new TransformData
            {
                Position = transform.position,
                Rotation = transform.rotation,
                Scale = transform.localScale,
            };
            return JsonConvert.SerializeObject(data, Settings);
        }

        private class TransformData
        {
            public Vector3 Position { get; set; } = Vector3.zero;

            public Quaternion Rotation { get; set; } = Quaternion.identity;

            public Vector3 Scale { get; set; } = Vector3.one;
        }
    }
}