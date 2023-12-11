/*
 * Server API - Space
 *
 * The Restful APIs of Space.
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
    public class Space : IEquatable<Space>
    {
        /// <summary>
        /// Gets or Sets SpaceId
        /// </summary>
        [JsonProperty(PropertyName = "space_id")]
        public string SpaceId { get; set; }

        /// <summary>
        /// Gets or Sets SpaceGroupId
        /// </summary>
        [JsonProperty(PropertyName = "space_group_id")]
        public string SpaceGroupId { get; set; }

        /// <summary>
        /// Gets or Sets Name
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or Sets Description
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or Sets Thumbnail
        /// </summary>
        [JsonProperty(PropertyName = "thumbnail")]
        public string Thumbnail { get; set; }

        /// <summary>
        /// Gets or Sets Addressable
        /// </summary>
        [JsonProperty(PropertyName = "addressable")]
        public string Addressable { get; set; }

        /// <summary>
        /// Gets or Sets StartAt
        /// </summary>
        [JsonProperty(PropertyName = "start_at")]
        public DateTime StartAt { get; set; }

        /// <summary>
        /// Gets or Sets EndAt
        /// </summary>
        [JsonProperty(PropertyName = "end_at")]
        public DateTime EndAt { get; set; }

        public static bool operator ==(Space left, Space right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Space left, Space right)
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

            return obj.GetType() == GetType() && Equals((Space)obj);
        }

        /// <summary>
        /// Returns true if Space instances are equal.
        /// </summary>
        /// <param name="other">Instance of Space to be compared.</param>
        /// <returns>Boolean.</returns>
        public bool Equals(Space other)
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
                    SpaceGroupId == other.SpaceGroupId ||
                    SpaceGroupId != null &&
                    SpaceGroupId.Equals(other.SpaceGroupId)
                ) &&
                (
                    Name == other.Name ||
                    Name != null &&
                    Name.Equals(other.Name)
                ) &&
                (
                    Description == other.Description ||
                    Description != null &&
                    Description.Equals(other.Description)
                ) &&
                (
                    Thumbnail == other.Thumbnail ||
                    Thumbnail != null &&
                    Thumbnail.Equals(other.Thumbnail)
                ) &&
                (
                    Addressable == other.Addressable ||
                    Addressable != null &&
                    Addressable.Equals(other.Addressable)
                ) &&
                (
                    StartAt == other.StartAt ||
                    StartAt != null &&
                    StartAt.Equals(other.StartAt)
                ) &&
                (
                    EndAt == other.EndAt ||
                    EndAt != null &&
                    EndAt.Equals(other.EndAt)
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
            hashCode.Add(SpaceGroupId);
            hashCode.Add(Name);
            hashCode.Add(Description);
            hashCode.Add(Thumbnail);
            hashCode.Add(Addressable);
            hashCode.Add(StartAt);
            hashCode.Add(EndAt);

            return hashCode.ToHashCode();
        }
    }
}
#pragma warning restore