using System;
using TPFive.Game.ApplicationConfiguration;
using TPFive.Game.GameServer;
using VContainer;

namespace TPFive.Game.App.Entry.GameServer
{
    /// <summary>
    /// Provides base uri of game server.
    /// </summary>
    public class GameServerBaseUriProvider : IGameServerBaseUriProvider
    {
        /// <summary>
        /// Application information.
        /// </summary>
        private readonly IReadOnlyAppInfo appInfo;

        [Inject]
        public GameServerBaseUriProvider(IReadOnlyAppInfo appInfo)
        {
            this.appInfo = appInfo;
        }

        /// <summary>
        /// Gets the base URI.
        /// </summary>
        /// <value>
        /// domain of game server
        /// </value>
        public Uri BaseUri => new Uri(appInfo.GameServer.BaseUri);
    }
}