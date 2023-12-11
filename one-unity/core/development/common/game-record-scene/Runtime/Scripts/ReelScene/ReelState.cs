namespace TPFive.Game.Record.Scene
{
    /// <summary>
    /// The state of reel.
    /// </summary>
    public enum ReelState
    {
        /// <summary>
        /// Watch reel.
        /// </summary>
        Watch,

        /// <summary>
        /// Prepare camera placement or other things for recording reels.
        /// </summary>
        Prepare,

        /// <summary>
        /// standby for recording reels.
        /// </summary>
        Standby,

        /// <summary>
        /// Recording reel.
        /// </summary>
        Recording,

        /// <summary>
        /// Preview that just recorded reel.
        /// </summary>
        Preview,
    }
}
