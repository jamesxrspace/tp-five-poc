using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using VContainer;

namespace TPFive.Extended.Addressable
{
    using TPFive.Game.Logging;
    using TPFive.Game.Resource;
    using GameLoggingUtility = TPFive.Game.Logging.Utility;

    using GameResource = TPFive.Game.Resource;

    public sealed partial class ServiceProvider :
        GameResource.IServiceProvider
    {
        private readonly Dictionary<string, SceneInstance> _sceneTable = new ();

        private readonly Dictionary<string, GameObject> _gameObjectTable = new ();

        private readonly Dictionary<string, ScriptableObject> _scriptableObjectTable = new ();

        private readonly Dictionary<string, IResourceLocator> _cachedResourceLocatorTable = new ();

        // Might replace the above few tables if these two work as expected.
        private readonly Dictionary<object, AsyncOperationHandle<SceneInstance>> _objectKeySceneHandleTable = new ();

        private readonly Dictionary<object, AsyncOperationHandle> _objectKeyHandleTable = new ();

        public async UniTask LoadBundledDataAsync(
            string bundleId,
            CancellationToken cancellationToken = default)
        {
            Logger.LogEditorDebug(
                "{Method} - Start",
                nameof(LoadBundledDataAsync));

            var catalogPath = GetCatalogPath(bundleId);

            var resourceLocator = await Addressables.LoadContentCatalogAsync(
                catalogPath,
                true).Task;

            var r = _cachedResourceLocatorTable.TryAdd(catalogPath, resourceLocator);

            if (!r)
            {
                return;
            }

            foreach (var locatorKey in resourceLocator.Keys)
            {
                Logger.LogEditorDebug(
                    "{Method} - Locator Key: {LocatorKey}",
                    nameof(LoadBundledDataAsync),
                    locatorKey);

                resourceLocator.Locate(
                    locatorKey,
                    typeof(UnityEngine.Object),
                    out var locations);
                if (locations == null)
                {
                    continue;
                }

                foreach (var location in locations)
                {
                    var locationKey = location.InternalId;
                    Logger.LogEditorDebug(
                        "{Method} - Location Key: {LocationKey}, ProviderId: {ProviderId} ResourceType: {RT}  Primary Key: {PrimaryKey}",
                        nameof(LoadBundledDataAsync),
                        locationKey,
                        location.ProviderId,
                        location.ResourceType.ToString(),
                        location.PrimaryKey);
                }
            }
        }

        public async UniTask UnloadBundleDataAsync(
            string bundleId,
            CancellationToken cancellationToken = default)
        {
            var catalogPath = GetCatalogPath(bundleId);
            var result = _cachedResourceLocatorTable.TryGetValue(catalogPath, out var resourceLocator);
            if (result)
            {
                // Keep the commented code here as it may or may not necessary.
                // Addressables.ClearResourceLocators();
                await Addressables.CleanBundleCache(new List<string> { resourceLocator.LocatorId }).Task;

                Addressables.RemoveResourceLocator(resourceLocator);
                Addressables.ClearDependencyCacheAsync(resourceLocator);
                _cachedResourceLocatorTable.Remove(catalogPath);
            }
        }

        public async UniTask<T> LoadAssetAsync<T>(
            object key,
            CancellationToken cancellationToken = default)
        {
            var utcs = new UniTaskCompletionSource<T>();
            var result = default(T);

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var handle = Addressables.LoadAssetAsync<T>(key);

                var added = _objectKeyHandleTable.TryAdd(key, handle);
                if (added)
                {
                    await ShowDownloadStatus(key, handle);

                    result = await handle.Task;
                }
                else
                {
                    Logger.LogWarning(
                        "{Method} Add failed due to finding duplicated asset {Key} in asset table",
                        nameof(UnloadAssetAsync),
                        key);

                    var r = await _objectKeyHandleTable[key].Task;
                    result = (T)r;
                }
            }
            catch (System.OperationCanceledException e)
            {
                Logger.LogWarning("{Exception}", e);
            }
            catch (System.Exception e)
            {
                Logger.LogError("{Exception}", e);
            }

            return result;
        }

        public async UniTask<bool> UnloadAssetAsync<T>(
            object key,
            CancellationToken cancellationToken = default)
        {
            var result = false;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                result = _objectKeyHandleTable.TryGetValue(key, out var asset);
                if (result)
                {
                    Addressables.Release(asset);
                    _objectKeyHandleTable.Remove(key);
                }
                else
                {
                    Logger.LogWarning(
                        "{Method} Remove failed due to unable to find asset {Key} in asset table",
                        nameof(UnloadAssetAsync),
                        key);
                }
            }
            catch (System.OperationCanceledException e)
            {
                Logger.LogWarning("{Exception}", e);
            }
            catch (System.Exception e)
            {
                Logger.LogError("{Exception}", e);
            }

            return false;
        }

        public async UniTask<Scene> LoadSceneAsync(
            object key,
            LoadSceneMode loadSceneMode,
            CancellationToken cancellationToken = default)
        {
            var scene = default(Scene);

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var handle = Addressables.LoadSceneAsync(key, loadSceneMode);
                var added = _objectKeySceneHandleTable.TryAdd(key, handle);

                if (added)
                {
                    await ShowDownloadStatus(key, handle);

                    var sceneInstance = await handle.Task;
                    scene = sceneInstance.Scene;
                }
                else
                {
                    Logger.LogWarning(
                        "{Method} Add failed due to finding duplicated scene {Key} in scene table",
                        nameof(UnloadAssetAsync),
                        key);
                }
            }
            catch (System.OperationCanceledException e)
            {
                Logger.LogWarning("{Exception}", e);
            }
            catch (System.Exception e)
            {
                Logger.LogError("{Exception}", e);
            }

            return scene;
        }

        public async UniTask<bool> UnloadSceneAsync(
            object key,
            CancellationToken cancellationToken = default)
        {
            var result = false;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                result = _objectKeySceneHandleTable.TryGetValue(key, out var asset);
                if (result)
                {
                    Addressables.Release(asset);
                    _objectKeySceneHandleTable.Remove(key);
                }
                else
                {
                    Logger.LogWarning(
                        "{Method} Remove failed due to unable to find asset {Key} in scene table",
                        nameof(UnloadAssetAsync),
                        key);
                }
            }
            catch (System.OperationCanceledException e)
            {
                Logger.LogWarning("{Exception}", e);
            }
            catch (System.Exception e)
            {
                Logger.LogError("{Exception}", e);
            }

            return false;
        }

        public Scene GetLoadedScene(string name)
        {
            var result = _sceneTable.TryGetValue(name, out SceneInstance sceneInstance);
            if (!result)
            {
                return default;
            }

            return sceneInstance.Scene;
        }

        public GameObject GetLoadedGameObject(string name)
        {
            var result = _gameObjectTable.TryGetValue(name, out GameObject gameObject);
            if (!result)
            {
                return default;
            }

            return gameObject;
        }

        public ScriptableObject GetLoadedScriptableObject(string name)
        {
            var result = _scriptableObjectTable.TryGetValue(name, out ScriptableObject scriptableObject);
            if (!result)
            {
                return default;
            }

            return scriptableObject;
        }

        public UniTask<TextureData> LoadTexture(object owner, TextureRequestContext context, CancellationToken token)
        {
            throw new System.NotImplementedException();
        }

        public void Release(string resourceUrl, object owner)
        {
            throw new System.NotImplementedException();
        }

        private async UniTask SetupAddressables(CancellationToken cancellationToken = default)
        {
            Logger.LogEditorDebug(
                "{Method}",
                nameof(SetupAddressables));

            var resourceLocator = await Addressables.InitializeAsync().Task;

            foreach (var locatorKey in resourceLocator.Keys)
            {
                resourceLocator.Locate(locatorKey, typeof(UnityEngine.Object), out var locations);
                if (locations == null)
                {
                    continue;
                }

                foreach (var location in locations)
                {
                    var locationKey = location.InternalId;
                    Logger.LogEditorDebug(
                        "{Method} - Location Key: {LocationKey} Location: {Location}",
                        nameof(SetupAddressables),
                        locationKey,
                        location);
                }
            }
        }
    }
}
