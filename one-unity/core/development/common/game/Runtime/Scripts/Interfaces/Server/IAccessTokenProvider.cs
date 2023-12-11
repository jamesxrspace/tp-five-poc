using System.Threading;
using System.Threading.Tasks;

namespace TPFive.Game.Server
{
    /// <summary>
    /// Provides access token.
    /// <br/>
    /// e.g. auth token, session token, etc.
    /// </summary>
    public interface IAccessTokenProvider
    {
        string GetToken();

        Task<string> RefreshTokenAsync(CancellationToken cancellationToken);
    }
}