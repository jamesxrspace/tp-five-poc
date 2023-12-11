using System.Collections.Generic;
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

namespace TPFive.Extended.SceneActivity
{
    using TPFive.Game.Logging;
    using TPFive.SCG.AsyncStartable.Abstractions;
    using TPFive.SCG.DisposePattern.Abstractions;

    using GameConfig = TPFive.Game.Config;
    using GameLoggingUtility = TPFive.Game.Logging.Utility;
    using GameResource = TPFive.Game.Resource;
    using GameSceneFlow = TPFive.Game.SceneFlow;

    [AsyncStartable(ExceptionList = "System.OperationCanceledException, System.Exception")]
    [Dispose]
    [AsyncDispose]
    public partial class ServiceProvider :
        GameSceneFlow.IServiceProvider
    {
        private readonly CompositeDisposable _compositeDisposable = new ();
        private readonly GameConfig.IService _configService;
        private readonly GameResource.IService _resourceService;

        // TODO: Make this auto assigned later
        private readonly GameSceneFlow.IServiceProvider _nullServiceProvider;

        private readonly IPublisher<TPFive.Game.Messages.SceneLoading> _pubSceneLoading;
        private readonly IPublisher<TPFive.Game.Messages.SceneLoaded> _pubSceneLoaded;
        private readonly IPublisher<TPFive.Game.Messages.SceneUnloading> _pubSceneUnloading;
        private readonly IPublisher<TPFive.Game.Messages.SceneUnloaded> _pubSceneUnloaded;

        private readonly IPublisher<TPFive.Game.FlutterUnityWidget.PostUnityMessage> _pubPostUnityMessage;

        private readonly ISubscriber<TPFive.Game.Messages.SceneLoading> _subSceneLoading;
        private readonly ISubscriber<TPFive.Game.Messages.SceneLoaded> _subSceneLoaded;
        private readonly ISubscriber<TPFive.Game.Messages.SceneUnloading> _subSceneUnloading;
        private readonly ISubscriber<TPFive.Game.Messages.SceneUnloaded> _subSceneUnloaded;

        private UniTaskCompletionSource<bool> _utcs = new ();

        public ServiceProvider(
            ILoggerFactory loggerFactory,
            GameConfig.IService configService,
            GameResource.IService resourceService,
            GameSceneFlow.IServiceProvider nullServiceProvider,
            IPublisher<TPFive.Game.Messages.SceneLoading> pubSceneLoading,
            IPublisher<TPFive.Game.Messages.SceneLoaded> pubSceneLoaded,
            IPublisher<TPFive.Game.Messages.SceneUnloading> pubSceneUnloading,
            IPublisher<TPFive.Game.Messages.SceneUnloaded> pubSceneUnloaded,
            IPublisher<TPFive.Game.FlutterUnityWidget.PostUnityMessage> pubPostUnityMessage,
            ISubscriber<TPFive.Game.Messages.SceneLoading> subSceneLoading,
            ISubscriber<TPFive.Game.Messages.SceneLoaded> subSceneLoaded,
            ISubscriber<TPFive.Game.Messages.SceneUnloading> subSceneUnloading,
            ISubscriber<TPFive.Game.Messages.SceneUnloaded> subSceneUnloaded)
        {
            Assert.IsNotNull(loggerFactory);

            Logger = GameLoggingUtility.CreateLogger<ServiceProvider>(loggerFactory);

            _configService = configService;
            _resourceService = resourceService;

            _nullServiceProvider = nullServiceProvider;

            _pubSceneLoading = pubSceneLoading;
            _pubSceneLoaded = pubSceneLoaded;
            _pubSceneUnloading = pubSceneUnloading;
            _pubSceneUnloaded = pubSceneUnloaded;

            _pubPostUnityMessage = pubPostUnityMessage;

            _subSceneLoading = subSceneLoading;
            _subSceneLoaded = subSceneLoaded;
            _subSceneUnloading = subSceneUnloading;
            _subSceneUnloaded = subSceneUnloaded;

            _funcOperationCanceledException = HandleStartAsyncOperationCanceledException;
            _funcException = HandleStartAsyncException;
        }

        private Microsoft.Extensions.Logging.ILogger Logger { get; }

        private async UniTask SetupBegin(CancellationToken cancellationToken = default)
        {
            Logger.LogEditorDebug(
                "{Method}",
                nameof(SetupBegin));

            await UniTask.CompletedTask;
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
            if (disposing)
            {
                _compositeDisposable?.Dispose();
            }
        }

        private async ValueTask HandleDisposeAsync()
        {
            await Task.CompletedTask;
        }
    }
}
