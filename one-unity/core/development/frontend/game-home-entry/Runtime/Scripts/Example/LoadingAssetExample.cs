using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UnityEngine;
using VContainer;
using AppEntryLifetimeScope = TPFive.Game.App.Entry.LifetimeScope;
using AppEntrySettings = TPFive.Game.App.Entry.Settings;
using IResourceService = TPFive.Game.Resource.IService;
using ISceneFlowService = TPFive.Game.SceneFlow.IService;

namespace TPFive.Home.Entry
{
    /// <summary>
    /// This is a testing example code for loading and unloading assets.
    /// REMOVE ME : When the Game.Resource.IService is ready.
    /// </summary>
    internal class LoadingAssetExample : MonoBehaviour
    {
        [SerializeField]
        private DecoData[] testData;

        [SerializeField]
        private int testDataIndex;

        private GameObject _showIconTarget;

        private IResourceService _resourceService;

        private Microsoft.Extensions.Logging.ILogger _logger;

        private DecoData CurrentData => testData[testDataIndex];

        [Inject]
        public void Construct(
            ILoggerFactory loggerFactory,
            IResourceService resourceService)
        {
            _logger = loggerFactory?.CreateLogger<LoadingAssetExample>();
            if (_logger == null)
            {
                throw new Exception($"[Error][{nameof(LoadingAssetExample)}] Failed creating logger");
            }

            if (resourceService == null)
            {
                throw new Exception($"[Error][{nameof(LoadingAssetExample)}] IResourceService is null");
            }

            _resourceService = resourceService;
        }

        [ContextMenu(nameof(TestLoadGameObject))]
        public void TestLoadGameObject()
        {
            LoadAssetAsync<GameObject>(CurrentData.GameObjectKey, destroyCancellationToken)
                .ContinueWith(prefab =>
                {
                    if (prefab == null)
                    {
                        return;
                    }

                    Instantiate(prefab, transform);
                })
                .SuppressCancellationThrow();
        }

        [ContextMenu(nameof(TestUnloadObject))]
        public void TestUnloadObject()
        {
            UnloadAsset<GameObject>(CurrentData.GameObjectKey, destroyCancellationToken)
                .SuppressCancellationThrow();
        }

        [ContextMenu(nameof(TestLoadIcon))]
        public void TestLoadIcon()
        {
            LoadAssetAsync<Texture>(CurrentData.IconKey, destroyCancellationToken)
                .ContinueWith(texture =>
                {
                    if (texture == null)
                    {
                        return;
                    }

                    if (_showIconTarget == null)
                    {
                        _showIconTarget = CreateShowIconObject();
                    }

                    _showIconTarget.GetComponent<MeshRenderer>().material.mainTexture = texture;
                })
                .SuppressCancellationThrow();
        }

        [ContextMenu(nameof(TestUnloadIcon))]
        public void TestUnloadIcon()
        {
            UnloadAsset<GameObject>(CurrentData.IconKey, destroyCancellationToken)
                .ContinueWith(_ =>
                {
                    if (_showIconTarget != null)
                    {
                        _showIconTarget.GetComponent<MeshRenderer>().material.mainTexture = Texture2D.redTexture;
                    }
                })
                .SuppressCancellationThrow();
        }

        private async UniTask<T> LoadAssetAsync<T>(string assetKey, CancellationToken token)
        {
            await _resourceService.LoadBundledDataAsync(CurrentData.BundleID, token);
            var asset = await _resourceService.LoadAssetAsync<T>(assetKey, token);
            if (asset == null)
            {
                _logger.LogError($"Failed load asset. assetKey: {assetKey}", assetKey);
            }

            return asset;
        }

        private async UniTask<bool> UnloadAsset<T>(string assetKey, CancellationToken token)
        {
            var success = await _resourceService.UnloadAssetAsync<T>(assetKey, token);
            if (!success)
            {
                _logger.LogError("Failed unload asset. assetKey: {assetKey}", assetKey);
            }

            return success;
        }

        private GameObject CreateShowIconObject()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.transform.position = Vector3.zero;
            go.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            go.transform.SetParent(this.transform);
            return go;
        }

        [Serializable]
        public class DecoData
        {
            [SerializeField]
            private string bundleID;

            [SerializeField]
            private string gameObjectKey;

            [SerializeField]
            private string iconKey;

            public string BundleID => bundleID;

            public string GameObjectKey => gameObjectKey;

            public string IconKey => iconKey;
        }
    }
}