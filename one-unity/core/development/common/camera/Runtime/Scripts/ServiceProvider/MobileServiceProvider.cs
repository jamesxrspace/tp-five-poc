using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using TPFive.Game;
using TPFive.Game.Camera;
using TPFive.Game.Utils;
using TPFive.SCG.DisposePattern.Abstractions;
using UniRx;
using UnityEngine;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Extended.Camera
{
    /// <summary>
    /// Mobile camera service provider.
    /// </summary>
    [Dispose]
    public sealed partial class MobileServiceProvider : TPFive.Game.Camera.IServiceProvider
    {
        private readonly ILogger log;

        /// <summary>
        /// camera dictionary.
        /// </summary>
        /// <value>Key is <see cref="ICamera.Name"/>; Value is <see cref="ICamera"/>.</value>
        private readonly Dictionary<string, ICamera> cameraDict = new Dictionary<string, ICamera>();

        private readonly Dictionary<ICamera, System.IDisposable> cameraReactiveObserverDict = new Dictionary<ICamera, System.IDisposable>();

        private UnityEngine.Camera renderCamera;

        /// <summary>
        /// The original culling mask of render camera.
        /// Default value is 0 means culling nothing.
        /// </summary>
        private int originalCullingMask = 0;

        private ICamera currentLiveCamera;
        private CompositeDisposable currentLiveCameraSubscriptions;

        public MobileServiceProvider(ILoggerFactory loggerFactory)
        {
            log = loggerFactory.CreateLogger<MobileServiceProvider>();

            OnMainCameraChanged(CameraCache.Main);
            CameraCache.OnMainCameraChanged += OnMainCameraChanged;
        }

        public ICamera GetLiveCamera()
        {
            return currentLiveCamera;
        }

        public void SetLiveCamera(string cameraName)
        {
            if (!cameraDict.TryGetValue(cameraName, out ICamera camera))
            {
                throw new System.InvalidOperationException($"The camera({cameraName}) isn't in service. Please add it first.");
            }

            if (!camera.IsAlive())
            {
                log.LogError("{Method}: camera({CameraName}) isn't alive", nameof(SetLiveCamera), cameraName);
                return;
            }

            camera.State.Value = CameraState.Live;
        }

        public void UnsetLiveCamera()
        {
            if (!currentLiveCamera.IsAlive())
            {
                log.LogError("{Method}: live camera isn't alive", nameof(UnsetLiveCamera));
                return;
            }

            currentLiveCamera.State.Value = CameraState.Standby;
        }

        public void AddCamera(ICamera camera)
        {
            if (!camera.IsAlive())
            {
                throw new System.ArgumentNullException(nameof(camera), "camera isn't alive");
            }

            if (ContainsCamera(camera))
            {
                log.LogWarning(
                    "{Method}: The camera({CameraName}) is already in service",
                    nameof(AddCamera),
                    camera.Name);

                return;
            }

            cameraDict.Add(camera.Name, camera);

            var observer = new CameraReactiveObserver<CameraState>(camera, camera.State, OnCameraStateChanged);
            cameraReactiveObserverDict.Add(camera, observer);

            OnCameraStateChanged(camera, camera.State.Value);
        }

        public void RemoveCamera(ICamera camera)
        {
            if (!camera.IsAlive())
            {
                throw new System.ArgumentNullException(nameof(camera), "camera isn't alive");
            }

            if (!ContainsCamera(camera))
            {
                log.LogWarning(
                    "{Method}: The camera({CameraName}) isn't in service",
                    nameof(RemoveCamera),
                    camera.Name);

                return;
            }

            // Turn off camera
            if (camera.State.Value == CameraState.Live)
            {
                camera.State.Value = CameraState.Standby;
            }

            cameraDict.Remove(camera.Name);

            if (!cameraReactiveObserverDict.TryGetValue(camera, out var observer))
            {
                return;
            }

            observer?.Dispose();
            cameraReactiveObserverDict.Remove(camera);
        }

        public bool ContainsCamera(ICamera camera)
        {
            return cameraDict.ContainsKey(camera.Name);
        }

        public bool ContainsCamera(string cameraName)
        {
            return cameraDict.ContainsKey(cameraName);
        }

        public bool TryGetCamera(string cameraName, out ICamera camera)
        {
            return cameraDict.TryGetValue(cameraName, out camera);
        }

        public bool IsTargetInView(Transform target)
        {
            if (renderCamera == null)
            {
                log.LogWarning(
                    "{Method}: The render camera is null. Please tag 'MainCamera' to a 'Camera'",
                    nameof(IsTargetInView));

                return false;
            }

            if (target == null)
            {
                log.LogWarning(
                    "{Method}: The target is null",
                    nameof(IsTargetInView));

                return false;
            }

            Vector3 targetViewportPosition = renderCamera.WorldToViewportPoint(target.position);

            // return true if the target is in the viewport,
            // z should be greater than 0 otherwise it's behind the camera.
            return targetViewportPosition.x >= 0f &&
                   targetViewportPosition.x <= 1f &&
                   targetViewportPosition.y >= 0f &&
                   targetViewportPosition.y <= 1f &&
                   targetViewportPosition.z > 0f;
        }

        /// <summary>
        /// Called when the main camera changed.
        /// </summary>
        /// <param name="newMainCamera">new main camera.</param>
        /// <seealso cref="CameraCache.OnMainCameraChanged"/>
        private void OnMainCameraChanged(UnityEngine.Camera newMainCamera)
        {
            renderCamera = newMainCamera;
            originalCullingMask = (renderCamera != null) ? renderCamera.cullingMask : 0;

            ApplyLiveCameraCullingMaskIfNeed();
        }

        private void OnCameraStateChanged(ICamera camera, CameraState state)
        {
            if (!camera.IsAlive())
            {
                log.LogError("{Method}: camera isn't alive", nameof(OnCameraStateChanged));

                return;
            }

            log.LogInformation(
                "{Method}: The camera({CameraName}) goto {CameraState} state",
                nameof(OnCameraStateChanged),
                camera.Name,
                state);

            if ((state == CameraState.Live && camera == currentLiveCamera) ||
                (state == CameraState.Standby && camera != currentLiveCamera))
            {
                log.LogDebug(
                    "{Method}: The camera({CameraName}) is already in {CameraState} state",
                    nameof(OnCameraStateChanged),
                    camera.Name,
                    state);

                return;
            }

            // Unsubscribe previous live camera's reactive properties.
            currentLiveCameraSubscriptions?.Dispose();
            currentLiveCameraSubscriptions = new CompositeDisposable();

            if (state == CameraState.Standby)
            {
                // Clear current live camera
                currentLiveCamera = null;

                ApplyLiveCameraCullingMaskIfNeed();
                return;
            }

            // Swap current live camera.
            var previousLiveCamera = currentLiveCamera;
            currentLiveCamera = camera;

            // Turn off previous live camera.
            if (previousLiveCamera.IsAlive())
            {
                previousLiveCamera.State.Value = CameraState.Standby;
            }

            // Subscribe current live camera's reactive properties.
            // Don't need to call <see cref="ApplyLiveCameraCullingMaskIfNeed"/> here,
            // because <see cref="UniRx.ReactiveProperty<>"/> will notify latest value when subscribed.
            currentLiveCamera
                .CullingMaskOverride
                .Subscribe(OnLiveCameraCullingMaskChanged)
                .AddTo(currentLiveCameraSubscriptions);
        }

        /// <summary>
        /// Called when the live camera's culling mask changed.
        /// </summary>
        /// <param name="cullingMask">live camera culling mask.</param>
        private void OnLiveCameraCullingMaskChanged(int? cullingMask)
        {
            ApplyLiveCameraCullingMaskIfNeed();
        }

        /// <summary>
        /// Apply live camera's culling mask to render camera if need.
        /// </summary>
        private void ApplyLiveCameraCullingMaskIfNeed()
        {
            if (renderCamera == null)
            {
                log.LogWarning(
                    "{Method}: The render camera is null. Please tag 'MainCamera' to a 'Camera'",
                    nameof(ApplyLiveCameraCullingMaskIfNeed));

                return;
            }

            if (!currentLiveCamera.IsAlive() || currentLiveCamera.CullingMaskOverride.Value == null)
            {
                renderCamera.cullingMask = originalCullingMask;

                log.LogDebug(
                    "{Method}: Apply original culling mask({CullingMask})",
                    nameof(ApplyLiveCameraCullingMaskIfNeed),
                    originalCullingMask);

                return;
            }

            int newCullingMask = currentLiveCamera.CullingMaskOverride.Value.Value;
            renderCamera.cullingMask = newCullingMask;

            log.LogDebug(
                "{Method}: Apply override culling mask({CullingMask})",
                nameof(ApplyLiveCameraCullingMaskIfNeed),
                newCullingMask);
        }

        /// <summary>
        /// Handle IDisposable interface.
        /// </summary>
        /// <param name="disposing">disposing or not.</param>
        /// <seealso cref="Dispose"/>
        private void HandleDispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                cameraDict.Clear();
                currentLiveCameraSubscriptions?.Dispose();
                CameraCache.OnMainCameraChanged -= OnMainCameraChanged;

                foreach (var observer in cameraReactiveObserverDict.Values)
                {
                    observer?.Dispose();
                }

                cameraReactiveObserverDict.Clear();
            }

            _disposed = true;
        }
    }
}
