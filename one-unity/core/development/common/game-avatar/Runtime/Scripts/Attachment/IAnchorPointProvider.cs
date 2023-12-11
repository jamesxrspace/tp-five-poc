using UnityEngine;

namespace TPFive.Game.Avatar.Attachment
{
    /// <summary>
    /// This avatar attachment is defined the body parts from an avatar.
    /// </summary>
    public interface IAnchorPointProvider
    {
        /// <summary>
        /// Gets the avatar custom body party transform by <see cref="AnchorPointType"/>.
        /// </summary>
        /// <param name="anchorType"> The custom enum of body party name. </param>
        /// <param name="anchorTransform">The custom transform of body party.</param>
        /// <returns> return the boolean whether get the transform or not. </returns>
        bool TryGetAnchorPoint(AnchorPointType anchorType, out Transform anchorTransform);
    }
}