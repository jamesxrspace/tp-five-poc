using System;

namespace TPFive.Extended.ResourceLoader
{
    using System.Threading;
    using System.Threading.Tasks;
    using BestHTTP;
    using Microsoft.Extensions.Logging;
    using TPFive.Game.Resource;

    public class RemoteFetchData :
        IRemoteFetcher,
        System.IDisposable
    {
        private readonly IPool<RemoteResponse> _remoteResponsePool;
        private readonly System.Uri _uri;
        private readonly TimeSpan _timeout;
        private readonly int _maxRetry;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private ILoggerFactory _loggerFactory;
        private ILogger<RemoteFetchData> logger;

        public RemoteFetchData(
            IPool<RemoteResponse> remoteResponsePool,
            System.Uri uri,
            int maxRetry,
            TimeSpan timeout,
            ILoggerFactory loggerFactory)
        {
            _remoteResponsePool = remoteResponsePool;
            _uri = uri;
            _timeout = timeout;
            _maxRetry = maxRetry;
            _cancellationTokenSource = new CancellationTokenSource();
            _loggerFactory = loggerFactory;
        }

        public ILogger Logger => logger ??= _loggerFactory.CreateLogger<RemoteFetchData>();

        public async Task<RemoteResponse> Fetch(CancellationToken cancellationToken = default)
        {
            Logger.LogDebug($"{nameof(Fetch)} - {_uri}");

            var remoteResponse = new RemoteResponse();
            try
            {
                cancellationToken.Register(_cancellationTokenSource.Cancel);

                cancellationToken.ThrowIfCancellationRequested();

                var request = new HTTPRequest(_uri)
                {
                    MaxRetries = _maxRetry,
                    Timeout = _timeout,
                };
                var data = await request.GetRawDataAsync(cancellationToken);
                remoteResponse.Data = data;
                remoteResponse.Valid = true;
            }
            catch (System.OperationCanceledException)
            {
            }
            catch (AsyncHTTPException e)
            {
                Logger.LogWarning($"{nameof(Fetch)} - AsyncHTTPException: while loading {_uri}: ", e);
                remoteResponse.ErrorMessage = e.ToString();
            }
            catch (System.Exception e)
            {
                Logger.LogWarning($"{nameof(Fetch)} - General - while loading {_uri}: ", e);
            }

            return remoteResponse;
        }

        public void Dispose()
        {
            Logger.LogWarning($"{nameof(Dispose)} - request cancel");
            if (!_cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource?.Cancel();
            }
        }
    }
}