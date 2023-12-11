using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TPFive.Game.Analytics
{
    using TPFive.SCG.AsyncStartable.Abstractions;
    using TPFive.SCG.DisposePattern.Abstractions;
    using TPFive.SCG.ServiceEco.Abstractions;

    using TPFive.Game.Logging;

    using GameLoggingUtility = TPFive.Game.Logging.Utility;

    // The order of attributes is not important
    [AsyncStartable(ExceptionList = "System.OperationCanceledException, System.Exception")]
    [Dispose]
    [ServiceProviderManagement]
    [ServiceProvidedBy(typeof(TPFive.Game.Analytics.IServiceProvider))]
    [RegisterToContainer]
    public sealed partial class Service :
        IService
    {
        private readonly LifetimeScope _lifetimeScope;
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        private UniTaskCompletionSource<bool> _utcs = new UniTaskCompletionSource<bool>();

        private const int GoogleAnalyticsServiceProvider = (int)ServiceProviderKind.Rank1ServiceProvider;

        [Inject]
        public Service(ILoggerFactory loggerFactory)
        {
            Logger = GameLoggingUtility.CreateLogger<Service>(loggerFactory);

            _funcOperationCanceledException = HandleStartAsyncOperationCanceledException;
            _funcException = HandleStartAsyncException;

            var nullServiceProvider = new NullServiceProvider((s, args) =>
            {
                Logger.LogDebug(s, args);
            });

            _serviceProviderTable.Add(0, nullServiceProvider);
        }

        private Microsoft.Extensions.Logging.ILogger Logger { get; }

        public IServiceProvider NullServiceProvider => _serviceProviderTable[0] as IServiceProvider;

        private async UniTask SetupBegin(CancellationToken cancellationToken = default)
        {
            Logger.LogEditorDebug(
                "{Method}",
                nameof(SetupBegin));

            await UniTask.CompletedTask;
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
            await Task.CompletedTask;
        }

        private async Task HandleStartAsyncException(
            System.Exception e, CancellationToken cancellationToken = default)
        {
            Logger.LogError("{Exception}", e);
            await Task.CompletedTask;
        }

        public void SetUser(string userID)
        {
            var serviceProvider = GetServiceProvider(GoogleAnalyticsServiceProvider);
            serviceProvider.SetUser(userID);
        }

        public void ScreenView(string screenName, string screenClass)
        {
            var serviceProvider = GetServiceProvider(GoogleAnalyticsServiceProvider);
            serviceProvider.ScreenView(screenName, screenClass);
        }

        private void HandleDispose(bool disposing)
        {
            if (disposing)
            {
                _compositeDisposable?.Dispose();

                _utcs.TrySetCanceled(_cancellationTokenSource.Token);
                _utcs = default;
                _disposed = true;
            }
        }
    }
}
