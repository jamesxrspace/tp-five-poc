/*
 * Server API - Avatar
 *
 * The Restful APIs of Avatar.
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
    /// 
    /// </summary>
    public class AvatarActivateResponseData : IEquatable<AvatarActivateResponseData>
    {
        /// <summary>
        /// Gets or Sets Message
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        public static bool operator ==(AvatarActivateResponseData left, AvatarActivateResponseData right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AvatarActivateResponseData left, AvatarActivateResponseData right)
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

            return obj.GetType() == GetType() && Equals((AvatarActivateResponseData)obj);
        }

        /// <summary>
        /// Returns true if AvatarActivateResponseData instances are equal.
        /// </summary>
        /// <param name="other">Instance of AvatarActivateResponseData to be compared.</param>
        /// <returns>Boolean.</returns>
        public bool Equals(AvatarActivateResponseData other)
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
                    Message == other.Message ||
                    Message != null &&
                    Message.Equals(other.Message)
                );
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>Hash code.</returns>
        public override int GetHashCode()
        {
            var hashCode = default(HashCode);

            hashCode.Add(Message);

            return hashCode.ToHashCode();
        }
    }
}
#pragma warning restore