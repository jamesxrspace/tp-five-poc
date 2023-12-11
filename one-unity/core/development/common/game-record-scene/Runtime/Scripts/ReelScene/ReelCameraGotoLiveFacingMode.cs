namespace TPFive.Game.Record.Scene
{
    /// <summary>
    /// How to face the target when camera goto live.
    /// </summary>
    public enum ReelCameraGotoLiveFacingMode
    {
        /// <summary>
        /// Do nothing or do it automatically through the camera.
        /// </summary>
        NoneOrAuto,

        /// <summary>
        /// Align previous camera place when camera goto live.
        /// </summary>
        AlignPreviousCameraPlace,

        /// <summary>
        /// Face to target when camera goto live.
        /// The target usually refers to an avatar or an object in the scene.
        /// </summary>
        FaceToTarget,
    }
}
