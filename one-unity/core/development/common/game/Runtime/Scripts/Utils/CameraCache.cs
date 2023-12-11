using System;
using UnityEngine;

namespace TPFive.Game.Utils
{
    /// <summary>
    /// A cache for the main camera.
    /// </summary>
    public static class CameraCache
    {
        private static Camera _mainCamera;

        public static event Action<Camera> OnMainCameraChanged;

        public static Camera Main
        {
            get
            {
                if (_mainCamera == null)
                {
                    UpdateMainCamera(Camera.main);
                }

                return _mainCamera;
            }
        }

        public static void UpdateMainCamera(Camera newCamera)
        {
            if (_mainCamera == newCamera)
            {
                return;
            }

            _mainCamera = newCamera;
            if (!_mainCamera.CompareTag("MainCamera"))
            {
                _mainCamera.tag = "MainCamera";
            }

            OnMainCameraChanged?.Invoke(_mainCamera);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ClearLegacyReferences()
        {
            _mainCamera = null;
            OnMainCameraChanged = null;
        }
    }
}
