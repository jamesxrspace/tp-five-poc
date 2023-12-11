using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using AppEntryLifetimeScope = TPFive.Game.App.Entry.LifetimeScope;
using AppEntrySettings = TPFive.Game.App.Entry.Settings;
using IResourceService = TPFive.Game.Resource.IService;
using ISceneFlowService = TPFive.Game.SceneFlow.IService;

namespace TPFive.Home.Entry.Example
{
    [Serializable]
    internal class ChangeSceneInfo
    {
        [SerializeField]
        private SceneInfo fromSceneInfo;

        [SerializeField]
        private SceneInfo nextSceneInfo;

        public SceneInfo FromSceneInfo => fromSceneInfo;

        public SceneInfo NextSceneInfo => nextSceneInfo;

        public override string ToString()
        {
            return $"From : Title={FromSceneInfo.BundleID} Category={FromSceneInfo.CategoryOrder}, SubOrder={FromSceneInfo.SubOrder}";
        }
    }

    [Serializable]
    internal class SceneInfo
    {
        [SerializeField]
        private string category;

        [SerializeField]
        private string bundleID;

        [SerializeField]
        private int categoryOrder;

        [SerializeField]
        private int subOrder;

        [SerializeField]
        private VContainer.Unity.LifetimeScope lifetimeScope;

        public string Category => category;

        public string BundleID => bundleID;

        public int CategoryOrder => categoryOrder;

        public int SubOrder => subOrder;

        public string AssetKey => BundleID + ".asset";

        public VContainer.Unity.LifetimeScope LifetimeScope => lifetimeScope;

        public override string ToString()
        {
            return $"FromCategory: {Category}, FromTitle: {BundleID}, FromCategoryOrder: {CategoryOrder}, FromSubOrder: {SubOrder}";
        }
    }

    internal class ChangeSceneExample : MonoBehaviour
    {
        [SerializeField]
        private ChangeSceneInfo changeSceneReq;

        [SerializeField]
        private SceneInfo additiveSceneInfo;

        private IResourceService _resourceService;

        private ISceneFlowService _sceneFlowService;

        private Microsoft.Extensions.Logging.ILogger _logger;

        [Inject]
        public void Construct(
            ILoggerFactory loggerFactory,
            IResourceService resourceService,
            ISceneFlowService sceneFlowService)
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
            _sceneFlowService = sceneFlowService;
        }

        // 目標，測試加載新場景、Unload 當前場景、A > B > C
        [ContextMenu(nameof(TestChangeSceneFromHome))]
        private async void TestChangeSceneFromHome()
        {
            await ChangeSceneFromHome(changeSceneReq, default);
        }

        [ContextMenu(nameof(TestChangeSceneToHome))]
        private async void TestChangeSceneToHome()
        {
            await ChangeSceneToHome(changeSceneReq, default);
        }

        [ContextMenu(nameof(TestChangeScene))]
        private async void TestChangeScene()
        {
            await ChangeSceneAsync(changeSceneReq, default);
        }

        [ContextMenu(nameof(TestLoadSceneAdditive))]
        private async void TestLoadSceneAdditive()
        {
            await LoadSceneAdditive(additiveSceneInfo, default);
        }

        private async UniTask ChangeSceneFromHome(ChangeSceneInfo changeScene, CancellationToken token)
        {
            try
            {
                var homeSceneInfo = changeScene.FromSceneInfo;
                var nextSceneInfo = changeScene.NextSceneInfo;

                await _sceneFlowService.UnloadSceneAsync(
                    homeSceneInfo.AssetKey,
                    homeSceneInfo.CategoryOrder,
                    subOrder: homeSceneInfo.SubOrder,
                    false,
                    token);

                await _resourceService.LoadBundledDataAsync(nextSceneInfo.BundleID, token);

                await _sceneFlowService.LoadSceneAsync(
                    nextSceneInfo.AssetKey,
                    LoadSceneMode.Additive,
                    nextSceneInfo.CategoryOrder,
                    nextSceneInfo.SubOrder,
                    nextSceneInfo.LifetimeScope,
                    true,
                    token);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
        }

        private async UniTask ChangeSceneToHome(ChangeSceneInfo changeScene, CancellationToken token)
        {
            try
            {
                var homeSceneInfo = changeScene.FromSceneInfo;
                var fromSceneInfo = changeScene.NextSceneInfo;

                await _sceneFlowService.UnloadSceneAsync(
                    fromSceneInfo.AssetKey,
                    fromSceneInfo.CategoryOrder,
                    subOrder: fromSceneInfo.SubOrder,
                    true,
                    token);

                await _sceneFlowService.LoadSceneAsync(
                    homeSceneInfo.BundleID,
                    LoadSceneMode.Additive,
                    homeSceneInfo.CategoryOrder,
                    homeSceneInfo.SubOrder,
                    homeSceneInfo.LifetimeScope,
                    false,
                    token);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
        }

        private async UniTask ChangeSceneAsync(ChangeSceneInfo changeScene, CancellationToken token)
        {
            try
            {
                // Special case to handle this .asset file.
                var fromSceneInfo = changeScene.FromSceneInfo;
                var nextSceneInfo = changeScene.NextSceneInfo;

                await _sceneFlowService.UnloadSceneAsync(
                    fromSceneInfo.AssetKey,
                    fromSceneInfo.CategoryOrder,
                    fromSceneInfo.SubOrder,
                    true,
                    token);

                await _resourceService.UnloadBundleDataAsync(fromSceneInfo.BundleID, token);

                await _resourceService.LoadBundledDataAsync(nextSceneInfo.BundleID, token);

                await _sceneFlowService.LoadSceneAsync(
                    nextSceneInfo.AssetKey,
                    LoadSceneMode.Additive,
                    nextSceneInfo.CategoryOrder,
                    nextSceneInfo.SubOrder,
                    nextSceneInfo.LifetimeScope,
                    true,
                    token);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
        }

        private async UniTask LoadSceneAdditive(SceneInfo newScene, CancellationToken token)
        {
            try
            {
                await _resourceService.LoadBundledDataAsync(newScene.BundleID, token);

                await _sceneFlowService.LoadSceneAsync(
                    newScene.AssetKey,
                    LoadSceneMode.Additive,
                    newScene.CategoryOrder,
                    newScene.SubOrder,
                    newScene.LifetimeScope,
                    true,
                    token);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
        }
    }
}
