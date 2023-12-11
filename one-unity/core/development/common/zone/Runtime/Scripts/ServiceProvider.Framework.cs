using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MessagePipe;
using Microsoft.Extensions.Logging;
using TPFive.Game;
using UniRx;
using UnityEngine.Assertions;

namespace TPFive.Extended.Zone
{
    using TPFive.SCG.AsyncStartable.Abstractions;
    using TPFive.SCG.DisposePattern.Abstractions;

    using TPFive.Game.Logging;

    using GameLoggingUtility = TPFive.Game.Logging.Utility;

    using GameConfig = TPFive.Game.Config;
    using GameResource = TPFive.Game.Resource;
    using GameSceneFlow = TPFive.Game.SceneFlow;

    using GameZone = TPFive.Game.Zone;

    [AsyncStartable(ExceptionList = "System.OperationCanceledException, System.Exception")]
    [Dispose]
    [AsyncDispose]
    public sealed partial class ServiceProvider
    {
        private const int DefaultWaitTimeInSecond = 180;

        private readonly CompositeDisposable _compositeDisposable = new ();
        private UniTaskCompletionSource<bool> _utcs = new ();

        private readonly GameConfig.IService _configService;
        private readonly GameResource.IService _resourceService;
        private readonly GameSceneFlow.IService _sceneFlowService;
        private readonly GameZone.IServiceProvider _nullServiceProvider;

        private readonly TPFive.Room.IOpenRoomCmd _openRoomCmd;

        private readonly IPublisher<Game.Messages.NotifyNetLoaderToUnload> _pubNotifyNetLoaderToUnload;

        private readonly IPublisher<Game.Messages.ContentLevelFullyLoaded> _pubContentLevelFullyLoaded;

        private readonly ISubscriber<Game.Messages.BackToHome> _subBackToHome;
        private readonly ISubscriber<Game.Messages.LoadContentLevel> _subLoadContentLevel;
        private readonly ISubscriber<Game.Messages.UnloadContentLevel> _subUnloadContentLevel;
        private readonly ISubscriber<Game.Messages.ContentLevelFullyLoaded> _subContentLevelFullyLoaded;

        private int _loadWaitTimeout;

        public ServiceProvider(
            ILoggerFactory loggerFactory,
            GameConfig.IService configService,
            GameResource.IService resourceService,
            GameSceneFlow.IService sceneFlowService,
            GameZone.IServiceProvider nullServiceProvider,
            TPFive.Room.IOpenRoomCmd openRoomCmd,
            IPublisher<Game.Messages.NotifyNetLoaderToUnload> pubNotifyNetLoaderToUnload,
            IPublisher<Game.Messages.ContentLevelFullyLoaded> pubContentLevelFullyLoaded,
            ISubscriber<Game.Messages.BackToHome> subBackToHome,
            ISubscriber<Game.Messages.LoadContentLevel> subLoadContentLevel,
            ISubscriber<Game.Messages.UnloadContentLevel> subUnloadContentLevel,
            ISubscriber<Game.Messages.ContentLevelFullyLoaded> subContentLevelFullyLoaded)
        {
            Assert.IsNotNull(loggerFactory);

            Logger = GameLoggingUtility.CreateLogger<ServiceProvider>(loggerFactory);

            _configService = configService;
            _resourceService = resourceService;
            _sceneFlowService = sceneFlowService;

            _nullServiceProvider = nullServiceProvider;

            _openRoomCmd = openRoomCmd;

            _pubNotifyNetLoaderToUnload = pubNotifyNetLoaderToUnload;

            _pubContentLevelFullyLoaded = pubContentLevelFullyLoaded;

            _subBackToHome = subBackToHome;
            _subLoadContentLevel = subLoadContentLevel;
            _subUnloadContentLevel = subUnloadContentLevel;
            _subContentLevelFullyLoaded = subContentLevelFullyLoaded;

            _funcOperationCanceledException = HandleStartAsyncOperationCanceledException;
            _funcException = HandleStartAsyncException;
        }

        private Microsoft.Extensions.Logging.ILogger Logger { get; }

        private async UniTask SetupBegin(CancellationToken cancellationToken = default)
        {
            Logger.LogEditorDebug(
                "{Method}",
                nameof(SetupBegin));

            var (rTimeout, vTimeout) = await _configService.GetSpecificProviderIntValueAsync(
                GameConfig.Constants.RuntimeLocalProviderKind,
                "RoomContentLoadMaxTime");

            _loadWaitTimeout = rTimeout ? vTimeout : DefaultWaitTimeInSecond;

            _roomCategoryOrder = CategoryOrder;
            _roomSubCategoryOrder = SubCategoryOrder;
            _homeCategoryOrder = CategoryOrder;
            _homeSubCategoryOrder = SubCategoryOrder;
            _roomSceneKey = RoomSceneKey;
            _homeSceneKey = HomeSceneKey;

            var (rHome, spHomeObject) = await _configService.GetSpecificProviderSystemObjectValueAsync(
                GameConfig.Constants.RuntimeLocalProviderKind,
                "HomeEntry");

            var (rRoom, spRoomObject) = await _configService.GetSpecificProviderSystemObjectValueAsync(
                GameConfig.Constants.RuntimeLocalProviderKind,
                "RoomEntry");

            if (rHome && spHomeObject is TPFive.Game.SceneProperty spHome)
            {
                _homeCategoryOrder = spHome.categoryOrder;
                _homeSubCategoryOrder = spHome.subOrder;
                _homeSceneKey = spHome.title;
            }

            if (rRoom && spRoomObject is TPFive.Game.SceneProperty spRoom)
            {
                _roomCategoryOrder = spRoom.categoryOrder;
                _roomSubCategoryOrder = spRoom.subOrder;
                _roomSceneKey = spRoom.title;
            }

            await SetupMessageHandling(cancellationToken);
        }

        private async UniTask SetupEnd(
            bool success,
            CancellationToken cancellationToken = default)
        {
            Logger.LogEditorDebug(
                "{Method} reaches finally block",
                nameof(SetupEnd));

            _utcs.TrySetResult(success);

            await UniTask.CompletedTask;
        }

        private async Task HandleStartAsyncOperationCanceledException(
            System.OperationCanceledException e,
            CancellationToken cancellationToken = default)
        {
            Logger.LogWarning("{Exception}", e);
            await Task.CompletedTask;
        }

        private async Task HandleStartAsyncException(
            System.Exception e,
            CancellationToken cancellationToken = default)
        {
            Logger.LogError("{Exception}", e);
            await Task.CompletedTask;
        }

        private void HandleDispose(bool disposing)
        {
            Logger.LogEditorDebug(
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
                _compositeDisposable?.Dispose();

                _disposed = true;
            }
        }

        private async ValueTask HandleDisposeAsync()
        {
            await Task.CompletedTask;
        }
    }
}
