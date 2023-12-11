using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TPFive.Game.Resource;
using UnityEngine;

namespace TPFive.Game.Decoration
{
    /// <summary>
    /// DecorationService Interface has some responsibilities.
    /// 1. Load or Unload specific Decoration asset bundle from remote.
    /// 2. Manage each Decoration gameObject lifecycle into a group in a scene.
    /// 3. Get decoration category list and decoration list from server.
    /// </summary>
    public interface IService
    {
        IServiceProvider NullServiceProvider { get; }

        UniTask LoadAsset(string groupId, string bundleId, CancellationToken token = default);

        UniTask UnloadAsset(string groupId, string bundleId, CancellationToken token = default);

        UniTask<GameObject> InstantiateAsync(string groupId, string bundleID, CancellationToken token = default);

        UniTask<bool> DestroyAsync(string groupId, string bundleId, GameObject go, CancellationToken token = default);

        UniTask Release(string groupId, CancellationToken token = default);

        UniTask ReleaseAll(CancellationToken token = default);

        UniTask<XRSceneObject> InstantiateAsync(string groupId, XRObject data, CancellationToken token = default);

        UniTask<bool> DestroyAsync(string groupId, XRSceneObject sceneObject, CancellationToken token = default);

        IReadOnlyList<XRSceneObject> GetDecorations(string groupId);

        UniTask<List<OpenApi.GameServer.Model.CategoryItem>> GetCategoryListAsync(int size, int? offset = default, CancellationToken token = default);

        UniTask<List<OpenApi.GameServer.Model.Decoration>> GetDecorationListAsync(int size, int? offset = default, string categoryId = default, CancellationToken token = default);
    }

    public interface IServiceProvider :
        TPFive.Game.IServiceProvider
    {
        UniTask LoadAsset(string groupId, string bundleId, CancellationToken token = default);

        UniTask UnloadAsset(string groupId, string bundleId, CancellationToken token = default);

        UniTask<GameObject> InstantiateAsync(string groupId, string bundleId, CancellationToken token = default);

        UniTask<bool> DestroyAsync(string groupId, string bundleId, GameObject go, CancellationToken token = default);

        UniTask Release(string groupId, CancellationToken token = default);

        UniTask ReleaseAll(CancellationToken token = default);

        UniTask<TPFive.Game.Resource.XRSceneObject> InstantiateAsync(string groupId, TPFive.Game.Resource.XRObject data, CancellationToken token = default);

        UniTask<bool> DestroyAsync(string groupId, TPFive.Game.Resource.XRSceneObject sceneObject, CancellationToken token = default);

        IReadOnlyList<TPFive.Game.Resource.XRSceneObject> GetDecorations(string groupId);

        UniTask<List<OpenApi.GameServer.Model.CategoryItem>> GetCategoryList(int size, int? offset = default, CancellationToken token = default);

        UniTask<List<OpenApi.GameServer.Model.Decoration>> GetDecorationList(int size, int? offset = default, string categoryId = default, CancellationToken token = default);
    }
}