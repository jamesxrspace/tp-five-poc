using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UnityEngine;
using GameLoggingUtility = TPFive.Game.Logging.Utility;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using IResourceService = TPFive.Game.Resource.IService;
using Object = UnityEngine.Object;

namespace TPFive.Extended.Decoration
{
    /// <summary>
    /// Handle asset bundle loading and unloading.
    /// Handle the asset instance creating and releasing.
    /// </summary>
    public class GameObjectProvider : IAssetProvider<GameObject>
    {
        private readonly ILogger _logger;
        private readonly IResourceService _resourceService;
        private readonly string _bundleId;
        private readonly string _assetKey;
        private ScriptableObject _scriptableObject;
        private GameObject _prefab;
        private string _prefabRuntimeAssetKey;

        public GameObjectProvider(ILoggerFactory loggerFactory, IResourceService resourceService, string bundleId)
        {
            _logger = GameLoggingUtility.CreateLogger<GameObjectProvider>(loggerFactory);
            _resourceService = resourceService;
            _bundleId = bundleId;
            _assetKey = $"{bundleId}.asset";
        }

        public async UniTask<GameObject> LoadAsset(CancellationToken token = default)
        {
            await _resourceService.LoadBundledDataAsync(_bundleId, token);

            _scriptableObject = await _resourceService.LoadAssetAsync<ScriptableObject>(_assetKey, token);

            if (_scriptableObject == null)
            {
                throw new Exception($"Failed to load asset : ScriptableObject is not exist. _assetKey: {_assetKey}");
            }

            if (_scriptableObject is not Creator.BundleDetailData bdd ||
                bdd.bundleKind != Creator.BundleKind.SceneObject)
            {
                throw new Exception($"Failed to load asset : This bundle is not BundleDetailData or SceneObject. bundleId: {_bundleId}");
            }

            var assetReference = bdd.prefabs?.FirstOrDefault();
            if (assetReference == null)
            {
                throw new Exception($"Failed to load asset : The prefab is not exist. bundleId: {_bundleId}");
            }

            _prefabRuntimeAssetKey = assetReference.RuntimeKey as string;

            return await _resourceService.LoadAssetAsync<GameObject>(_prefabRuntimeAssetKey, token);
        }

        public async UniTask UnloadAsset(CancellationToken token = default)
        {
            _logger.LogDebug($"UnloadAsset start : bundleId: {_bundleId}, assetKey: {_assetKey}, prefabRuntimeAssetKey: {_prefabRuntimeAssetKey}");

            if (string.IsNullOrEmpty(_bundleId)
                || string.IsNullOrEmpty(_assetKey)
                || string.IsNullOrEmpty(_prefabRuntimeAssetKey))
            {
                throw new Exception($"_bundleId:{_bundleId} or _assetKey:{_assetKey} or _prefabRuntimeAssetKey:{_prefabRuntimeAssetKey} is null or empty.");
            }

            await _resourceService.UnloadAssetAsync<GameObject>(_prefabRuntimeAssetKey, token);
            await _resourceService.UnloadAssetAsync<ScriptableObject>(_assetKey, token);
            await _resourceService.UnloadBundleDataAsync(_bundleId, token);

            _scriptableObject = null;
            _prefab = null;

            _logger.LogDebug($"UnloadAsset finish : bundleId: {_bundleId}, assetKey: {_assetKey}, prefabRuntimeAssetKey: {_prefabRuntimeAssetKey}");
        }

        public async UniTask<GameObject> CreateInstance(CancellationToken token = default)
        {
            if (_prefab == null)
            {
                _prefab = await LoadAsset(token);
            }

            return CreateInstance(_prefab);
        }

        public UniTask<bool> ReleaseInstance(GameObject target, CancellationToken token = default)
        {
            if (target == null)
            {
                return UniTask.FromResult(false);
            }

            Object.Destroy(target);

            return UniTask.FromResult(true);
        }

        private static GameObject CreateInstance(GameObject prefab)
        {
            return Object.Instantiate(prefab);
        }
    }
}