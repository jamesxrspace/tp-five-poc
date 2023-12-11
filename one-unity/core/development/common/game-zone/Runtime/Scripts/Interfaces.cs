using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace TPFive.Game.Zone
{
    public interface IService
    {
        IServiceProvider NullServiceProvider { get; }

        UniTask<Scene> LoadRoomContentSceneAsync(
            string levelBundleId,
            int categoryOrder,
            int subCategoryOrder,
            LifetimeScope lifetimeScope,
            CancellationToken cancellationToken = default);

        UniTask<bool> UnloadRoomContentSceneAsync(
            string levelBundleId,
            int categoryOrder,
            int subCategoryOrder,
            CancellationToken cancellationToken = default);
    }

    public interface IServiceProvider :
        TPFive.Game.IServiceProvider
    {
        UniTask<Scene> LoadRoomContentSceneAsync(
            string levelBundleId,
            int categoryOrder,
            int subCategoryOrder,
            LifetimeScope lifetimeScope,
            CancellationToken cancellationToken = default);

        UniTask<bool> UnloadRoomContentSceneAsync(
            string levelBundleId,
            int categoryOrder,
            int subCategoryOrder,
            CancellationToken cancellationToken = default);
    }
}
