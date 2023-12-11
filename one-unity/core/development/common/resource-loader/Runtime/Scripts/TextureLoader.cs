using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TPFive.Game.Resource;
using UniRx;
using UnityEngine.Assertions;
using ObservableExtensions = UniRx.ObservableExtensions;

namespace TPFive.Extended.ResourceLoader
{
    /// <summary>
    /// This loads texture(currently used in space and UI).
    /// </summary>
    public class TextureLoader : IResourceLoader
    {
        private readonly IResourceManager _resourceManager;
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        // Still in plan if pool is necessary or not, but keep the declaration here
        private readonly IPool<RemoteResponse> _pool;
        private readonly TextureDataFactory _textureDataFactory;
        private readonly TextureDataErrorHandler _errorHandler;
        private readonly int _maxRetry;
        private readonly TimeSpan _timeout;

        // This is a composite usage for remote fetching. This makes the extending of several
        // remote operation easier for this loader.
        private readonly List<IRemoteFetcher> _remoteFetchers = new List<IRemoteFetcher>();

        // Keep the pool usage commented for now
        // private readonly List<RemoteResponse> _returningToPool = new List<RemoteResponse>();
        private readonly List<RemoteResponse> _cachedFromLoad = new List<RemoteResponse>();
        private TextureRequestContext _requestContext;
        private TextureData _textureData;
        private bool _disposed = false;
        private ILoggerFactory _loggerFactory;
        private ILogger<TextureLoader> logger;

        public TextureLoader(
            IResourceManager resourceManager,
            TextureRequestContext requestContext,
            IPool<RemoteResponse> pool,
            TextureDataFactory textureDataFactory,
            ILoggerFactory loggerFactory)
        {
            _resourceManager = resourceManager;
            _requestContext = requestContext;
            _pool = pool;
            _textureDataFactory = textureDataFactory;
            _loggerFactory = loggerFactory;
            _errorHandler = _requestContext.ErrorHandler;
            _maxRetry = _requestContext.MaxRetry;
            _timeout = _requestContext.Timeout;

            Assert.IsNotNull(_resourceManager);
            Assert.IsNotNull(_pool);
            Assert.IsNotNull(_textureDataFactory);
        }

        public ILogger Logger => logger ??= _loggerFactory.CreateLogger<TextureLoader>();

        public string Url => _requestContext.Key;

        public void Load()
        {
            var mainThread = TaskScheduler.FromCurrentSynchronizationContext();

            ObservableExtensions.Subscribe(
                InternalLoad(_cancellationTokenSource.Token).ToObservable()
                    .ObserveOnMainThread()
                    .SubscribeOnMainThread(),
                onNext: response =>
                {
                    Logger.LogDebug($"{nameof(Load)} - got texture data");

                    if (response.Valid)
                    {
                        var t = new Task(
                        async () =>
                        {
                            _textureData = await ConvertToTextureData(response);
                            if (_textureData == null || _textureData == TextureData.Default)
                            {
                                Logger.LogWarning($"{nameof(Load)} - textureData can not be converted");
                                _textureData = TextureData.Default;
                                _textureData.Succeed = false;
                                _textureData.UnsuccessfulReason =
                                    _errorHandler?.Handle(response) ?? UnsuccessfulReason.Unknown;
                                _resourceManager.OnLoaderFailed(this);
                            }
                            else
                            {
                                _textureData.SetTextureName(Url);
#if UNITY_EDITOR
                                Logger.LogDebug($"{nameof(Load)} - textureData loaded: {_textureData}");
#endif
                                _textureData.Succeed = true;
                                _textureData.UnsuccessfulReason = UnsuccessfulReason.Unknown;
                                _resourceManager.OnLoaderFinished(this);
                            }
                        });
                        t.Start(mainThread);
                    }
                    else
                    {
#if UNITY_EDITOR
                        Logger.LogDebug($"{nameof(Load)} - remoteResponse: {response.Valid}");
#endif
                        _textureData = TextureData.Default;
                        _textureData.Succeed = false;
                        _textureData.UnsuccessfulReason = UnsuccessfulReason.Unknown;
                        _resourceManager.OnLoaderFailed(this);
                    }
                },
                onError: e =>
                {
                    Logger.LogWarning($"{nameof(Load)} - error causing loader fail");
                    _textureData = TextureData.Default;
                    _textureData.Succeed = false;
                    _textureData.UnsuccessfulReason = UnsuccessfulReason.Unknown;
                    _resourceManager.OnLoaderFailed(this);
                })
                .AddTo(_compositeDisposable);
        }

        public void Abort()
        {
            _cancellationTokenSource.Cancel();
        }

        public object GetResource()
        {
            return _textureData;
        }

        public void Dispose()
        {
            this.Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        private async Task<RemoteResponse> InternalLoad(CancellationToken cancellationToken = default)
        {
            Logger.LogDebug($"{nameof(InternalLoad)}");
            try
            {
                Assert.IsTrue(!_cachedFromLoad.Any());

                if (_requestContext.ApplyConversion)
                {
                    var remoteResponseNormal = await LoadNormal(hasImageOptimizeFeature: true, cancellationToken);
                    _cachedFromLoad.Add(remoteResponseNormal);
                    if (!remoteResponseNormal.Valid)
                    {
#if UNITY_EDITOR
                        Logger.LogDebug($"{nameof(InternalLoad)} - LoadNormal: {remoteResponseNormal.Valid}");
#endif
                        if (_requestContext.Fallback)
                        {
                            // Was using auto fallback assumption, now let fallback flag decides if fallback take
                            // place if normal path fail
                            var remoteResponseFallback = await LoadFallback(cancellationToken);
                            _cachedFromLoad.Add(remoteResponseFallback);
                        }
                    }
                }
                else
                {
                    // When not using ktx2, just load as fallback solution for normal image
                    var remoteResponseFallback = await LoadFallback(cancellationToken);
                    _cachedFromLoad.Add(remoteResponseFallback);
                }

                var remoteResponse = FlattenRemoteResponse(_pool, _cachedFromLoad);
                if (!remoteResponse.Valid)
                {
                    Logger.LogWarning($"{nameof(InternalLoad)} - remoteResponse {remoteResponse.Valid}");
                }

                return remoteResponse;
            }
            catch (System.Exception e)
            {
                Logger.LogWarning($"{nameof(InternalLoad)} - General", e);
                var remoteResponse = new RemoteResponse();
                return remoteResponse;
            }
        }

        private async Task<RemoteResponse> LoadNormal(bool hasImageOptimizeFeature, CancellationToken cancellationToken)
        {
            Logger.LogDebug($"{nameof(LoadNormal)} - hasImageOptimizeFeature: {hasImageOptimizeFeature}");

            // var remoteResponse = _pool.Get();
            var remoteResponse = new RemoteResponse();
            try
            {
                _remoteFetchers.Clear();

                cancellationToken.ThrowIfCancellationRequested();

                _requestContext.NormalImageFormat = ImageFormat.Ktx2;

                // Remote Fetcher to get data
                {
                    var url = _requestContext.Url;
                    _requestContext.NormalImageFormat = ImageFormat.Normal;

                    var uri = new System.Uri(url);
#if UNITY_EDITOR
                    Logger.LogDebug($"{nameof(LoadNormal)} - {uri}");
#endif
                    _remoteFetchers.Add(new RemoteFetchData(_pool, uri, _maxRetry, _timeout, _loggerFactory));
                }

                cancellationToken.ThrowIfCancellationRequested();

                var fetchingTasks = _remoteFetchers.Select(x => x.Fetch(_cancellationTokenSource.Token));
                var fetchedTasks = await Task.WhenAll(fetchingTasks);
                remoteResponse = FlattenRemoteResponse(_pool, fetchedTasks);

                // ReturnToPool(_pool, fetchedTasks);
                remoteResponse.ImageFormat = _requestContext.NormalImageFormat;

#if UNITY_EDITOR
                Logger.LogDebug($"{nameof(LoadNormal)} - before returning");
#endif
            }
            catch (System.OperationCanceledException e)
            {
                Logger.LogDebug($"{nameof(LoadNormal)} Task Canceled", e);
            }
            catch (System.Exception e)
            {
                Logger.LogWarning($"{nameof(LoadNormal)} - General", e);
            }

            return remoteResponse;
        }

        private async Task<RemoteResponse> LoadFallback(CancellationToken cancellationToken)
        {
            Logger.LogDebug($"{nameof(LoadFallback)}");

            // var remoteResponse = _pool.Get();
            var remoteResponse = new RemoteResponse();
            try
            {
                _remoteFetchers.Clear();

                cancellationToken.ThrowIfCancellationRequested();

                var uri = new System.Uri(_requestContext.Url);
#if UNITY_EDITOR
                Logger.LogDebug($"{nameof(LoadFallback)} - {uri}");
#endif
                _requestContext.FallbackImageFormat = ImageFormat.Normal;
                _remoteFetchers.Add(new RemoteFetchData(_pool, uri, _maxRetry, _timeout, _loggerFactory));

                cancellationToken.ThrowIfCancellationRequested();

                var fetchingTasks = _remoteFetchers.Select(x => x.Fetch(_cancellationTokenSource.Token));
                var fetchedTasks = await Task.WhenAll(fetchingTasks);
                remoteResponse = FlattenRemoteResponse(_pool, fetchedTasks);
            }
            catch (System.OperationCanceledException e)
            {
                Logger.LogDebug($"{nameof(LoadFallback)} Task Canceled", e);
            }
            catch (System.Exception e)
            {
                Logger.LogWarning($"{nameof(LoadFallback)} - General", e);
            }

            return remoteResponse;
        }

        private async Task<(bool, TextureData)> TryConvertToTextureData(
            RemoteResponse remoteResponse,
            TextureDataFactory textureDataFactory)
        {
            try
            {
                var context = new TextureDataCreationContext
                {
                    ImageFormat = remoteResponse.ImageFormat,
                    Width = remoteResponse.Width,
                    Height = remoteResponse.Height,
                    HasOrientation = remoteResponse.HasOrientation,
                    Orientation = remoteResponse.Orientation,
                    Data = remoteResponse.Data,
                };

                var textureData = await textureDataFactory.Create(context);

                return (true, textureData);
            }
            catch (System.Exception e)
            {
                Logger.LogWarning($"{nameof(TryConvertToTextureData)} - exception: {e}");
                return (false, TextureData.Default);
            }
        }

        private async Task<TextureData> ConvertToTextureData(RemoteResponse remoteResponse)
        {
            try
            {
                var (valid, textureData) = await TryConvertToTextureData(remoteResponse, _textureDataFactory);
                if (!valid)
                {
#if UNITY_EDITOR
                    Logger.LogDebug($"{nameof(InternalLoad)} - convert to texture data fail");
#endif
                    return TextureData.Default;
                }

                return textureData;
            }
            catch (System.Exception e)
            {
                Logger.LogWarning($"{nameof(ConvertToTextureData)} - General", e);
                return TextureData.Default;
            }
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                Abort();
                _textureData = null;
                _cachedFromLoad.Clear();
                _compositeDisposable?.Dispose();
            }

            _disposed = true;
        }

        /// <summary>
        /// Combine all the remote response into one as different response only contain portion of
        /// information that need to be processed later.
        /// The use of pool is still in plan, keep the param but not use pool at this moment.
        /// </summary>
        /// <param name="pool">RemoteResponse pool.</param>
        /// <param name="remoteResponses">remote response list.</param>
        /// <returns>Remote Response.</returns>
        private RemoteResponse FlattenRemoteResponse(
            IPool<RemoteResponse> pool,
            IList<RemoteResponse> remoteResponses)
        {
            // var result = pool.Get();
            var result = new RemoteResponse();

            for (var index = 0; index < remoteResponses.Count; index++)
            {
                var remoteResponse = remoteResponses[index];

                // When response is invalid or valid always aggregate the error message.
                if (!string.IsNullOrEmpty(remoteResponse.ErrorMessage))
                {
                    var splitChar = index == 0 ? string.Empty : ";";
                    result.ErrorMessage += splitChar + remoteResponse.ErrorMessage;
                }

                if (!remoteResponse.Valid)
                {
                    continue;
                }

                // There is always be true.
                result.Valid = remoteResponse.Valid;

                if (remoteResponse.Data != null)
                {
                    result.Data = remoteResponse.Data;
                }

                if (!string.IsNullOrEmpty(remoteResponse.InfoMessage))
                {
                    result.InfoMessage = remoteResponse.InfoMessage;
                }

                if (remoteResponse.Width != 0)
                {
                    result.Width = remoteResponse.Width;
                }

                if (remoteResponse.Height != 0)
                {
                    result.Height = remoteResponse.Height;
                }

                result.ImageFormat = remoteResponse.ImageFormat;
            }

            return result;
        }

        // Keep the pool use for now, may be enhanced or removed according to the situation
        private void ReturnToPool(
            IPool<RemoteResponse> pool,
            IList<RemoteResponse> remoteResponses)
        {
            var count = remoteResponses.Count;
            for (var i = 0; i < count; ++i)
            {
                var rp = remoteResponses[i];
                pool.Release(rp);
            }
        }
    }
}