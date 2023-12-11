namespace TPFive.Extended.ResourceLoader
{
    using System.Threading;
    using System.Threading.Tasks;
    using TPFive.Game.Resource;

    /// <summary>
    /// This interface will help the later remote fetching job being extended easily
    /// </summary>
    public interface IRemoteFetcher
    {
        Task<RemoteResponse> Fetch(CancellationToken cancellationToken = default);
    }
}