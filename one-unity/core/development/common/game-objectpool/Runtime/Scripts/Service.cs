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

namespace TPFive.Game.ObjectPool
{
    using TPFive.SCG.AsyncStartable.Abstractions;
    using TPFive.SCG.Bridge.Abstractions;
    using TPFive.SCG.DisposePattern.Abstractions;
    using TPFive.SCG.ServiceEco.Abstractions;

    //
    using TPFive.Game.Logging;

    //
    using CrossBridge = TPFive.Cross.Bridge;

    using GameLoggingUtility = TPFive.Game.Logging.Utility;

    // The order of attributes is not important
    [AsyncStartable(ExceptionList = "System.OperationCanceledException, System.Exception")]
    [Dispose]
    [ServiceProviderManagement]
    [ServiceProvidedBy(typeof(TPFive.Game.ObjectPool.IServiceProvider))]
    [RegisterToContainer]
    public sealed partial class Service :
        IService
    {
        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        private UniTaskCompletionSource<bool> _utcs = new UniTaskCompletionSource<bool>();

        [Inject]
        public Service(
            //
            ILoggerFactory loggerFactory)
        {
            Logger = GameLoggingUtility.CreateLogger<Service>(loggerFactory);

            _funcOperationCanceledException = HandleStartAsyncOperationCanceledException;
            _funcException = HandleStartAsyncException;

            var nullServiceProvider = new NullServiceProvider((s, args) =>
            {
                Logger.LogDebug(s, args);
            });

            _serviceProviderTable.Add(0, nullServiceProvider);

            // GetNullServiceProvider = new NullServiceProvider();

            CrossBridge.SpawnFromPool = SpawnGameObjectFromPool;
            CrossBridge.DespawnToPool = DespawnGameObjectToPool;
        }

        //
        private Microsoft.Extensions.Logging.ILogger Logger { get; }

        public IServiceProvider NullServiceProvider => _serviceProviderTable[0] as IServiceProvider;

        private async UniTask SetupBegin(CancellationToken cancellationToken = default)
        {
            Logger.LogEditorDebug(
                "{Method}",
                nameof(SetupBegin));
        }

        private async UniTask SetupEnd(bool success, CancellationToken cancellationToken = default)
        {
            Logger.LogEditorDebug(
                "{Method}",
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

        //
        public T Spawn<T>(string name, T leasing)
        {
            var serviceProvider = GetServiceProvider(10);

            return serviceProvider.Spawn<T>(name, leasing);
        }

        public void Despawn<T>(T leased)
        {
            var serviceProvider = GetServiceProvider(10);

            serviceProvider.Despawn<T>(leased);
        }

        public GameObject SpawnFromPrefab(string name, GameObject prefab)
        {
            var serviceProvider = GetServiceProvider(10);

            return serviceProvider.SpawnFromPrefab(name, prefab);
        }

        public bool DespawnByGameObject(string name, GameObject inGO)
        {
            var serviceProvider = GetServiceProvider(10);

            return serviceProvider.DespawnByGameObject(name, inGO);
        }

        [DelegateFrom(DelegateName = "SpawnFromPool")]
        private GameObject SpawnGameObjectFromPool(string name, GameObject prefab)
        {
            return SpawnFromPrefab(name, prefab);
        }

        [DelegateFrom(DelegateName = "DespawnToPool")]
        private bool DespawnGameObjectToPool(string name, GameObject inGO)
        {
            return DespawnByGameObject(name, inGO);
        }
    }
}
