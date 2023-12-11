using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace TPFive.Game.Resource
{
    using TPFive.Game.Logging;
    using TPFive.SCG.AsyncStartable.Abstractions;
    using TPFive.SCG.Bridge.Abstractions;
    using TPFive.SCG.DisposePattern.Abstractions;
    using TPFive.SCG.ServiceEco.Abstractions;
    using CrossBridge = TPFive.Cross.Bridge;
    using GameLoggingUtility = TPFive.Game.Logging.Utility;

    /// <summary>
    /// Service for resource load / unload.
    /// </summary>
    [AsyncStartable(ExceptionList = "System.OperationCanceledException, System.Exception")]
    [Dispose]
    [ServiceProviderManagement]
    [ServiceProvidedBy(typeof(TPFive.Game.Resource.IServiceProvider))]
    [RegisterToContainer]
    public sealed partial class Service : IService
    {
        private const int AddressableServiceProvider = (int)Game.ServiceProviderKind.Rank1ServiceProvider;
        private const int TextureLoaderServiceProviderKindIndex = (int)ServiceProviderKind.Rank2ServiceProvider;
        private CompositeDisposable _compositeDisposable = new CompositeDisposable();
        private UniTaskCompletionSource<bool> _utcs = new ();

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

            _serviceProviderTable.Add((int)Game.ServiceProviderKind.NullServiceProvider, nullServiceProvider);

            CrossBridge.LoadGameObject = LoadGameObject;
            CrossBridge.UnloadGameObject = UnloadGameObject;
            CrossBridge.LoadScriptableObject = LoadScriptableObject;
            CrossBridge.UnloadScriptableObject = UnloadScriptableObject;

            CrossBridge.GetLoadedGameObject = GetLoadedGameObject;
            CrossBridge.GetLoadedScene = GetLoadedScene;
            CrossBridge.GetLoadedScriptableObject = GetLoadedScriptableObject;
            CrossBridge.LoadBundledDataAsync = LoadBundledDataAsync;
        }

        public IServiceProvider NullServiceProvider => _serviceProviderTable[(int)Game.ServiceProviderKind.NullServiceProvider] as IServiceProvider;

        private Microsoft.Extensions.Logging.ILogger Logger { get; }

        [DelegateFrom(DelegateName = "LoadBundledDataAsync")]
        public async Cysharp.Threading.Tasks.UniTask LoadBundledDataAsync(
            string bundleId,
            System.Threading.CancellationToken cancellationToken = default)
        {
            var serviceProvider = GetServiceProvider(AddressableServiceProvider);

            await serviceProvider.LoadBundledDataAsync(bundleId, cancellationToken);
        }

        public async UniTask UnloadBundleDataAsync(
            string bundleId,
            CancellationToken cancellationToken = default)
        {
            var serviceProvider = GetServiceProvider(AddressableServiceProvider);

            await serviceProvider.UnloadBundleDataAsync(bundleId, cancellationToken);
        }

        public async UniTask<T> LoadAssetAsync<T>(
            object key,
            CancellationToken cancellationToken = default)
        {
            var serviceProvider = GetServiceProvider(AddressableServiceProvider);

            return await serviceProvider.LoadAssetAsync<T>(key, cancellationToken);
        }

        public async UniTask<bool> UnloadAssetAsync<T>(
            object key,
            CancellationToken cancellationToken = default)
        {
            var serviceProvider = GetServiceProvider(AddressableServiceProvider);

            return await serviceProvider.UnloadAssetAsync<T>(key, cancellationToken);
        }

        public async UniTask<Scene> LoadSceneAsync(
            object key,
            LoadSceneMode loadSceneMode,
            CancellationToken cancellationToken = default)
        {
            var serviceProvider = GetServiceProvider(AddressableServiceProvider);

            return await serviceProvider.LoadSceneAsync(key, loadSceneMode, cancellationToken);
        }

        public async UniTask<bool> UnloadSceneAsync(
            object key,
            CancellationToken cancellationToken = default)
        {
            var serviceProvider = GetServiceProvider(AddressableServiceProvider);

            return await serviceProvider.UnloadSceneAsync(key, cancellationToken);
        }

        [DelegateFrom(DelegateName = "GetLoadedScene")]
        public Scene GetLoadedScene(string name)
        {
            var serviceProvider = GetServiceProvider(AddressableServiceProvider);

            return serviceProvider.GetLoadedScene(name);
        }

        [DelegateFrom(DelegateName = "LoadGameObject")]
        public IEnumerator LoadGameObject(
            string name)
        {
            return LoadAssetAsync<GameObject>(name, _cancellationTokenSource.Token).AsUniTask().ToCoroutine();
        }

        [DelegateFrom(DelegateName = "UnloadGameObject")]
        public IEnumerator UnloadGameObject(
            string name)
        {
            return UnloadAssetAsync<GameObject>(name, _cancellationTokenSource.Token).AsUniTask().ToCoroutine();
        }

        [DelegateFrom(DelegateName = "LoadScriptableObject")]
        public IEnumerator LoadScriptableObject(
            string name)
        {
            return LoadAssetAsync<ScriptableObject>(name, _cancellationTokenSource.Token).AsUniTask().ToCoroutine();
        }

        [DelegateFrom(DelegateName = "UnloadScriptableObject")]
        public IEnumerator UnloadScriptableObject(
            string name)
        {
            return UnloadAssetAsync<ScriptableObject>(name, _cancellationTokenSource.Token).AsUniTask().ToCoroutine();
        }

        [DelegateFrom(DelegateName = "GetLoadedGameObject")]
        public GameObject GetLoadedGameObject(string name)
        {
            var serviceProvider = GetServiceProvider(AddressableServiceProvider);

            return serviceProvider.GetLoadedGameObject(name);
        }

        [DelegateFrom(DelegateName = "GetLoadedScriptableObject")]
        public ScriptableObject GetLoadedScriptableObject(string name)
        {
            var serviceProvider = GetServiceProvider(AddressableServiceProvider);

            return serviceProvider.GetLoadedScriptableObject(name);
        }

        public UniTask<TextureData> LoadTexture(object owner, TextureRequestContext context, CancellationToken token)
        {
            var serviceProvider = GetServiceProvider(TextureLoaderServiceProviderKindIndex);

            return serviceProvider.LoadTexture(owner, context, token);
        }

        public void ReleaseTexture(string resourceUrl, object owner)
        {
            var serviceProvider = GetServiceProvider(TextureLoaderServiceProviderKindIndex);

            serviceProvider.Release(resourceUrl, owner);
        }

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
