/*
 * Server API - Reel
 *
 * The Restful APIs of Reel.
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
    public class CreateReelRequest : IEquatable<CreateReelRequest>
    {
        /// <summary>
        /// reel description
        /// </summary>
        /// <value>reel description</value>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// reel thumbnail url [Required]
        /// </summary>
        /// <value>reel thumbnail url</value>
        [JsonProperty(PropertyName = "thumbnail")]
        public string Thumbnail { get; set; }

        /// <summary>
        /// video url [Required]
        /// </summary>
        /// <value>video url</value>
        [JsonProperty(PropertyName = "video")]
        public string Video { get; set; }

        /// <summary>
        /// music to motion url
        /// </summary>
        /// <value>music to motion url</value>
        [JsonProperty(PropertyName = "music_to_motion_url")]
        public string MusicToMotionUrl { get; set; }

        /// <summary>
        /// xrs url [Required]
        /// </summary>
        /// <value>xrs url</value>
        [JsonProperty(PropertyName = "xrs")]
        public string Xrs { get; set; }

        /// <summary>
        /// parent reel id
        /// </summary>
        /// <value>parent reel id</value>
        [JsonProperty(PropertyName = "parent_reel_id")]
        public string ParentReelId { get; set; }

        /// <summary>
        /// categories of reel belonging feed [Required]
        /// </summary>
        /// <value>categories of reel belonging feed</value>
        [JsonProperty(PropertyName = "categories")]
        public List<CategoriesEnum> Categories { get; set; }

        /// <summary>
        /// Gets or Sets JoinMode [Required]
        /// </summary>
        [JsonProperty(PropertyName = "join_mode")]
        public JoinModeEnum JoinMode { get; set; }

        public static bool operator ==(CreateReelRequest left, CreateReelRequest right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CreateReelRequest left, CreateReelRequest right)
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

            return obj.GetType() == GetType() && Equals((CreateReelRequest)obj);
        }

        /// <summary>
        /// Returns true if CreateReelRequest instances are equal.
        /// </summary>
        /// <param name="other">Instance of CreateReelRequest to be compared.</param>
        /// <returns>Boolean.</returns>
        public bool Equals(CreateReelRequest other)
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
                    Video == other.Video ||
                    Video != null &&
                    Video.Equals(other.Video)
                ) &&
                (
                    MusicToMotionUrl == other.MusicToMotionUrl ||
                    MusicToMotionUrl != null &&
                    MusicToMotionUrl.Equals(other.MusicToMotionUrl)
                ) &&
                (
                    Xrs == other.Xrs ||
                    Xrs != null &&
                    Xrs.Equals(other.Xrs)
                ) &&
                (
                    ParentReelId == other.ParentReelId ||
                    ParentReelId != null &&
                    ParentReelId.Equals(other.ParentReelId)
                ) &&
                (
                    Categories == other.Categories ||
                    Categories != null &&
                    other.Categories != null &&
                    Categories.SequenceEqual(other.Categories)
                ) && 
                (
                    JoinMode == other.JoinMode ||
                    
                    JoinMode.Equals(other.JoinMode)
                );
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>Hash code.</returns>
        public override int GetHashCode()
        {
            var hashCode = default(HashCode);

            hashCode.Add(Description);
            hashCode.Add(Thumbnail);
            hashCode.Add(Video);
            hashCode.Add(MusicToMotionUrl);
            hashCode.Add(Xrs);
            hashCode.Add(ParentReelId);
            hashCode.Add(Categories);
            hashCode.Add(JoinMode);

            return hashCode.ToHashCode();
        }
    }
}
#pragma warning restore