using System;
using UnityEngine;

namespace TPFive.Game.Record
{
    /// <summary>
    /// Reel scene description.
    /// </summary>
    [Serializable]
    public struct ReelSceneDesc : IEquatable<ReelSceneDesc>
    {
        /// <summary>
        /// Name of scene.
        /// </summary>
        public string Name;

        /// <summary>
        /// Addressable bundle id of scene asset.
        /// </summary>
        /// <seealso cref="BuildInAddressableKey"/>
        public string BundleID;

        /// <summary>
        /// Addressable key of build-in scene asset.
        /// </summary>
        /// <seealso cref="BundleID"/>
        public string BuildInAddressableKey;

        /// <summary>
        /// Template type of scene.
        /// </summary>
        public TemplateType Template;

        /// <summary>
        /// Get random track when record reel video.
        /// </summary>
        public bool RandomTrack;

        /// <summary>
        /// Camera track initial position.
        /// only works when <see cref="CameraTarget"/> is <see cref="CameraTargetType.FixedPosition"/>.
        /// </summary>
        public Vector3 FixedPosition;

        /// <summary>
        /// Camera Target Type of reel tracks' initial position.
        /// </summary>
        public CameraTargetType CameraTarget;

        public enum TemplateType
        {
            /// <summary>
            /// Indicates default template.
            ///
            /// standing pose, free space, free to move, rotate, zoom...etc.
            /// </summary>
            Default,

            /// <summary>
            /// Indicates carpool template.
            ///
            /// sitting pose, sitting chair, can't move, just few fixed chairs can switch.
            /// </summary>
            Carpool,

            /// <summary>
            /// Indicates bar template.
            ///
            /// standing pose, free space, free to move, rotate, zoom...etc.
            /// </summary>
            Bar,
        }

        public enum CameraTargetType
        {
            /// <summary>
            /// Set the initial camera target position of the reel track as the initial position of the user avatar.
            /// </summary>
            Avatar,

            /// <summary>
            /// Set the initial camera target position of the reel track to a fixed point in the scene.
            /// It also means that position of the camera in the record scene is fixed.
            /// </summary>
            FixedPosition,
        }

        public static ReelSceneDesc None => default;

        public readonly bool IsValid => !string.IsNullOrEmpty(Name) &&
                                        (!string.IsNullOrEmpty(BundleID) || !string.IsNullOrEmpty(BuildInAddressableKey));

        public readonly bool IsBundleAsset => !string.IsNullOrEmpty(BundleID);

        public readonly bool IsBuildInAsset => !string.IsNullOrEmpty(BuildInAddressableKey);

        public readonly string AssetKey => IsBundleAsset ? BundleID : BuildInAddressableKey;

        public static bool operator ==(ReelSceneDesc left, ReelSceneDesc right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ReelSceneDesc left, ReelSceneDesc right)
        {
            return !(left == right);
        }

        public readonly override bool Equals(object obj)
        {
            return obj is ReelSceneDesc desc && Equals(desc);
        }

        public readonly bool Equals(ReelSceneDesc other)
        {
            return Name == other.Name &&
                   BundleID == other.BundleID &&
                   BuildInAddressableKey == other.BuildInAddressableKey &&
                   Template == other.Template &&
                   RandomTrack == other.RandomTrack &&
                   FixedPosition == other.FixedPosition &&
                   CameraTarget == other.CameraTarget;
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(Name, BundleID, BuildInAddressableKey, Template, RandomTrack, FixedPosition, CameraTarget);
        }

        public readonly override string ToString()
        {
            return $"{{ {nameof(Name)}: {Name}, {nameof(BundleID)}: {BundleID}, {nameof(BuildInAddressableKey)}: {BuildInAddressableKey}, {nameof(Template)}: {Template}, {nameof(RandomTrack)}: {RandomTrack}, {nameof(FixedPosition)}: {FixedPosition}, {nameof(CameraTarget)}: {CameraTarget} }}";
        }
    }
}
