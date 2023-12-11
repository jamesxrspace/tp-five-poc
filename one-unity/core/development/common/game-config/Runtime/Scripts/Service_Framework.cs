using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MessagePipe;
using Microsoft.Extensions.Logging;
using UniRx;
using VContainer;

namespace TPFive.Game.Config
{
    using TPFive.Game.Logging;
    using TPFive.SCG.AsyncStartable.Abstractions;
    using TPFive.SCG.DisposePattern.Abstractions;
    using TPFive.SCG.ServiceEco.Abstractions;

    using GameLoggingUtility = TPFive.Game.Logging.Utility;
    using GameMessages = TPFive.Game.Messages;

    // The order of attributes is not important
    [AsyncStartable(ExceptionList = "System.OperationCanceledException, System.Exception")]
    [Dispose]
    [ServiceProviderManagement]
    [ServiceProvidedBy(typeof(TPFive.Game.Config.IServiceProvider))]
    [RegisterToContainer]
    public sealed partial class Service :
        IService
    {
        private readonly CompositeDisposable _compositeDisposable = new ();
        private readonly IAsyncPublisher<GameMessages.MultiPhaseSetupDone> _asyncPubMultiPhaseSetupDone;
        private readonly IAsyncSubscriber<GameMessages.MultiPhaseSetupDone> _asyncSubMultiPhaseSetupDone;

        private UniTaskCompletionSource<bool> _utcs = new ();
        private UniTaskCompletionSource<bool> _utcsWait = new ();

        private int _initializedProviderCount = default;

        [Inject]
        public Service(
            ILoggerFactory loggerFactory,
            IAsyncPublisher<GameMessages.MultiPhaseSetupDone> asyncPubMultiPhaseSetupDone,
            IAsyncSubscriber<GameMessages.MultiPhaseSetupDone> asyncSubMultiPhaseSetupDone)
        {
            Logger = GameLoggingUtility.CreateLogger<Service>(loggerFactory);

            _funcOperationCanceledException = HandleStartAsyncOperationCanceledException;
            _funcException = HandleStartAsyncException;

            var nullServiceProvider = new NullServiceProvider((s, args) =>
            {
                Logger.LogDebug(s, args);
            });

            _serviceProviderTable.Add(0, nullServiceProvider);

            _asyncPubMultiPhaseSetupDone = asyncPubMultiPhaseSetupDone;
            _asyncSubMultiPhaseSetupDone = asyncSubMultiPhaseSetupDone;
        }

        public IServiceProvider NullServiceProvider =>
            _serviceProviderTable[Constants.NullProviderIndex] as IServiceProvider;

        private Microsoft.Extensions.Logging.ILogger Logger { get; }

        // One provider is reserved for null.
        private int ProviderCount => _serviceProviderTable.Count - 1;

        private async UniTask SetupBegin(CancellationToken cancellationToken = default)
        {
            Logger.LogEditorDebug(
                "{Method}",
                nameof(SetupBegin));

            _asyncSubMultiPhaseSetupDone
                .Subscribe(async (x, ct) =>
                {
                    if (x.Phase.Equals("SetupFinished", System.StringComparison.Ordinal) &&
                        x.Category.Equals("ConfigServiceProvider", System.StringComparison.Ordinal) &&
                        x.Success)
                    {
                        ++_initializedProviderCount;
                        if (_initializedProviderCount == ProviderCount)
                        {
                            Logger.LogEditorDebug(
                            "{Method} - ProviderCount: {ProviderCount} _initializedProviderCount: {InitializedProviderCount}",
                            nameof(SetupBegin),
                            ProviderCount,
                            _initializedProviderCount);

                            await _asyncPubMultiPhaseSetupDone.PublishAsync(
                                new GameMessages.MultiPhaseSetupDone
                                {
                                    Phase = "SetupFinished",
                                    Category = "ConfigService",
                                    Success = true,
                                },
                                ct);
                        }
                    }
                })
                .AddTo(_compositeDisposable);
        }

        private async UniTask KeepWaiting(CancellationToken cancellationToken = default)
        {
            await _utcsWait.Task;
        }

        private async UniTask SetupEnd(bool success, CancellationToken cancellationToken = default)
        {
            Logger.LogEditorDebug(
                "{Method}",
                nameof(SetupEnd));

            _utcs.TrySetResult(success);

            await UniTask.CompletedTask;
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

        private void HandleDispose(bool disposing)
        {
            if (disposing)
            {
                _compositeDisposable?.Dispose();

                if (_cancellationTokenSource != null)
                {
                    _utcs?.TrySetCanceled(_cancellationTokenSource.Token);
                }

                _utcs = default;
                _disposed = true;
            }
        }
    }
}
