using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;
using VContainer.Unity;

namespace TPFive.Extended.I2Localization
{
    //
    using TPFive.SCG.AsyncStartable.Abstractions;
    using TPFive.SCG.DisposePattern.Abstractions;

    //
    using TPFive.Game.Logging;

    //
    // using AutoLogger = TPFive.SourceCodeGen.AutoLogger.Abstractions.AutoLoggerAttribute;

    //
    using GameLoggingUtility = TPFive.Game.Logging.Utility;

    //
    using GameConfig = TPFive.Game.Config;

    using GameLocalization = TPFive.Game.Localization;

    //
    [AsyncStartable(ExceptionList = "System.OperationCanceledException, System.Exception")]
    [AsyncDispose]
    [Dispose]
    // [AutoLogger]
    public sealed partial class ServiceProvider :
        TPFive.Game.IServiceProvider
    {
        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        private UniTaskCompletionSource<bool> _utcs = new UniTaskCompletionSource<bool>();

        //
        private readonly GameConfig.IService _configService;
        //
        private readonly GameLocalization.IServiceProvider _nullServiceProvider;

        //
        public ServiceProvider(
            //
            ILoggerFactory loggerFactory,
            //
            GameConfig.IService configService,
            //
            GameLocalization.IServiceProvider nullServiceProvider)
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
