using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MessagePipe;
using Microsoft.Extensions.Logging;
using UniRx;
using UnityEngine.Assertions;

namespace TPFive.Extended.QuantumConsole
{
    using TPFive.SCG.AsyncStartable.Abstractions;
    using TPFive.SCG.DisposePattern.Abstractions;

    using TPFive.Game.Logging;

    using GameLoggingUtility = TPFive.Game.Logging.Utility;

    using GameConfig = TPFive.Game.Config;
    using GameSceneFlow = TPFive.Game.SceneFlow;

    using GameAssist = TPFive.Game.Assist;

    [AsyncStartable(ExceptionList = "System.OperationCanceledException, System.Exception")]
    [Dispose]
    [AsyncDispose]
    public sealed partial class ServiceProvider
    {
        private readonly CompositeDisposable _compositeDisposable = new ();

        private readonly GameConfig.IService _configService;
        private readonly GameSceneFlow.IService _sceneFlowService;

        private readonly GameAssist.IServiceProvider _nullServiceProvider;

        private readonly IPublisher<Game.SceneFlow.ChangeScene> _pubChangeScene;
        private readonly IPublisher<Game.Messages.BackToHome> _pubBackToHome;

        private readonly ISubscriber<Game.Messages.AssistMode> _subAssistMode;

        private UniTaskCompletionSource<bool> _utcs = new ();

        public ServiceProvider(
            ILoggerFactory loggerFactory,
            GameConfig.IService configService,
            GameSceneFlow.IService sceneFlowService,
            GameAssist.IServiceProvider nullServiceProvider,
            QFSW.QC.QuantumConsole quantumConsole,
            IPublisher<Game.SceneFlow.ChangeScene> pubChangeScene,
            IPublisher<Game.Messages.BackToHome> pubBackToHome,
            ISubscriber<Game.Messages.AssistMode> subAssistMode)
        {
            Assert.IsNotNull(loggerFactory);

            Logger = GameLoggingUtility.CreateLogger<ServiceProvider>(loggerFactory);

            _configService = configService;
            _sceneFlowService = sceneFlowService;

            _nullServiceProvider = nullServiceProvider;

            _pubChangeScene = pubChangeScene;
            _pubBackToHome = pubBackToHome;

            _subAssistMode = subAssistMode;

            _funcOperationCanceledException = HandleStartAsyncOperationCanceledException;
            _funcException = HandleStartAsyncException;
        }

        private Microsoft.Extensions.Logging.ILogger Logger { get; }

        private async UniTask SetupBegin(CancellationToken cancellationToken = default)
        {
            Logger.LogEditorDebug(
                "{Method}",
                nameof(SetupBegin));

            await SetupMessageHandling(cancellationToken);
            SetupQuantumConsole();
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

            await UniTask.CompletedTask;
        }

        private async Task HandleStartAsyncException(
            System.Exception e,
            CancellationToken cancellationToken = default)
        {
            Logger.LogError("{Exception}", e);

            await UniTask.CompletedTask;
        }

        private void HandleDispose(bool disposing)
        {
            Logger.LogEditorDebug(
                "{Method} - disposing: {Disposing}",
                nameof(HandleDispose),
                disposing);

            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                CleanupQuantumConsole();
                _compositeDisposable?.Dispose();

                _disposed = true;
            }
        }

        private async ValueTask HandleDisposeAsync()
        {
            await UniTask.CompletedTask;
        }
    }
}
