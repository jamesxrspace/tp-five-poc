namespace TPFive.Game.RealtimeChat
{
    public interface IParticipant
    {
        uint Uid { get; }

        string XRSocialId { get; }

        /// <summary>
        /// Gets the ChannelSession that this participant joined.
        /// </summary>
        /// <value>
        /// The Channel.
        /// </value>
        IChannel Channel { get; }
    }
}
