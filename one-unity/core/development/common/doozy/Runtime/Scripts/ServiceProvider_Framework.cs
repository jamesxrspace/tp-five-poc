using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MessagePipe;
using Microsoft.Extensions.Logging;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;
using VContainer.Unity;

namespace TPFive.Extended.Doozy
{
    //
    using TPFive.SCG.AsyncStartable.Abstractions;
    using TPFive.SCG.DisposePattern.Abstractions;

    //
    using TPFive.Game.Logging;

    //
    using GameMessages = TPFive.Game.Messages;

    //
    using GameLoggingUtility = TPFive.Game.Logging.Utility;

    using GameConfig = TPFive.Game.Config;

    using GameHud = TPFive.Game.Hud;

    [AsyncStartable(ExceptionList = "System.OperationCanceledException, System.Exception")]
    [Dispose]
    [AsyncDispose]
    public sealed partial class ServiceProvider
    {
        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        private UniTaskCompletionSource<bool> _utcs = new UniTaskCompletionSource<bool>();

        //
        private readonly GameConfig.IService _configService;
        //
        private readonly GameHud.IServiceProvider _nullServiceProvider;

        //
        private readonly IPublisher<GameMessages.HudMessage> _pubHudMessage;

        private readonly ISubscriber<TPFive.Game.Messages.SceneLoading> _subSceneLoading;
        private readonly ISubscriber<TPFive.Game.Messages.SceneLoaded> _subSceneLoaded;
        private readonly ISubscriber<TPFive.Game.Messages.SceneUnloading> _subSceneUnloading;
        private readonly ISubscriber<TPFive.Game.Messages.SceneUnloaded> _subSceneUnloaded;

        //
        private Settings _settings;

        private bool _setup;

        public ServiceProvider(
            //
            ILoggerFactory loggerFactory,
            //
            GameConfig.IService configService,
            //
            GameHud.IServiceProvider nullServiceProvider,

            //
            IPublisher<GameMessages.HudMessage> pubHudMessage,

            ISubscriber<TPFive.Game.Messages.SceneLoading> subSceneLoading,
            ISubscriber<TPFive.Game.Messages.SceneLoaded> subSceneLoaded,
            ISubscriber<TPFive.Game.Messages.SceneUnloading> subSceneUnloading,
            ISubscriber<TPFive.Game.Messages.SceneUnloaded> subSceneUnloaded,

            ScriptableObject settingsSO)
        {
            Assert.IsNotNull(loggerFactory);

            Logger = GameLoggingUtility.CreateLogger<ServiceProvider>(loggerFactory);

            _configService = configService;

            _nullServiceProvider = nullServiceProvider;

            _pubHudMessage = pubHudMessage;

            _subSceneLoading = subSceneLoading;
            _subSceneLoaded = subSceneLoaded;
            _subSceneUnloading = subSceneUnloading;
            _subSceneUnloaded = subSceneUnloaded;

            //
            _settings = settingsSO as Settings;
            Assert.IsNotNull(_settings);

            _funcOperationCanceledException = HandleStartAsyncOperationCanceledException;
            _funcException = HandleStartAsyncException;

        }

        //
        private Microsoft.Extensions.Logging.ILogger Logger { get; }

        //
        private async UniTask SetupBegin(CancellationToken cancellationToken = default)
        {
            Logger.LogEditorDebug(
                "{Method}",
                nameof(SetupBegin));

            await SetupMessageHandling(cancellationToken);

            SetupDoozySignal();
            RegisterDoozySignal();
        }

        private async Task SetupMessageHandling(CancellationToken cancellationToken = default)
        {
            _subSceneLoaded
                .Subscribe(x =>
                {
                    // if (x is { CategoryOrder: 3, SubOrder: 0 })
                    // {
                    //     Logger.LogEditorDebug(
                    //         "{Method} - Receive {Message} - {Title} {CategoryOrder}, {SubOrder}",
                    //         nameof(SetupMessageHandling),
                    //         nameof(GameMessages.SceneLoaded),
                    //         x.Title,
                    //         x.CategoryOrder,
                    //         x.SubOrder);
                    //
                    //     SetupDoozySignal();
                    //     RegisterDoozySignal();
                    // }
                })
                .AddTo(_compositeDisposable);

            _subSceneUnloaded
                .Subscribe(x =>
                {
                    // if (x is { CategoryOrder: 3, SubOrder: 0 })
                    // {
                    //     Logger.LogEditorDebug(
                    //         "{Method} - Receive {Message} - {Title} {CategoryOrder}, {SubOrder}",
                    //         nameof(SetupMessageHandling),
                    //         nameof(GameMessages.SceneUnloaded),
                    //         x.Title,
                    //         x.CategoryOrder,
                    //         x.SubOrder);
                    //
                    //     UnregisterDoozySignal();
                    //     CleanupDoozySignal();
                    // }
                })
                .AddTo(_compositeDisposable);
        }

        private async UniTask SetupEnd(bool success, CancellationToken cancellationToken = default)
        {
            Logger.LogEditorDebug(
                "{Method} reaches finally block",
                nameof(SetupEnd));

            _utcs.TrySetResult(success);
        }

        private async Task HandleStartAsyncOperationCanceledException(
            System.OperationCanceledException e, CancellationToken cancellationToken = default)
        {
            Logger.LogWarning("{Exception}", e);
        }

        private async Task HandleStartAsyncException(
            System.Exception e, CancellationToken cancellationToken = default)
        {
            Logger.LogError("{Exception}", e);
        }

        //
        private void HandleDispose(bool disposing)
        {
            Logger.LogEditorDebug(
                "{Method} - disposing: {Disposing}",
                nameof(HandleDispose),
                disposing);

            if (disposing)
            {
                // if (_doneSetup)
                {
                    UnregisterDoozySignal();
                    CleanupDoozySignal();
                }

                _compositeDisposable?.Dispose();
            }
        }

        private async ValueTask HandleDisposeAsync()
        {
            await Task.Yield();
        }
    }
}
