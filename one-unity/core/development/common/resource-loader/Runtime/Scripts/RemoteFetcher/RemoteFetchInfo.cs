namespace TPFive.Extended.ResourceLoader
{
    using System.Threading;
    using System.Threading.Tasks;
    using BestHTTP;
    using Microsoft.Extensions.Logging;
    using TPFive.Game.Resource;
    using VContainer;
    using ILogger = Microsoft.Extensions.Logging.ILogger;

    public class RemoteFetchInfo : IRemoteFetcher, System.IDisposable
    {
        private readonly IPool<RemoteResponse> _remoteResponsePool;
        private readonly System.Uri _uri;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private ILoggerFactory _loggerFactory;
        private ILogger logger;

        public RemoteFetchInfo(
            IPool<RemoteResponse> remoteResponsePool,
            System.Uri uri,
            ILoggerFactory loggerFactory)
        {
            _remoteResponsePool = remoteResponsePool;
            _uri = uri;
            _cancellationTokenSource = new CancellationTokenSource();
            _loggerFactory = loggerFactory;
        }

        public ILogger Logger => logger ??= _loggerFactory.CreateLogger<RemoteFetchInfo>();

        public async Task<RemoteResponse> Fetch(CancellationToken cancellationToken = default)
        {
            Logger.LogDebug($"{nameof(Fetch)} - {_uri}");

            cancellationToken.Register(_cancellationTokenSource.Cancel);

            var remoteResponse = new RemoteResponse();
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var request = new HTTPRequest(_uri) { MaxRetries = 3, };
                remoteResponse.InfoMessage = await request.GetAsStringAsync(cancellationToken);
                remoteResponse.Valid = true;
            }
            catch (System.OperationCanceledException e)
            {
                Logger.LogWarning($"{nameof(Fetch)} Task Canceled", e);
            }
            catch (AsyncHTTPException e)
            {
                Logger.LogWarning($"{nameof(Fetch)} - AsyncHTTPException - while loading {_uri}: ", e);
            }
            catch (System.Exception e)
            {
                Logger.LogWarning($"{nameof(Fetch)} - General - while loading {_uri}: ", e);
            }

            return remoteResponse;
        }

        public void Dispose()
        {
            Logger.LogDebug($"{nameof(Dispose)} - request cancel");

            if (!_cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource?.Cancel();
            }
        }
    }
}