using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TPFive.Game.Extensions;
using TPFive.Game.Resource;
using TPFive.OpenApi.GameServer.Model;
using UnityEngine;

namespace TPFive.Extended.Decoration
{
    public sealed partial class ServiceProvider :
        Game.Decoration.IServiceProvider
    {
        // Due to the current architecture being designed as a multi-scene system, there may be multiple scenes,
        // each requiring different scene assets to be loaded/unloaded.
        // As a result, we use the "groupId" as a means of categorizing assets into groups.
        // You can utilize "groupId" as a way to categorize assets for a specific scene,
        // and manage the corresponding resources within the SceneAssetManager.
        private Dictionary<string/*group id*/, SceneAssetManager> _sceneAssetManagers = new ();

        public UniTask LoadAsset(string groupId, string bundleId, CancellationToken token = default)
        {
            return GetOrCreateManager(groupId).LoadAsset(bundleId, token);
        }

        public UniTask UnloadAsset(string groupId, string bundleId, CancellationToken token = default)
        {
            return GetOrCreateManager(groupId).UnloadAsset(bundleId, token);
        }

        public async UniTask<GameObject> InstantiateAsync(string groupId, string bundleId, CancellationToken token)
        {
            return await GetOrCreateManager(groupId).CreateInstance(bundleId, token);
        }

        public async UniTask<bool> DestroyAsync(string groupId, string bundleId, GameObject go, CancellationToken token = default)
        {
            return await GetOrCreateManager(groupId).DestroyInstance(bundleId, go, token);
        }

        public UniTask Release(string groupId, CancellationToken token = default)
        {
            if (_sceneAssetManagers.TryGetValue(groupId, out var manager))
            {
                return manager.ReleaseAll(token);
            }

            _logger.LogDebug($"Release : {groupId} is not exist.");

            return UniTask.CompletedTask;
        }

        public async UniTask ReleaseAll(CancellationToken token = default)
        {
            foreach (var manager in _sceneAssetManagers)
            {
                await manager.Value.ReleaseAll(token);
            }
        }

        public async UniTask<TPFive.Game.Resource.XRSceneObject> InstantiateAsync(string groupId, TPFive.Game.Resource.XRObject data, CancellationToken token = default)
        {
            var go = await InstantiateAsync(groupId, data.BundleId, token);
            var comp = go.GetOrAddComponent<XRSceneObject>();
            comp.FromXRObject(data);
            return comp;
        }

        public UniTask<bool> DestroyAsync(string groupId, TPFive.Game.Resource.XRSceneObject sceneObject, CancellationToken token = default)
        {
             return DestroyAsync(groupId, sceneObject.XRObject.BundleId, sceneObject.gameObject, token);
        }

        public IReadOnlyList<TPFive.Game.Resource.XRSceneObject> GetDecorations(string groupId)
        {
            var list = new List<XRSceneObject>();

            if (_sceneAssetManagers.TryGetValue(groupId, out var manager))
            {
                // GetGameObjects() returns a list of all instantiated GameObjects.
                // And check if the GameObject has a XRSceneObject component.
                foreach (var go in manager.GetGameObjects())
                {
                    if (go.TryGetComponent<XRSceneObject>(out var comp))
                    {
                        list.Add(comp);
                    }
                }
            }
            else
            {
                _logger.LogWarning($"GetDecorations fail : SceneAssetManager is not exit by groupId: {groupId}");
            }

            return list;
        }

        // Check current gameObject instance is exist or not in this group.
        public bool IsInstanceEmpty(string groupId, string bundleId)
        {
            if (!_sceneAssetManagers.TryGetValue(groupId, out var manager))
            {
                return true;
            }

            return manager.IsInstanceEmpty(bundleId);
        }

        public async UniTask<List<CategoryItem>> GetCategoryList(
            int size,
            int? offset,
            CancellationToken token)
        {
            var response = await _decorationApi.GetDecorationCategoryListAsync(size, offset, cancellationToken: token);
            if (!response.IsSuccess)
            {
                _logger.LogError("Failed to get decoration category data. error: {error}, error message: {message}", response.ErrorCode, response.Message);
                return null;
            }

            return response.Data.Items;
        }

        public async UniTask<List<OpenApi.GameServer.Model.Decoration>> GetDecorationList(
            int size,
            int? offset,
            string categoryId,
            CancellationToken token)
        {
            var response = await _decorationApi.GetDecorationItemsAsync(size, offset, categoryId, cancellationToken: token);
            if (!response.IsSuccess)
            {
                _logger.LogError("Failed to get decoration item data. error: {error}, Error message: {message}", response.ErrorCode, response.Message);
                return null;
            }

            return response.Data.Items;
        }

        private SceneAssetManager GetOrCreateManager(string groupId)
        {
            if (!_sceneAssetManagers.TryGetValue(groupId, out var manager))
            {
                manager = new SceneAssetManager(_loggerFactory, _resourceService, groupId);
                _sceneAssetManagers.Add(groupId, manager);
            }

            return manager;
        }
    }
}