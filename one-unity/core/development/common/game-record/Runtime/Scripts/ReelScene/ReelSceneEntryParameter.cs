using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TPFive.OpenApi.GameServer.Model;

namespace TPFive.Game.Record
{
    /// <summary>
    /// Parameter for entry reel scene.
    /// </summary>
    [Serializable]
    public sealed class ReelSceneEntryParameter : IEquatable<ReelSceneEntryParameter>
    {
#pragma warning disable SA1401 // Fields should be private
        /// <summary>
        /// Entry type.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public EntryType Entry;

        /// <summary>
        /// Reel url. (e.g. https://***.xrs).
        /// </summary>
        public string ReelUrl;

        public Reel Reel;

        /// <summary>
        /// Scene description.
        /// </summary>
        public ReelSceneDesc SceneDesc;
#pragma warning restore SA1401

        public enum EntryType
        {
            /// <summary>
            /// In the case of browsing reel.
            /// </summary>
            Browse,

            /// <summary>
            /// In the case of creating reel.
            /// </summary>
            Create,
        }

        public static bool operator ==(ReelSceneEntryParameter left, ReelSceneEntryParameter right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ReelSceneEntryParameter left, ReelSceneEntryParameter right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || (obj is ReelSceneEntryParameter other && Equals(other));
        }

        public bool Equals(ReelSceneEntryParameter other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Entry == other.Entry && ReelUrl == other.ReelUrl && SceneDesc.Equals(other.SceneDesc);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int)Entry, ReelUrl, SceneDesc);
        }

        public override string ToString()
        {
            return $"{nameof(Entry)}: {Entry}, {nameof(ReelUrl)}: {ReelUrl}, {nameof(SceneDesc)}: {SceneDesc}";
        }
    }
}