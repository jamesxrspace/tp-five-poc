using System;
using TPFive.Game.GameServer;
using VContainer;
using XRSpace.OpenAPI;

namespace TPFive.OpenApi.GameServer
{
    public class GameServerBaseUriProvider : IServerBaseUriProvider
    {
        private readonly IGameServerBaseUriProvider baseUriProvider;

        [Inject]
        public GameServerBaseUriProvider(IGameServerBaseUriProvider baseUriProvider)
        {
            this.baseUriProvider = baseUriProvider;
        }

        public Uri BaseUri
        {
            get => baseUriProvider.BaseUri;
        }
    }
}