namespace TPFive.Game.ApplicationConfiguration
{
    /// <summary>
    /// GameServer information. (ReadOnly)
    /// </summary>
    public interface IReadOnlyGameServerInfo
    {
        /// <summary>
        /// Gets base uri of game server.
        /// </summary>
        /// <value>
        /// domain of game server
        /// </value>
        string BaseUri { get; }
    }
}