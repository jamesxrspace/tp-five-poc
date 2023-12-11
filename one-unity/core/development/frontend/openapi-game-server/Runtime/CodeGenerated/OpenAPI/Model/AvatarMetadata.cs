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
    /// Detailed information of the avatar
    /// </summary>
    public class AvatarMetadata : IEquatable<AvatarMetadata>
    {
        /// <summary>
        /// create from which app
        /// </summary>
        /// <value>create from which app</value>
        [JsonProperty(PropertyName = "app_id")]
        public string AppId { get; set; }

        /// <summary>
        /// author of the avatar
        /// </summary>
        /// <value>author of the avatar</value>
        [JsonProperty(PropertyName = "author")]
        public string Author { get; set; }

        /// <summary>
        /// xrspace avatar format. this is a json string should be parsed using AvatarFormat.Deserialize
        /// </summary>
        /// <value>xrspace avatar format. this is a json string should be parsed using AvatarFormat.Deserialize</value>
        [JsonProperty(PropertyName = "avatar_format")]
        public Object AvatarFormat { get; set; }

        /// <summary>
        /// avatar id
        /// </summary>
        /// <value>avatar id</value>
        [JsonProperty(PropertyName = "avatar_id")]
        public string AvatarId { get; set; }

        /// <summary>
        /// avatar url
        /// </summary>
        /// <value>avatar url</value>
        [JsonProperty(PropertyName = "avatar_url")]
        public string AvatarUrl { get; set; }

        /// <summary>
        /// avatar created time
        /// </summary>
        /// <value>avatar created time</value>
        [JsonProperty(PropertyName = "created_at")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or Sets Thumbnail
        /// </summary>
        [JsonProperty(PropertyName = "thumbnail")]
        public AvatarThumbnail Thumbnail { get; set; }

        /// <summary>
        /// Gets or Sets Type
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public AvatarModelType Type { get; set; }

        /// <summary>
        /// avatar updated time
        /// </summary>
        /// <value>avatar updated time</value>
        [JsonProperty(PropertyName = "updated_at")]
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// xrspace user id
        /// </summary>
        /// <value>xrspace user id</value>
        [JsonProperty(PropertyName = "xrid")]
        public string Xrid { get; set; }

        public static bool operator ==(AvatarMetadata left, AvatarMetadata right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AvatarMetadata left, AvatarMetadata right)
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

            return obj.GetType() == GetType() && Equals((AvatarMetadata)obj);
        }

        /// <summary>
        /// Returns true if AvatarMetadata instances are equal.
        /// </summary>
        /// <param name="other">Instance of AvatarMetadata to be compared.</param>
        /// <returns>Boolean.</returns>
        public bool Equals(AvatarMetadata other)
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
                    AppId == other.AppId ||
                    AppId != null &&
                    AppId.Equals(other.AppId)
                ) &&
                (
                    Author == other.Author ||
                    Author != null &&
                    Author.Equals(other.Author)
                ) &&
                (
                    AvatarFormat == other.AvatarFormat ||
                    AvatarFormat != null &&
                    AvatarFormat.Equals(other.AvatarFormat)
                ) &&
                (
                    AvatarId == other.AvatarId ||
                    AvatarId != null &&
                    AvatarId.Equals(other.AvatarId)
                ) &&
                (
                    AvatarUrl == other.AvatarUrl ||
                    AvatarUrl != null &&
                    AvatarUrl.Equals(other.AvatarUrl)
                ) &&
                (
                    CreatedAt == other.CreatedAt ||
                    CreatedAt != null &&
                    CreatedAt.Equals(other.CreatedAt)
                ) &&
                (
                    Thumbnail == other.Thumbnail ||
                    Thumbnail != null &&
                    Thumbnail.Equals(other.Thumbnail)
                ) &&
                (
                    Type == other.Type ||
                    
                    Type.Equals(other.Type)
                ) &&
                (
                    UpdatedAt == other.UpdatedAt ||
                    UpdatedAt != null &&
                    UpdatedAt.Equals(other.UpdatedAt)
                ) &&
                (
                    Xrid == other.Xrid ||
                    Xrid != null &&
                    Xrid.Equals(other.Xrid)
                );
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>Hash code.</returns>
        public override int GetHashCode()
        {
            var hashCode = default(HashCode);

            hashCode.Add(AppId);
            hashCode.Add(Author);
            hashCode.Add(AvatarFormat);
            hashCode.Add(AvatarId);
            hashCode.Add(AvatarUrl);
            hashCode.Add(CreatedAt);
            hashCode.Add(Thumbnail);
            hashCode.Add(Type);
            hashCode.Add(UpdatedAt);
            hashCode.Add(Xrid);

            return hashCode.ToHashCode();
        }
    }
}
#pragma warning restore