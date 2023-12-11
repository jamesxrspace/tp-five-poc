using System;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

namespace TPFive.Game.Reel.Camera
{
    public class ZoomReelCamera : BaseReelCamera
    {
        [SerializeField]
        private float maxFov = 10.0f;

        [SerializeField]
        private float minFov = 70.0f;

        [SerializeField]
        private ZoomType zoomType;

        [SerializeField]
        private bool loop;

        private Tween tween;

        private enum ZoomType
        {
            ZoomIn,
            ZoomOut,
        }

        public override void Play(Transform target)
        {
            base.Play(target);

            tween.Restart();
        }

        protected void Awake()
        {
            CinemachineVirtualCamera cinemachineVirtualCamera = virtualCamera as CinemachineVirtualCamera;

            if (cinemachineVirtualCamera == null)
            {
                Debug.LogError($"[{nameof(ZoomReelCamera)}] VirtualCamera is null");
                return;
            }

            var (startValue, endValue) = zoomType switch
            {
                ZoomType.ZoomIn => (minFov, maxFov),
                ZoomType.ZoomOut => (maxFov, minFov),
                _ => throw new ArgumentOutOfRangeException()
            };

            cinemachineVirtualCamera.m_Lens.FieldOfView = startValue;

            tween = DOTween.To(
                () => cinemachineVirtualCamera.m_Lens.FieldOfView,
                x => cinemachineVirtualCamera.m_Lens.FieldOfView = x,
                endValue,
                Duration)
                .SetEase(Ease.Linear)
                .Pause();

            if (loop)
            {
                tween.SetLoops(-1, LoopType.Yoyo);
            }
        }

        protected void OnDestroy()
        {
            if (tween == null)
            {
                return;
            }

            DOTween.Kill(tween);
        }

#if UNITY_EDITOR
        protected void OnValidate()
        {
            if (virtualCamera != null && virtualCamera is not CinemachineVirtualCamera)
            {
                Debug.LogError("virtualCamera must be CinemachineVirtualCamera");
                virtualCamera = null;
            }
        }
#endif

        [ContextMenu("Play On Editor")]
        private void PlayOnEditor()
        {
            Play(virtualCamera.Follow);
        }
    }
}
