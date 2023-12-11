using UnityEngine;

namespace TPFive.Game.Messages
{
    /// <summary>
    /// message for notify avatar sit down.
    /// </summary>
    public sealed class AvatarSitDownMessage : AvatarMessageBase
    {
        public AvatarSitDownMessage(GameObject root, Pose sitPoint)
            : base(root)
        {
            SitPoint = sitPoint;
        }

        /// <summary>
        /// Gets the sit point.
        /// The ass of avatar will be aligned to this point.
        /// </summary>
        /// <value>sit point position and rotation.</value>
        public Pose SitPoint { get; }
    }
}
