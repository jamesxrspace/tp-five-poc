/*
 * Server API - AIGC
 *
 * The Restful APIs of AIGC.
 *
 * The version of the OpenAPI document: 1.0.0
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */

#pragma warning disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TPFive.OpenApi.GameServer.Model
{
    /// <summary>
    /// Generate motion request
    /// </summary>
    public class GenerateMotionRequest : IEquatable<GenerateMotionRequest>
    {
        /// <summary>
        /// music input url
        /// </summary>
        /// <value>music input url</value>
        [JsonProperty(PropertyName = "input_url")]
        public string InputUrl { get; set; }

        public static bool operator ==(GenerateMotionRequest left, GenerateMotionRequest right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(GenerateMotionRequest left, GenerateMotionRequest right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// Returns the JSON string presentation of the object.
        /// </summary>
        /// <returns>JSON string presentation of the object.</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented, JsonUtils.ToStringJsonSerializerSettings);
        }

        /// <summary>
        /// Returns true if objects are equal.
        /// </summary>
        /// <param name="obj">Object to be compared.</param>
        /// <returns>Boolean.</returns>
        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == GetType() && Equals((GenerateMotionRequest)obj);
        }

        /// <summary>
        /// Returns true if GenerateMotionRequest instances are equal.
        /// </summary>
        /// <param name="other">Instance of GenerateMotionRequest to be compared.</param>
        /// <returns>Boolean.</returns>
        public bool Equals(GenerateMotionRequest other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return
                (
                    InputUrl == other.InputUrl ||
                    InputUrl != null &&
                    InputUrl.Equals(other.InputUrl)
                );
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>Hash code.</returns>
        public override int GetHashCode()
        {
            var hashCode = default(HashCode);

            hashCode.Add(InputUrl);

            return hashCode.ToHashCode();
        }
    }
}
#pragma warning restore