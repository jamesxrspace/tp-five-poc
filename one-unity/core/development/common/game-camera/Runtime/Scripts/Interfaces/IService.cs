using UnityEngine;

namespace TPFive.Game.Camera
{
    public interface IService
    {
        /// <summary>
        /// Gets the null(default) service provider.
        /// </summary>
        /// <value>null service provider.</value>
        IServiceProvider NullServiceProvider { get; }

        /// <summary>
        /// Gets current live camera.
        /// </summary>
        /// <returns>camera object.</returns>
        ICamera GetLiveCamera();

        /// <summary>
        /// Turn the camera go to <see cref="CameraState.Live"/> state by name.
        /// </summary>
        /// <param name="cameraName">The name of camera that wants to change state.</param>
        void SetLiveCamera(string cameraName);

        /// <summary>
        /// Turn the current live camera go to <see cref="CameraState.Standby"/>.
        /// </summary>
        void UnsetLiveCamera();

        /// <summary>
        /// Add the camera into the service.
        /// Let the camera managed by service.
        /// </summary>
        /// <param name="camera">the camera object wants managed by service.</param>
        void AddCamera(ICamera camera);

        /// <summary>
        /// Remove the camera from the service.
        /// Let the camera not managed by service.
        /// </summary>
        /// <param name="camera">the camera object wants not managed by service.</param>
        void RemoveCamera(ICamera camera);

        /// <summary>
        /// Check if the camera is managed by service.
        /// </summary>
        /// <param name="camera">the camera object.</param>
        /// <returns>If TRUE means the camera in service, otherwise not.</returns>
        bool ContainsCamera(ICamera camera);

        /// <summary>
        /// Check if the camera is managed by service.
        /// </summary>
        /// <param name="cameraName">the name of camera.</param>
        /// <returns>If TRUE means the camera in service, otherwise not.</returns>
        bool ContainsCamera(string cameraName);

        /// <summary>
        /// Try to get the camera by name.
        /// </summary>
        /// <param name="cameraName">the name of camera.</param>
        /// <param name="camera">return found camera.</param>
        /// <returns>If TRUE means the camera found, otherwise not.</returns>
        bool TryGetCamera(string cameraName, out ICamera camera);

        /// <summary>
        /// Check if the target is in the view of the camera.
        /// </summary>
        /// <param name="target">the object wants to check in view.</param>
        /// <returns>If TRUE means the target object in view, otherwise not.</returns>
        bool IsTargetInView(Transform target);
    }

    public interface IServiceProvider : Game.IServiceProvider
    {
        /// <summary>
        /// Gets current live camera.
        /// </summary>
        /// <returns>camera object.</returns>
        ICamera GetLiveCamera();

        /// <summary>
        /// Turn the camera go to <see cref="CameraState.Live"/> state by name.
        /// </summary>
        /// <param name="cameraName">The name of camera that wants to change state.</param>
        void SetLiveCamera(string cameraName);

        /// <summary>
        /// Turn the current live camera go to <see cref="CameraState.Standby"/>.
        /// </summary>
        void UnsetLiveCamera();

        /// <summary>
        /// Add the camera into the service,
        /// let it managed by service,
        /// if it in live state,
        /// will send the view content of this to live.
        /// </summary>
        /// <param name="camera">the camera object wants managed by service.</param>
        void AddCamera(ICamera camera);

        /// <summary>
        /// Remove the camera from the service,
        /// let it not managed by service,
        /// and force set it to standby state.
        /// </summary>
        /// <param name="camera">the camera object wants not managed by service.</param>
        void RemoveCamera(ICamera camera);

        /// <summary>
        /// Check if the camera is managed by service.
        /// </summary>
        /// <param name="camera">the camera object.</param>
        /// <returns>If TRUE means the camera in service, otherwise not.</returns>
        bool ContainsCamera(ICamera camera);

        /// <summary>
        /// Check if the camera is managed by service.
        /// </summary>
        /// <param name="cameraName">the name of camera.</param>
        /// <returns>If TRUE means the camera in service, otherwise not.</returns>
        bool ContainsCamera(string cameraName);

        /// <summary>
        /// Try to get the camera by name.
        /// </summary>
        /// <param name="cameraName">the name of camera.</param>
        /// <param name="camera">return found camera.</param>
        /// <returns>If TRUE means the camera found, otherwise not.</returns>
        bool TryGetCamera(string cameraName, out ICamera camera);

        /// <summary>
        /// Check if the target is in the view of the camera.
        /// </summary>
        /// <param name="target">the object wants to check in view.</param>
        /// <returns>If TRUE means the target object in view, otherwise not.</returns>
        bool IsTargetInView(Transform target);
    }
}
