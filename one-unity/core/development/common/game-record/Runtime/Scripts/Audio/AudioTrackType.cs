namespace TPFive.Game.Record
{
    public enum AudioTrackType
    {
        /// <summary>
        /// Represents the background music audio track type.
        /// </summary>
        BGM,

        /// <summary>
        /// Represents an audio track type for sound effects.
        /// </summary>
        Effect,

        /// <summary>
        /// Represents an audio track type for voice audio.)
        /// </summary>
        Voice,

        /// <summary>
        /// Represents an audio track type for microphone audio.
        /// After recording microphone audio, it will be converted to voice audio.
        /// </summary>
        Mic,
    }
}
