namespace TPFive.Game.Record.Scene
{
    public enum ReelCameraTargetType
    {
        /// <summary>
        /// Set the initial camera target position of the reel track as the initial position of the user avatar.
        /// </summary>
        Avatar,

        /// <summary>
        /// Set the initial camera target position of the reel track to a fixed point in the scene.
        /// It also means that position of the camera in the record scene is fixed.
        /// </summary>
        FixedPosition,
    }
}
