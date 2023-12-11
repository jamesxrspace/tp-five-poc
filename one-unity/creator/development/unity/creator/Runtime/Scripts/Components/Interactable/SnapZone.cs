using DG.Tweening;
using UnityEngine;

namespace TPFive.Creator.Components.Interactable
{
    /// <summary>
    /// Snap zone for controllable objects.
    /// 1. Use custom DoTweenAnimation to control animation of the snap object (etc. Move...).
    /// 2. When you release the Controllable object and remain within the trigger bounds,
    /// it will automatically snap to the designated transform point.
    /// </summary>
    public class SnapZone : ComponentBase
    {
        [SerializeField]
        private DOTweenAnimation tweenAnimation;

        /// <summary>
        /// When you release the Controllable object and remain within the trigger bounds,
        /// it will automatically snap to the designated transform point.
        /// </summary>
        /// <param name="other">The collider which stay the trigger bounds.</param>
        protected void OnTriggerStay(Collider other)
        {
            TrySnapObject(other, tweenAnimation);
        }

        /// <summary>
        /// When the hooked target is out of the trigger bounds, then clear the hooked target.
        /// </summary>
        /// <param name="other">The collider which exit the trigger bounds.</param>
        protected void OnTriggerExit(Collider other)
        {
            TryReleaseSnappedObject(other);
        }

        private bool TrySnapObject(Collider other, DG.Tweening.DOTweenAnimation animation)
        {
            var result = Cross.Bridge.TrySnapObject?.Invoke(other, animation);
            return result.HasValue && result.Value;
        }

        private bool TryReleaseSnappedObject(Collider other)
        {
            var result = Cross.Bridge.TryReleaseSnappedObject?.Invoke(other);
            return result.HasValue && result.Value;
        }
    }
}