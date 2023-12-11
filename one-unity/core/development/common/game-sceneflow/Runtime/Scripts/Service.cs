using System.Collections;
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

namespace TPFive.Game.SceneFlow
{
    using TPFive.SCG.Bridge.Abstractions;
    using TPFive.SCG.ServiceEco.Abstractions;

    using CrossBridge = TPFive.Cross.Bridge;
    using GameLoggingUtility = TPFive.Game.Logging.Utility;
    using GameMessages = TPFive.Game.Messages;

    public struct ChangeScene
    {
        public string FromCategory;
        public string FromTitle;
        public int FromCategoryOrder;
        public int FromSubOrder;
        public string ToCategory;
        public string ToTitle;
        public int ToCategoryOrder;
        public int ToSubOrder;
        public bool LoadFromScriptableObject;
        public VContainer.Unity.LifetimeScope LifetimeScope;

        // As scene might be loaded and unloaded from remote bundle data or regular scene,
        // adding following fields to help distinguish.
        public bool FromScriptableObject;
        public bool ToScriptableObject;

        public override string ToString()
        {
            return $"FromCategory: {FromCategory}, FromTitle: {FromTitle}, FromCategoryOrder: {FromCategoryOrder}, FromSubOrder: {FromSubOrder}, ToCategory: {ToCategory}, ToTitle: {ToTitle}, ToCategoryOrder: {ToCategoryOrder}, ToSubOrder: {ToSubOrder}, LoadFromScriptableObject: {LoadFromScriptableObject}";
        }
    }

    // The order of attributes is not important
    [ServiceProviderManagement]
    [ServiceProvidedBy(typeof(TPFive.Game.SceneFlow.IServiceProvider))]
    [RegisterToContainer]
    public sealed partial class Service :
        IService
    {
        private const int SceneActivityProvider = (int)Game.ServiceProviderKind.Rank1ServiceProvider;

        private readonly CompositeDisposable _compositeDisposable = new ();
        private readonly LifetimeScope _lifetimeScope;
        private readonly ISubscriber<ChangeScene> _subChangeScene;

        private UniTaskCompletionSource<bool> _utcs = new ();

        [Inject]
        public Service(
            ILoggerFactory loggerFactory,
            LifetimeScope lifetimeScope,
            ISubscriber<ChangeScene> subChangeScene)
        {
            Logger = GameLoggingUtility.CreateLogger<Service>(loggerFactory);

            _lifetimeScope = lifetimeScope;

            _funcOperationCanceledException = HandleStartAsyncOperationCanceledException;
            _funcException = HandleStartAsyncException;

            var nullServiceProvider = new NullServiceProvider((s, args) =>
            {
                Logger.LogDebug(s, args);
            });

            _serviceProviderTable.Add(0, nullServiceProvider);

            _subChangeScene = subChangeScene;

            CrossBridge.LoadScene = LoadSceneToBeAdjusted;
            CrossBridge.UnloadScene = UnloadSceneToBeAdjusted;
            CrossBridge.ChangeScene = ChangeScene;
        }

        public IServiceProvider NullServiceProvider => _serviceProviderTable[0] as IServiceProvider;

        private Microsoft.Extensions.Logging.ILogger Logger { get; }

        public async UniTask<Scene> LoadSceneAsync(
            object name,
            LoadSceneMode loadSceneMode,
            int categoryOrder,
            int subOrder,
            LifetimeScope lifetimeScope,
            bool loadFromScriptableObject = false,
            CancellationToken cancellationToken = default)
        {
            var serviceProvider = GetServiceProvider(SceneActivityProvider);

            // Delegate to call resource load scene async for now
            return await serviceProvider.LoadSceneAsync(
                new LoadContext
                {
                    Title = name,
                    Category = "Creator",
                    CategoryOrder = categoryOrder,
                    SubOrder = subOrder,
                    LoadSceneMode = loadSceneMode,
                    LifetimeScope = lifetimeScope,
                },
                loadFromScriptableObject,
                cancellationToken);
        }

        public async UniTask<bool> UnloadSceneAsync(
            object name,
            int categoryOrder,
            int subOrder,
            bool loadFromScriptableObject = false,
            CancellationToken cancellationToken = default)
        {
            var serviceProvider = GetServiceProvider(SceneActivityProvider);

            // Delegate to call resource unload scene async for now
            return await serviceProvider.UnloadSceneAsync(
                new UnloadContext
                {
                    Title = name,
                    Category = "Creator",
                    CategoryOrder = categoryOrder,
                    SubOrder = subOrder,
                },
                loadFromScriptableObject,
                cancellationToken);
        }

        public void SetupTopmostLifetimeScope(LifetimeScope lifetimeScope)
        {
            var serviceProvider = GetServiceProvider(SceneActivityProvider);

            serviceProvider.SetupTopmostLifetimeScope(lifetimeScope);
        }

        public IEnumerator LoadScene(
            string name,
            LoadSceneMode loadSceneMode,
            int categoryOrder,
            int subOrder,
            LifetimeScope lifetimeScope,
            bool loadFromScriptableObject = false)
        {
            return LoadSceneAsync(
                name,
                loadSceneMode,
                categoryOrder,
                subOrder,
                lifetimeScope,
                loadFromScriptableObject,
                _cancellationTokenSource.Token).AsUniTask().ToCoroutine();
        }

        public IEnumerator UnloadScene(
            string name,
            int categoryOrder,
            int subOrder,
            bool loadFromScriptableObject = false)
        {
            return UnloadSceneAsync(
                name,
                categoryOrder,
                subOrder,
                loadFromScriptableObject,
                _cancellationTokenSource.Token).AsUniTask().ToCoroutine();
        }

        public async UniTask<Scene> ReplaceSceneAsync(
            object currentName,
            int currentCategoryOrder,
            int currentSubOrder,
            object nextName,
            int nextCategoryOrder,
            int nextSubOrder,
            LifetimeScope lifetimeScope)
        {
            await UnloadSceneAsync(currentName, currentCategoryOrder, currentSubOrder);
            return await LoadSceneAsync(
                nextName,
                LoadSceneMode.Additive,
                nextCategoryOrder,
                nextSubOrder,
                lifetimeScope);
        }

        [DelegateFrom(DelegateName = "LoadScene")]
        private IEnumerator LoadSceneToBeAdjusted(
            string name,
            LoadSceneMode loadSceneMode,
            int categoryOrder,
            int subOrder,
            UnityEngine.MonoBehaviour lifetimeScope)
        {
            yield return LoadScene(
                name,
                loadSceneMode,
                categoryOrder,
                subOrder,
                (LifetimeScope)lifetimeScope);
        }

        [DelegateFrom(DelegateName = "UnloadScene")]
        private IEnumerator UnloadSceneToBeAdjusted(
            string name,
            int categoryOrder,
            int subOrder)
        {
            yield return UnloadScene(
                name,
                categoryOrder,
                subOrder);
        }

        [DelegateFrom(DelegateName = "ChangeScene")]
        private void ChangeScene(
            string fromCategory,
            string fromTitle,
            int fromCategoryOrder,
            int fromSuborder,
            string toCategory,
            string toTitle,
            int toCategoryOrder,
            int toSuborder,
            MonoBehaviour lifeTimeScope)
        {
            var changeSceneRequest = new ChangeScene()
            {
                FromTitle = fromTitle,
                FromCategory = fromCategory,
                FromCategoryOrder = fromCategoryOrder,
                FromSubOrder = fromSuborder,
                ToCategory = toCategory,
                ToTitle = toTitle,
                ToCategoryOrder = toCategoryOrder,
                ToSubOrder = toSuborder,
                LifetimeScope = lifeTimeScope != null ? (LifetimeScope)lifeTimeScope : _lifetimeScope,
            };

            HandleChangeScene()?.Invoke(changeSceneRequest);
        }
    }
}
