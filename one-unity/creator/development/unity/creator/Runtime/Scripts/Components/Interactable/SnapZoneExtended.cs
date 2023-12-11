using DG.Tweening;
using TPFive.Game.Interactable.Toolkit;
using UnityEngine;

namespace TPFive.Creator.Components.Interactable
{
    /// <summary>
    /// Snap zone for controllable objects.
    /// 1. Use custom DoTweenAnimation to control animation of the snap object (etc. Move...).
    /// 2. When you release the Controllable object and remain within the trigger bounds,
    /// it will automatically snap to the designated transform point.
    /// </summary>
    public class SnapZoneExtended : IServiceProvider
    {
        private Transform _snappedTarget;

        public bool TrySnapObject(Collider other, DOTweenAnimation animation)
        {
            if (!other.TryGetComponent<Controllable>(out var controllable))
            {
                return false;
            }

            // Until someone+ release it controlled object then try to snap it.
            if (controllable.IsControlled)
            {
                return false;
            }

            var newTarget = other.transform;

            if (_snappedTarget == newTarget)
            {
                return false;
            }

            _snappedTarget = newTarget;

            PlayAnimation(newTarget, animation);

            return true;
        }

        public bool TryReleaseSnappedObject(Collider other)
        {
            if (_snappedTarget == other.transform)
            {
                _snappedTarget = null;
                return true;
            }

            return false;
        }

        private void PlayAnimation(Transform target, DOTweenAnimation animation)
        {
            if (target == null)
            {
                return;
            }

            animation.SetAnimationTarget(target);

            // Animation must recreate tween to apply target.
            // Otherwise, it animation will return to the initial (position, rotation...) established last time.
            animation.RecreateTweenAndPlay();
        }
    }
}