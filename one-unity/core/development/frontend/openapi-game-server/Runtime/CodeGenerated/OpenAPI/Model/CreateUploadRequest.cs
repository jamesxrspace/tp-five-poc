/*
 * Server API - Asset
 *
 * The Restful APIs of Asset.
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
    public class CreateUploadRequest : IEquatable<CreateUploadRequest>
    {
        /// <summary>
        /// upload asset tags
        /// </summary>
        /// <value>upload asset tags</value>
        [JsonProperty(PropertyName = "tags")]
        public List<string> Tags { get; set; }

        /// <summary>
        /// upload asset type
        /// </summary>
        /// <value>upload asset type</value>
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        /// <summary>
        /// upload asset categories
        /// </summary>
        /// <value>upload asset categories</value>
        [JsonProperty(PropertyName = "categories")]
        public List<CategoriesEnum> Categories { get; set; }

        /// <summary>
        /// upload files information [Required]
        /// </summary>
        /// <value>upload files information</value>
        [JsonProperty(PropertyName = "files")]
        public List<UploadFile> Files { get; set; }

        public static bool operator ==(CreateUploadRequest left, CreateUploadRequest right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CreateUploadRequest left, CreateUploadRequest right)
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

            return obj.GetType() == GetType() && Equals((CreateUploadRequest)obj);
        }

        /// <summary>
        /// Returns true if CreateUploadRequest instances are equal.
        /// </summary>
        /// <param name="other">Instance of CreateUploadRequest to be compared.</param>
        /// <returns>Boolean.</returns>
        public bool Equals(CreateUploadRequest other)
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
                    Tags == other.Tags ||
                    Tags != null &&
                    other.Tags != null &&
                    Tags.SequenceEqual(other.Tags)
                ) && 
                (
                    Type == other.Type ||
                    Type != null &&
                    Type.Equals(other.Type)
                ) &&
                (
                    Categories == other.Categories ||
                    Categories != null &&
                    other.Categories != null &&
                    Categories.SequenceEqual(other.Categories)
                ) && 
                (
                    Files == other.Files ||
                    Files != null &&
                    other.Files != null &&
                    Files.SequenceEqual(other.Files)
                );
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>Hash code.</returns>
        public override int GetHashCode()
        {
            var hashCode = default(HashCode);

            hashCode.Add(Tags);
            hashCode.Add(Type);
            hashCode.Add(Categories);
            hashCode.Add(Files);

            return hashCode.ToHashCode();
        }
    }
}
#pragma warning restore