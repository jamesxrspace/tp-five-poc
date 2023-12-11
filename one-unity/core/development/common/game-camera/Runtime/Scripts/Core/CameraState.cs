namespace TPFive.Game.Camera
{
    /// <summary>
    /// Indicates the state of camera.
    /// </summary>
    public enum CameraState
    {
        /// <summary>
        /// Indicates the camera is in standby state,
        /// means the camera is not live.
        /// </summary>
        Standby,

        /// <summary>
        /// Indicates the camera is in live state,
        /// means the camera is live.
        /// </summary>
        Live,
    }
}
