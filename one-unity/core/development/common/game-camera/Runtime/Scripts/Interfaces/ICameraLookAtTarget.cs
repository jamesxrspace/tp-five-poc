using UniRx;
using UnityEngine;

namespace TPFive.Game.Camera
{
    /// <summary>
    /// Provides the camera look at target.
    /// </summary>
    public interface ICameraLookAtTarget
    {
        /// <summary>
        /// Gets the object that the camera wants to look at.
        /// </summary>
        /// <value>The object that the camera wants to look at.</value>
        IReactiveProperty<Transform> LookAtTarget { get; }
    }
}
