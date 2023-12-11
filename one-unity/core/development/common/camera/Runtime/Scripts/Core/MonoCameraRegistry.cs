using System.Linq;
using Microsoft.Extensions.Logging;
using TPFive.Game.Camera;
using UnityEngine;
using VContainer;
using ICameraService = TPFive.Game.Camera.IService;

namespace TPFive.Extended.Camera
{
    /// <summary>
    /// Registers all camera instances(<see cref="TPFive.Extended.Camera.MonoCameraBase"/>)
    /// in camera service(<see cref="TPFive.Game.Camera.IService"/>) at <see cref="OnEnable"/>,
    /// unregisters them at <see cref="OnDisable"/>.
    /// </summary>
    public sealed class MonoCameraRegistry : MonoBehaviour
    {
        [SerializeField]
        private MonoCameraBase[] cameras;

        [Space]
        [SerializeField]
        [Tooltip("The camera to use. (must be in the \'cameras\' field)")]
        private MonoCameraBase defaultLiveCamera;

        private SimpleCameraRegistry cameraRegistry;
        private bool isRegistered;

        [Inject]
        public void Construct(ILoggerFactory loggerFactory, ICameraService cameraService)
        {
            cameraRegistry = new SimpleCameraRegistry(loggerFactory, cameraService);

            if (this.enabled && !isRegistered)
            {
                RegisterCamera();
            }
        }

        private void OnEnable()
        {
            if (cameraRegistry == null || isRegistered)
            {
                return;
            }

            RegisterCamera();
        }

        private void OnDisable()
        {
            if (cameraRegistry == null || !isRegistered)
            {
                return;
            }

            UnregisterCamera();
        }

        private void RegisterCamera()
        {
            isRegistered = true;

            foreach (var cam in cameras)
            {
                cameraRegistry.Register(cam);
            }

            if (defaultLiveCamera != null && cameras.Contains(defaultLiveCamera))
            {
                defaultLiveCamera.State.Value = CameraState.Live;
            }
        }

        private void UnregisterCamera()
        {
            isRegistered = false;

            if (defaultLiveCamera != null && cameras.Contains(defaultLiveCamera))
            {
                defaultLiveCamera.State.Value = CameraState.Standby;
            }

            foreach (var cam in cameras)
            {
                cameraRegistry.Unregister(cam);
            }
        }
    }
}
