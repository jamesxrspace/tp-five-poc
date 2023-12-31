/*
 * Server API - Room
 *
 * The Restful APIs of Room.
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
    public class RoomUserRegistry : IEquatable<RoomUserRegistry>
    {
        /// <summary>
        /// id of the space by which the room is defined. [Required]
        /// </summary>
        /// <value>id of the space by which the room is defined.</value>
        [JsonProperty(PropertyName = "space_id")]
        public string SpaceId { get; set; }

        /// <summary>
        /// id of the Fusion session to which the room belongs [Required]
        /// </summary>
        /// <value>id of the Fusion session to which the room belongs</value>
        [JsonProperty(PropertyName = "room_id")]
        public string RoomId { get; set; }

        /// <summary>
        /// id of the user [Required]
        /// </summary>
        /// <value>id of the user</value>
        [JsonProperty(PropertyName = "user_id")]
        public string UserId { get; set; }

        public static bool operator ==(RoomUserRegistry left, RoomUserRegistry right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(RoomUserRegistry left, RoomUserRegistry right)
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

            return obj.GetType() == GetType() && Equals((RoomUserRegistry)obj);
        }

        /// <summary>
        /// Returns true if RoomUserRegistry instances are equal.
        /// </summary>
        /// <param name="other">Instance of RoomUserRegistry to be compared.</param>
        /// <returns>Boolean.</returns>
        public bool Equals(RoomUserRegistry other)
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
                    SpaceId == other.SpaceId ||
                    SpaceId != null &&
                    SpaceId.Equals(other.SpaceId)
                ) &&
                (
                    RoomId == other.RoomId ||
                    RoomId != null &&
                    RoomId.Equals(other.RoomId)
                ) &&
                (
                    UserId == other.UserId ||
                    UserId != null &&
                    UserId.Equals(other.UserId)
                );
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>Hash code.</returns>
        public override int GetHashCode()
        {
            var hashCode = default(HashCode);

            hashCode.Add(SpaceId);
            hashCode.Add(RoomId);
            hashCode.Add(UserId);

            return hashCode.ToHashCode();
        }
    }
}
#pragma warning restore