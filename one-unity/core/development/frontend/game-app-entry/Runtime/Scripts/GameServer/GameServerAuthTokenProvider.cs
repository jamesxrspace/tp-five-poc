using System.Threading;
using System.Threading.Tasks;
using TPFive.Game.Account;
using TPFive.Game.GameServer;
using VContainer;
using IAccountService = TPFive.Game.Account.IService;

namespace TPFive.Game.App.Entry.GameServer
{
    /// <summary>
    /// Provides auth token of game server.
    /// </summary>
    public class GameServerAuthTokenProvider : IGameServerAuthTokenProvider
    {
        /// <summary>
        /// Login Service.
        /// </summary>
        private readonly IAccountService accountService;

        [Inject]
        public GameServerAuthTokenProvider(IAccountService accountService)
        {
            this.accountService = accountService;
        }

        /// <summary>
        /// Gets auth token.
        /// </summary>
        /// <returns>
        /// Returns auth token for access game server.
        /// </returns>
        public string GetToken()
        {
            return accountService.GetAccessToken();
        }

        /// <summary>
        /// Gets auth token.
        /// </summary>
        /// <param name="cancellationToken">cancellation token.</param>
        /// <returns>
        /// Returns auth token for access game server.
        /// </returns>
        public async Task<string> RefreshTokenAsync(CancellationToken cancellationToken)
        {
            var promise = new TaskCompletionSource<bool>();
            cancellationToken.Register(() => promise.TrySetCanceled());
            accountService.TryGetValidToken(new AuthCallback<string>(
                result =>
                {
                    promise.TrySetResult(true);
                },
                (code, msg) =>
                {
                    promise.TrySetResult(false);
                }));

            if (!await promise.Task)
            {
                return null;
            }

            return accountService.GetAccessToken();
        }
    }
}