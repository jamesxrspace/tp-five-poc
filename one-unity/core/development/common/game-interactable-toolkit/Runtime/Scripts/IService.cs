using DG.Tweening;
using UnityEngine;

namespace TPFive.Game.Interactable.Toolkit
{
    /// <summary>
    /// Game.Interactable.Toolkit.IService is define the interactable service api for the game.
    /// </summary>
    public interface IService
    {
        /// <summary>
        /// Gets NullServiceProvider is according to the Service architecture of the project,
        /// NullServiceProvider needs to be provided to avoid NullException.
        /// </summary>
        IServiceProvider NullServiceProvider { get; }

        /// <summary>
        /// TrySnapObject is a delegate for the game to call the service to snap the object.
        /// </summary>
        /// <param name="other">The collided target.</param>
        /// <param name="animation">Your custom DOTweenAnimation.</param>
        /// <returns>if this collider is a interactable object which can be snapped.</returns>
        bool TrySnapObject(Collider other, DOTweenAnimation animation);

        /// <summary>
        /// TryReleaseSnappedObject is a delegate for the game to call the service to release the snapped object.
        /// </summary>
        /// <param name="other">The collided target which you want to release.</param>
        /// <returns>if this collider is the snapped object.</returns>
        bool TryReleaseSnappedObject(Collider other);
    }
}