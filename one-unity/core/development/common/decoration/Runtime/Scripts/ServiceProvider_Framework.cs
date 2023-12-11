using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MessagePipe;
using Microsoft.Extensions.Logging;
using TPFive.Game.Logging;
using TPFive.Game.Messages;
using TPFive.OpenApi.GameServer;
using TPFive.SCG.AsyncStartable.Abstractions;
using TPFive.SCG.DisposePattern.Abstractions;
using UnityEngine.Assertions;
using VContainer;

namespace TPFive.Extended.Decoration
{
    using GameLoggingUtility = TPFive.Game.Logging.Utility;
    using IDecorationServiceProvider = TPFive.Game.Decoration.IServiceProvider;
    using IResourceService = TPFive.Game.Resource.IService;

    [AsyncStartable(ExceptionList = "System.OperationCanceledException, System.Exception")]
    [Dispose]
    public sealed partial class ServiceProvider
    {
        private readonly IDecorationServiceProvider _nullServiceProvider;
        private readonly IDecorationApi _decorationApi;
        private readonly IResourceService _resourceService;
        private readonly ISubscriber<SceneUnloading> _sceneUnloadingPublisher;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;
        private UniTaskCompletionSource<bool> _utcs = new ();
        private IDisposable disposable;

        [Inject]
        public ServiceProvider(
            ILoggerFactory loggerFactory,
            IDecorationApi decorationApi,
            IResourceService resourceService,
            ISubscriber<SceneUnloading> sceneUnloadingPublisher,
            IDecorationServiceProvider nullServiceProvider)
        {
            Assert.IsNotNull(loggerFactory);

            _loggerFactory = loggerFactory;
            _logger = GameLoggingUtility.CreateLogger<ServiceProvider>(loggerFactory);
            _decorationApi = decorationApi;
            _resourceService = resourceService;
            _sceneUnloadingPublisher = sceneUnloadingPublisher;
            _nullServiceProvider = nullServiceProvider;
            var unloadAssetSubscriber = _sceneUnloadingPublisher.Subscribe(handler =>
            {
                ReleaseAll(CancellationToken.None).Forget();
            });
            disposable = DisposableBag.Create(unloadAssetSubscriber);
        }

        private async UniTask SetupBegin(CancellationToken cancellationToken = default)
        {
            _logger.LogEditorDebug(
                "{Method}",
                nameof(SetupBegin));

            await UniTask.CompletedTask;
        }

        private async UniTask SetupEnd(
            bool success,
            CancellationToken cancellationToken = default)
        {
            _logger.LogEditorDebug(
                "{Method} reaches finally block",
                nameof(SetupEnd));

            _utcs.TrySetResult(success);

            await UniTask.CompletedTask;
        }

        private void HandleDispose(bool disposing)
        {
            _logger.LogEditorDebug(
                "{Method} - disposing: {Disposing} disposed: {Disposed}",
                nameof(HandleDispose),
                disposing,
                _disposed);

            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _disposed = true;
                disposable?.Dispose();
            }
        }

        private async ValueTask HandleDisposeAsync()
        {
            await Task.CompletedTask;
        }
    }
}