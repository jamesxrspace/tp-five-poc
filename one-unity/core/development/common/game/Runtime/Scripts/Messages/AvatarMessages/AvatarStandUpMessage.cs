using UnityEngine;

namespace TPFive.Game.Messages
{
    /// <summary>
    /// message for notify avatar stand up.
    /// </summary>
    public sealed class AvatarStandUpMessage : AvatarMessageBase
    {
        public AvatarStandUpMessage(GameObject root, Pose standPoint)
            : base(root)
        {
            StandPoint = standPoint;
        }

        /// <summary>
        /// Gets the stand point.
        /// The feet of avatar will be aligned to this point.
        /// </summary>
        /// <value>stand point position and rotation.</value>
        public Pose StandPoint { get; }
    }
}
