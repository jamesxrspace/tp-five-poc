using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MessagePipe;
using Microsoft.Extensions.Logging;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace TPFive.Game.Zone
{
    using TPFive.SCG.AsyncStartable.Abstractions;
    using TPFive.SCG.Bridge.Abstractions;
    using TPFive.SCG.DisposePattern.Abstractions;
    using TPFive.SCG.ServiceEco.Abstractions;

    using TPFive.Game.Logging;

    using CrossBridge = TPFive.Cross.Bridge;

    using GameLoggingUtility = TPFive.Game.Logging.Utility;

    // The order of attributes is not important
    [AsyncStartable(ExceptionList = "System.OperationCanceledException, System.Exception")]
    [Dispose]
    [ServiceProviderManagement]
    [ServiceProvidedBy(typeof(TPFive.Game.Zone.IServiceProvider))]
    [RegisterToContainer]
    public sealed partial class Service :
        IService
    {
        private const int ZoneServiceProviderIndex = (int)Game.ServiceProviderKind.Rank1ServiceProvider;

        private const int NullServiceProviderIndex = (int)ServiceProviderKind.NullServiceProvider;

        private readonly CompositeDisposable _compositeDisposable = new ();

        private UniTaskCompletionSource<bool> _utcs = new ();

        private readonly LifetimeScope _lifetimeScope;

        [Inject]
        public Service(
            ILoggerFactory loggerFactory)
        {
            Logger = GameLoggingUtility.CreateLogger<Service>(loggerFactory);

            _funcOperationCanceledException = HandleStartAsyncOperationCanceledException;
            _funcException = HandleStartAsyncException;

            var nullServiceProvider = new NullServiceProvider((s, args) =>
            {
                Logger.LogDebug(s, args);
            });

            _serviceProviderTable.Add(NullServiceProviderIndex, nullServiceProvider);
        }

        private Microsoft.Extensions.Logging.ILogger Logger { get; }

        public IServiceProvider NullServiceProvider => _serviceProviderTable[NullServiceProviderIndex] as IServiceProvider;

        public async UniTask<Scene> LoadRoomContentSceneAsync(
            string levelBundleId,
            int categoryOrder,
            int subCategoryOrder,
            LifetimeScope lifetimeScope,
            CancellationToken cancellationToken = default)
        {
            var serviceProvider = GetServiceProvider(ZoneServiceProviderIndex);

            return await serviceProvider.LoadRoomContentSceneAsync(
                levelBundleId,
                categoryOrder,
                subCategoryOrder,
                lifetimeScope,
                cancellationToken);
        }

        public async UniTask<bool> UnloadRoomContentSceneAsync(
            string levelBundleId,
            int categoryOrder,
            int subCategoryOrder,
            CancellationToken cancellationToken = default)
        {
            var serviceProvider = GetServiceProvider(ZoneServiceProviderIndex);

            return await serviceProvider.UnloadRoomContentSceneAsync(
                levelBundleId,
                categoryOrder,
                subCategoryOrder,
                cancellationToken);
        }

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
                "{Method}",
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
            if (_disposed)
            {
                return;
            }

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
