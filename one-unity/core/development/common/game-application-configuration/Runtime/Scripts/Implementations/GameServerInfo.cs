using System;

namespace TPFive.Game.ApplicationConfiguration
{
    /// <summary>
    /// GameServer information.
    /// </summary>
    [Serializable]
    public sealed class GameServerInfo : IReadOnlyGameServerInfo
    {
        /// <summary>
        /// Gets or Sets base uri of game server.
        /// </summary>
        /// <value>
        /// domain of game server
        /// </value>
        public string BaseUri { get; set; }
    }
}
