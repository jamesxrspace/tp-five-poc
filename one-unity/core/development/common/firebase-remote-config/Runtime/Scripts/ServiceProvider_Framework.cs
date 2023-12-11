using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MessagePipe;
using Microsoft.Extensions.Logging;
using UniRx;
using UnityEngine.Assertions;

namespace TPFive.Extended.FirebaseRemoteConfig
{
    using TPFive.Game.Logging;
    using TPFive.SCG.AsyncStartable.Abstractions;
    using TPFive.SCG.DisposePattern.Abstractions;

    using GameConfig = TPFive.Game.Config;
    using GameLoggingUtility = TPFive.Game.Logging.Utility;
    using GameMessages = TPFive.Game.Messages;

    [AsyncStartable(ExceptionList = "System.OperationCanceledException, System.Exception")]
    [Dispose]
    [AsyncDispose]
    public sealed partial class ServiceProvider
    {
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        private readonly GameConfig.IServiceProvider _nullServiceProvider;
        private readonly IAsyncPublisher<GameMessages.MultiPhaseSetupDone> _asyncPubMultiPhaseSetupDone;

        private UniTaskCompletionSource<bool> _utcs = new UniTaskCompletionSource<bool>();

        public ServiceProvider(
            ILoggerFactory loggerFactory,
            GameConfig.IServiceProvider nullServiceProvider,
            IAsyncPublisher<GameMessages.MultiPhaseSetupDone> asyncPubMultiPhaseSetupDone)
        {
            Assert.IsNotNull(loggerFactory);

            Logger = GameLoggingUtility.CreateLogger<ServiceProvider>(loggerFactory);

            _nullServiceProvider = nullServiceProvider;

            _asyncPubMultiPhaseSetupDone = asyncPubMultiPhaseSetupDone;

            _funcOperationCanceledException = HandleStartAsyncOperationCanceledException;
            _funcException = HandleStartAsyncException;
        }

        private Microsoft.Extensions.Logging.ILogger Logger { get; }

        private async UniTask SetupBegin(CancellationToken cancellationToken = default)
        {
            Logger.LogEditorDebug(
                "{Method}",
                nameof(SetupBegin));

            await _asyncPubMultiPhaseSetupDone.PublishAsync(
                new GameMessages.MultiPhaseSetupDone
                {
                    Phase = "SetupFinished",
                    Category = "ConfigServiceProvider",
                    Success = true,
                },
                cancellationToken);
        }

        private async UniTask SetupEnd(
            bool success, CancellationToken cancellationToken = default)
        {
            Logger.LogEditorDebug(
                "{Method} reaches finally block",
                nameof(SetupEnd));

            _utcs.TrySetResult(success);

            await Task.CompletedTask;
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
