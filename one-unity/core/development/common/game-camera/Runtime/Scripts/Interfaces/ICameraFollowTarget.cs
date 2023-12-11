using UniRx;
using UnityEngine;

namespace TPFive.Game.Camera
{
    /// <summary>
    /// Provides the camera follow target.
    /// </summary>
    public interface ICameraFollowTarget
    {
        /// <summary>
        /// Gets the object that the camera wants to follow.
        /// </summary>
        /// <value>The object the camera wants to follow.</value>
        IReactiveProperty<Transform> FollowTarget { get; }
    }
}
