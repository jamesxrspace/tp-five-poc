namespace TPFive.Game.Camera
{
    public interface ICameraRegistry
    {
        /// <summary>
        /// Register the camera into the <see cref="IService"/>.
        /// </summary>
        /// <param name="camera">want register camera object.</param>
        void Register(ICamera camera);

        /// <summary>
        /// Unregister the camera from the <see cref="IService"/>.
        /// </summary>
        /// <param name="camera">want unregister camera object.</param>
        void Unregister(ICamera camera);
    }
}
