using Cinemachine;
using DG.Tweening;
using UnityEngine;

namespace TPFive.Game.Reel.Camera
{
    public class DollyReelCamera : BaseReelCamera
    {
        [SerializeField]
        private CinemachineSmoothPath cinemachineSmoothPath;

        [SerializeField]
        private bool reverse;

        private Tween tween;

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
                Debug.LogError($"[{nameof(DollyReelCamera)}] virtualCamera is null");
                return;
            }

            var cinemachineTrackedDolly = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
            if (cinemachineTrackedDolly == null)
            {
                Debug.LogError($"[{nameof(DollyReelCamera)}] Doesn't have `CinemachineTrackedDolly` component");
                return;
            }

            // Get WayPoints Length
            float maxPathPosition = cinemachineSmoothPath.m_Waypoints.Length;
            float minPathPosition = 0.0f;

            // Set Default Path Position
            cinemachineTrackedDolly.m_PathPosition = reverse ? maxPathPosition : minPathPosition;
            float endValue = reverse ? minPathPosition : maxPathPosition;

            tween = DOTween.To(
                () => cinemachineTrackedDolly.m_PathPosition,
                x => cinemachineTrackedDolly.m_PathPosition = x,
                endValue,
                Duration)
                .SetAutoKill(false)
                .Pause();

            if (cinemachineSmoothPath.m_Looped)
            {
                tween.SetLoops(-1, LoopType.Restart);
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
