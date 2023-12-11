using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using HellTap.PoolKit;
using Microsoft.Extensions.Logging;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;
using VContainer.Unity;

namespace TPFive.Extended.PoolKit
{
    //
    using TPFive.SCG.AsyncStartable.Abstractions;
    using TPFive.SCG.DisposePattern.Abstractions;

    //
    using TPFive.Game.Logging;

    //
    using GameLoggingUtility = TPFive.Game.Logging.Utility;

    using GameConfig = TPFive.Game.Config;

    using GameObjectPool = TPFive.Game.ObjectPool;

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

        public ServiceProvider(
            //
            ILoggerFactory loggerFactory,
            //
            GameConfig.IService configService,
            //
            GameObjectPool.IServiceProvider nullServiceProvider)
        {
            Assert.IsNotNull(loggerFactory);

            Logger = GameLoggingUtility.CreateLogger<ServiceProvider>(loggerFactory);

            _configService = configService;

            _nullServiceProvider = nullServiceProvider;

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
            Logger.LogEditorDebug(
                "{Method} - disposing: {Disposing}",
                nameof(HandleDispose),
                disposing);

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
