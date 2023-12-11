using System;
using TPFive.Extended.Animancer;
using UnityEngine;

namespace TPFive.Game.Record.Scene.SpawnPoint
{
    /// <summary>
    /// Description of a spawn point.
    /// </summary>
    [Serializable]
    public struct PointDesc : IEquatable<PointDesc>
    {
        [SerializeField]
        [Tooltip("The point you want to align.")]
        private Transform point;

        [SerializeField]
        [Tooltip("The pose you want to show when landing this point.")]
        private LandingPoseType landingPose;

        [SerializeField]
        [Tooltip("The specific animation you want to play when landing this point.")]
        private CommonTransitionDataGenderGroup specificAnimationGroup;

        public PointDesc(Transform point, LandingPoseType landingPose)
            : this(point, landingPose, null)
        {
        }

        public PointDesc(
            Transform point,
            LandingPoseType landingPose,
            CommonTransitionDataGenderGroup specificAnimationGroup)
        {
            this.point = point;
            this.landingPose = landingPose;
            this.specificAnimationGroup = specificAnimationGroup;
        }

        public static PointDesc None => new PointDesc(null, LandingPoseType.Standing);

        public readonly Transform Point => point;

        public readonly LandingPoseType LandingPose => landingPose;

        public readonly CommonTransitionDataGenderGroup SpecificAnimationGroup => specificAnimationGroup;

        public static bool operator ==(PointDesc left, PointDesc right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PointDesc left, PointDesc right)
        {
            return !(left == right);
        }

        public bool TryGetSpecificAnimationByGender(int gender, out CommonTransitionData specificAnimation)
        {
            specificAnimation = null;

            if (specificAnimationGroup == null)
            {
                return false;
            }

            return specificAnimationGroup.TryGetData(gender, out specificAnimation);
        }

        public readonly override bool Equals(object obj)
        {
            return obj is PointDesc other && Equals(other);
        }

        public readonly bool Equals(PointDesc other)
        {
            return point == other.point &&
                   landingPose == other.landingPose &&
                   ReferenceEquals(specificAnimationGroup, other.specificAnimationGroup);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(point, landingPose);
        }

        public readonly override string ToString()
        {
            string specificAnimationGroupName = specificAnimationGroup != null ? specificAnimationGroup.name : "<none>";

            return point == null
                ? $"{{ Point: {{ Position: NA, Rotation: NA }}, Pose: {landingPose}, SpecificAnimationGroup: {specificAnimationGroupName} }}"
                : $"{{ Point: {{ Position: {point.position.ToString("F3")}, Rotation: {point.eulerAngles.ToString("F3")} }}, Pose: {landingPose}, SpecificAnimationGroup: {specificAnimationGroupName} }}";
        }
    }
}
