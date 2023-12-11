using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;
using MessagePipe;
using Microsoft.Extensions.Logging;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

using GameConfig = TPFive.Game.Config;
using GameConfigService = TPFive.Game.Config.IService;
using ResourceService = TPFive.Game.Resource.IService;
using ZoneService = TPFive.Game.Zone.IService;

namespace TPFive.Room
{
    public class NetSceneLoader : INetworkSceneManager, INetSceneLoader
    {
        private const int DefaultContentSceneCategoryOrder = 6;
        private const int DefaultContentSceneSubCategoryOrder = 0;

        private readonly CompositeDisposable _compositeDisposable = new ();
        private readonly LifetimeScope _lifetimeScope;
        private readonly GameConfigService _configService;
        private readonly ResourceService _resourceService;
        private readonly ZoneService _zoneService;
        private readonly ISubscriber<Game.Messages.NotifyNetLoaderToUnload> _subNotifyNetLoaderToUnload;
        private readonly ILogger<NetSceneLoader> _logger;
        private int _contentSceneCategoryOrder;
        private int _contentSceneSubCategoryOrder;
        private NetworkRunner _runner;
        private UniTask _unloadSceneTask;
        private UniTask<Scene> _loadSceneTask;
        private bool _isUnloadingContentScene;
        private bool _isLoadingContentScene;
        private string _sceneAddressableKey;
        private Scene _scene;

        [Inject]
        public NetSceneLoader(
            ILoggerFactory loggerFactory,
            LifetimeScope lifetimeScope,
            GameConfigService configService,
            ResourceService resourceService,
            ZoneService zoneService,
            ISubscriber<Game.Messages.NotifyNetLoaderToUnload> subNotifyNetLoaderToUnload)
        {
            // Set dependency
            _logger = loggerFactory.CreateLogger<NetSceneLoader>();
            _lifetimeScope = lifetimeScope;
            _configService = configService;
            _resourceService = resourceService;
            _zoneService = zoneService;
            _subNotifyNetLoaderToUnload = subNotifyNetLoaderToUnload;

            // Resolve content scene category order
            ResolveContentSceneCategoryOrder();

            // Subscribe handler of message 'Game.Messages.NotifyNetLoaderToUnload'
            _subNotifyNetLoaderToUnload
                .Subscribe(msg =>
                {
                    if (_unloadSceneTask.Status != UniTaskStatus.Pending && !string.IsNullOrEmpty(_sceneAddressableKey))
                    {
                        UnloadContentScene(msg.OnCompleteAction);
                    }
                    else
                    {
                        msg.OnCompleteAction?.Invoke();
                    }
                })
                .AddTo(_compositeDisposable);
        }

        public bool IsBusy => _isLoadingContentScene || _isUnloadingContentScene;

        void INetworkSceneManager.Initialize(NetworkRunner runner)
        {
            _runner = runner;
        }

        bool INetworkSceneManager.IsReady(NetworkRunner runner)
        {
            return _runner != null && !IsBusy;
        }

        void INetworkSceneManager.Shutdown(NetworkRunner runner)
        {
            _runner = null;
            _compositeDisposable?.Dispose();
        }

        public void LoadScene(string targetSceneAddressableKey)
        {
            if (string.IsNullOrEmpty(targetSceneAddressableKey))
            {
                throw new ArgumentException($"{nameof(NetSceneLoader)}.{nameof(LoadScene)}: the given scene addressable key is null or empty.");
            }

            if (IsBusy)
            {
                throw new InvalidOperationException($"{nameof(NetSceneLoader)}.{nameof(LoadScene)}: {nameof(NetSceneLoader)} is busy(loading or unloading scene).");
            }

            if (_sceneAddressableKey == targetSceneAddressableKey)
            {
                // The identical scene has been loaded.
                // Therefore, skip loading scene, and notify the NetworkRunner of starting and finishing of loading scene directly.
                if (_runner != null)
                {
                    _runner.InvokeSceneLoadStart();
                }

                OnLoadSceneCompleted(ref _scene, targetSceneAddressableKey);
            }
            else
            {
                if (string.IsNullOrEmpty(_sceneAddressableKey))
                {
                    // No scene has been loaded yet and therefore load the target scene.
                    LoadContentScene(targetSceneAddressableKey);
                }
                else
                {
                    // A different scene has been loaded and therefore unload that scene before loading the target scene.
                    UnloadContentScene(() => LoadContentScene(targetSceneAddressableKey));
                }
            }
        }

        private void ResolveContentSceneCategoryOrder()
        {
            var (isFound, value) = _configService.GetSpecificProviderSystemObjectValue(GameConfig.Constants.RuntimeLocalProviderKind, "ContentEntry");
            if (isFound && value is Game.SceneProperty sceneProperty)
            {
                _contentSceneCategoryOrder = sceneProperty.categoryOrder;
                _contentSceneSubCategoryOrder = sceneProperty.subOrder;
            }
            else
            {
                _contentSceneCategoryOrder = DefaultContentSceneCategoryOrder;
                _contentSceneSubCategoryOrder = DefaultContentSceneSubCategoryOrder;
            }
        }

        private void UnloadContentScene(Action onCompleted)
        {
            if (IsBusy)
            {
                throw new InvalidOperationException($"{nameof(NetSceneLoader)}.{nameof(UnloadContentScene)}: {nameof(NetSceneLoader)} is busy(loading or unloading scene).");
            }

            if (string.IsNullOrEmpty(_sceneAddressableKey))
            {
                throw new InvalidOperationException($"{nameof(NetSceneLoader)}.{nameof(UnloadContentScene)}: {nameof(NetSceneLoader)} has no content scene to unload.");
            }

            _unloadSceneTask = _zoneService.UnloadRoomContentSceneAsync(_sceneAddressableKey, _contentSceneCategoryOrder, _contentSceneSubCategoryOrder);
            _isUnloadingContentScene = true;
            _unloadSceneTask.ContinueWith(() =>
            {
                _isUnloadingContentScene = false;
                _sceneAddressableKey = default;
                _scene = default;
                onCompleted?.Invoke();
            });
        }

        private void LoadContentScene(string targetSceneAddressableKey)
        {
            if (IsBusy)
            {
                throw new InvalidOperationException($"{nameof(NetSceneLoader)}.{nameof(LoadContentScene)}: {nameof(NetSceneLoader)} is busy(loading or unloading scene).");
            }

            if (!string.IsNullOrEmpty(_sceneAddressableKey))
            {
                throw new InvalidOperationException($"{nameof(NetSceneLoader)}.{nameof(LoadContentScene)}: {nameof(NetSceneLoader)} Some scene has been loaded.");
            }

            if (string.IsNullOrEmpty(targetSceneAddressableKey))
            {
                throw new ArgumentException($"{nameof(NetSceneLoader)}.{nameof(LoadContentScene)}: Target scene is undefined.");
            }

            // Inform the NetworkRunner that the scene-loading process is gonna start.
            if (_runner != null)
            {
                _runner.InvokeSceneLoadStart();
            }

            // Start loading content scene asynchronously
            _loadSceneTask = _zoneService.LoadRoomContentSceneAsync(
                    targetSceneAddressableKey,
                    _contentSceneCategoryOrder,
                    _contentSceneSubCategoryOrder,
                    _lifetimeScope);
            _isLoadingContentScene = true;
            _loadSceneTask.ContinueWith(scene => OnLoadSceneCompleted(ref scene, targetSceneAddressableKey));
        }

        private void OnLoadSceneCompleted(ref Scene scene, string targetSceneAddressableKey)
        {
            if (!scene.IsValid())
            {
                throw new ArgumentException($"{nameof(NetSceneLoader)}.{nameof(OnLoadSceneCompleted)}: The scene is invalid.");
            }

            if (string.IsNullOrEmpty(targetSceneAddressableKey))
            {
                throw new ArgumentException($"{nameof(NetSceneLoader)}.{nameof(OnLoadSceneCompleted)}: The addressable key of the target content scene is undefined.");
            }

            _isLoadingContentScene = false;
            _logger.LogInformation("Succeeded loading scene: {SceneName}", scene.name);
            SceneManager.SetActiveScene(scene);
            LightProbes.TetrahedralizeAsync();
            _sceneAddressableKey = targetSceneAddressableKey;
            _scene = scene;

            // It's necessary to check if Fusion is still alive because it may be shutdown(disconnected) during the asynchronous operation of loading scene.
            if (_runner != null)
            {
                // Register all the NetworkObject(s) found in the scene with the NetworkRunner
                _runner.RegisterSceneObjects(FindNetworkObjects(scene, (netObj) => !_runner.Exists(netObj)));

                // Notify the NetworkRunner the scene has been loaded successfully.
                _runner.InvokeSceneLoadDone();
            }
        }

        // Find all NetworkObject(s) in the given scene
        private List<NetworkObject> FindNetworkObjects(Scene scene, System.Predicate<NetworkObject> predicator)
        {
            var result = new List<NetworkObject>();
            var rootGameObjects = scene.GetRootGameObjects();

            var netObjs = new List<NetworkObject>();
            foreach (var rootGameObject in rootGameObjects)
            {
                netObjs.Clear();
                rootGameObject.GetComponentsInChildren(true, netObjs);

                foreach (var netObj in netObjs)
                {
                    if (netObj.NetworkGuid.IsValid &&
                        netObj.Flags.IsSceneObject() &&
                        (netObj.gameObject.activeInHierarchy || netObj.Flags.IsActivatedByUser()) &&
                        predicator(netObj))
                    {
                        result.Add(netObj);
                    }
                }
            }

            return result;
        }
    }
}
