using TPFive.Game.Utils;
using UnityEngine;

namespace TPFive.Home.Entry.SocialLobby
{
    public sealed class CameraManager : MonoBehaviour
    {
        [SerializeField]
        private Camera customCamera;
        [SerializeField]
        [Tooltip("0 = no buffer, 1 = full screen")]
        private Vector2 visibleBufferSize;
        private Camera _previousMainCamera;

        public bool IsEnableCustomCamera => customCamera != null && CameraCache.Main == customCamera;

        public Camera CustomCamera => customCamera;

        public void EnableCustomCamera(bool isEnable)
        {
            if (customCamera == null)
            {
                return;
            }

            if (_previousMainCamera == null)
            {
                _previousMainCamera = CameraCache.Main;
            }

            CameraCache.UpdateMainCamera(isEnable ? customCamera : _previousMainCamera);
            _previousMainCamera.enabled = !isEnable;
            customCamera.enabled = isEnable;
        }

        public bool IsVisible(Vector3 worldPosition, Vector3 entitySize)
        {
            if (customCamera == null)
            {
                return false;
            }

            var minX = 0 - visibleBufferSize.x;
            var maxX = 1 + visibleBufferSize.x;
            var minY = 0 - visibleBufferSize.y;
            var maxY = 1 + visibleBufferSize.y;
            var screenPoint = customCamera.WorldToViewportPoint(worldPosition);
            return minX <= screenPoint.x && screenPoint.x <= maxX && minY <= screenPoint.y && screenPoint.y <= maxY;
        }

        private void OnDestroy()
        {
            if (_previousMainCamera != null)
            {
                CameraCache.UpdateMainCamera(_previousMainCamera);
            }
        }
    }
}