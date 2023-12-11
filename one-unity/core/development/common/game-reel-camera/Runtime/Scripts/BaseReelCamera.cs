using Cinemachine;
using UnityEngine;

namespace TPFive.Game.Reel.Camera
{
    public abstract class BaseReelCamera : MonoBehaviour
    {
#pragma warning disable SA1401
        [SerializeField]
        protected float duration = 30f; // max play seconds

        [SerializeField]
        protected CinemachineVirtualCameraBase virtualCamera;

        [SerializeField]
        protected bool setFollow;

        [SerializeField]
        protected bool setLookAt;
#pragma warning restore SA1401

        public float Duration => duration;

        public CinemachineVirtualCameraBase VirtualCamera => virtualCamera;

        public virtual void Play(Transform target = null)
        {
            if (virtualCamera == null)
            {
                throw new System.Exception("VirtualCamera is null");
            }

            if (setFollow)
            {
                virtualCamera.Follow = target;
            }

            if (setLookAt)
            {
                virtualCamera.LookAt = target;
            }
        }
    }
}
