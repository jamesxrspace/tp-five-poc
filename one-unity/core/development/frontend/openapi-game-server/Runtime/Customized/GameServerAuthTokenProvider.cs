using System.Threading;
using System.Threading.Tasks;
using TPFive.Game.GameServer;
using VContainer;
using XRSpace.OpenAPI;

namespace TPFive.OpenApi.GameServer
{
    public class GameServerAuthTokenProvider : IAuthTokenProvider
    {
        private readonly IGameServerAuthTokenProvider authTokenProvider;

        [Inject]
        public GameServerAuthTokenProvider(IGameServerAuthTokenProvider authTokenProvider)
        {
            this.authTokenProvider = authTokenProvider;
        }

        public string GetAuthToken()
        {
            return authTokenProvider.GetToken();
        }

        public Task<string> RefreshTokenAsync(CancellationToken cancellationToken)
        {
            return authTokenProvider.RefreshTokenAsync(cancellationToken);
        }
    }
}