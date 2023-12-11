using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using Microsoft.Extensions.Logging;
using UniRx;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace TPFive.Extended.Zone
{
    using TPFive.Game.Logging;

    using GameZone = TPFive.Game.Zone;

    public sealed partial class ServiceProvider :
        GameZone.IServiceProvider
    {
        public async UniTask<Scene> LoadRoomContentSceneAsync(
            string levelBundleId,
            int categoryOrder,
            int subCategoryOrder,
            LifetimeScope lifetimeScope,
            CancellationToken cancellationToken = default)
        {
            var contentSceneLoading = true;
            var cts = new CancellationTokenSource();
            cts.CancelAfterSlim(System.TimeSpan.FromSeconds(_loadWaitTimeout));
            var scene = default(Scene);

            var disposable = _subContentLevelFullyLoaded
                .Subscribe(_ => contentSceneLoading = false);

            try
            {
                await _resourceService.LoadBundledDataAsync(levelBundleId);
                scene = await _sceneFlowService.LoadSceneAsync(
                    $"{levelBundleId}.asset",
                    LoadSceneMode.Additive,
                    categoryOrder,
                    subCategoryOrder,
                    lifetimeScope,
                    true);

                await UniTask.WaitUntil(() => !contentSceneLoading);
            }
            catch (System.OperationCanceledException e)
            {
                Logger.LogWarning("{Exception}", e);
            }
            catch (System.Exception e)
            {
                Logger.LogError("{Exception}", e);
                if (!scene.IsValid())
                {
                    throw;
                }
            }
            finally
            {
                disposable?.Dispose();
            }

            return scene;
        }

        public async UniTask<bool> UnloadRoomContentSceneAsync(
            string levelBundleId,
            int categoryOrder,
            int subCategoryOrder,
            CancellationToken cancellationToken = default)
        {
            var result = await _sceneFlowService.UnloadSceneAsync($"{levelBundleId}.asset", categoryOrder, subCategoryOrder, true);
            await _resourceService.UnloadBundleDataAsync(levelBundleId);

            return result;
        }
    }
}
