using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MessagePipe;
using Microsoft.Extensions.Logging;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;
using VContainer.Unity;

namespace TPFive.Extended.UnityAnalytics
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

    [AsyncStartable(ExceptionList = "System.OperationCanceledException, System.Exception")]
    [Dispose]
    [AsyncDispose]
    public sealed partial class ServiceProvider
    {
        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        private UniTaskCompletionSource<bool> _utcs = new UniTaskCompletionSource<bool>();

        //
        private readonly GameConfig.IServiceProvider _nullServiceProvider;

        private readonly IAsyncPublisher<GameMessages.MultiPhaseSetupDone> _asyncPubMultiPhaseSetupDone;

        public ServiceProvider(
            //
            ILoggerFactory loggerFactory,
            //
            GameConfig.IServiceProvider nullServiceProvider,
            //
            IAsyncPublisher<GameMessages.MultiPhaseSetupDone> asyncPubMultiPhaseSetupDone)
        {
            Assert.IsNotNull(loggerFactory);

            Logger = GameLoggingUtility.CreateLogger<ServiceProvider>(loggerFactory);

            _nullServiceProvider = nullServiceProvider;

            _asyncPubMultiPhaseSetupDone = asyncPubMultiPhaseSetupDone;

            //
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

            // await _asyncPubMultiPhaseSetupDone.PublishAsync(new GameMessages.MultiPhaseSetupDone
            // {
            //     Phase = 2, Category = "ConfigServiceProvider", Success = true
            // }, cancellationToken);
        }

        private async UniTask SetupEnd(
            bool success, CancellationToken cancellationToken = default)
        {
            Logger.LogEditorDebug(
                "{Method} reaches finally block",
                nameof(SetupEnd));

            _utcs.TrySetResult(success);
        }

        private async Task HandleStartAsyncOperationCanceledException(
            System.OperationCanceledException e,
            CancellationToken cancellationToken = default)
        {
            Logger.LogWarning("{Exception}", e);
        }

        private async Task HandleStartAsyncException(
            System.Exception e,
            CancellationToken cancellationToken = default)
        {
            Logger.LogError("{Exception}", e);
        }

        //
        private void HandleDispose(bool disposing)
        {
            if (disposing)
            {
                _compositeDisposable?.Dispose();
            }
        }

        private async ValueTask HandleDisposeAsync()
        {
            await Task.Yield();
        }
    }
}
