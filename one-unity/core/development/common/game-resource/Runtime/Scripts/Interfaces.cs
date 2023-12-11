using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TPFive.Game.Resource
{
    public interface IService
    {
        IServiceProvider NullServiceProvider { get; }

        /// <summary>
        /// Mainly loading catalog.json
        /// </summary>
        /// <param name="bundleId"></param>
        /// <param name="cancellationToken"></param>
        /// <remarks>
        /// Passing in Id(will rename to bundleId) instead of the whole path.
        /// </remarks>
        UniTask LoadBundledDataAsync(
            string bundleId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Since catalog.json can not be removed, this method is not really doing
        /// catalog unloading.
        /// </summary>
        /// <param name="bundleId"></param>
        /// <param name="cancellationToken"></param>
        /// <remarks>
        /// Passing into Id instead of full path.
        /// </remarks>
        UniTask UnloadBundleDataAsync(
            string bundleId,
            CancellationToken cancellationToken = default);

        UniTask<T> LoadAssetAsync<T>(
            object key,
            CancellationToken cancellationToken = default);

        UniTask<bool> UnloadAssetAsync<T>(
            object key,
            CancellationToken cancellationToken = default);

        UniTask<Scene> LoadSceneAsync(
            object key,
            LoadSceneMode loadSceneMode,
            CancellationToken cancellationToken = default);

        UniTask<bool> UnloadSceneAsync(
            object key,
            CancellationToken cancellationToken = default);

        Scene GetLoadedScene(string name);

        IEnumerator LoadGameObject(
            string name);

        IEnumerator UnloadGameObject(
            string name);

        IEnumerator LoadScriptableObject(
            string name);

        IEnumerator UnloadScriptableObject(
            string name);

        GameObject GetLoadedGameObject(string name);

        ScriptableObject GetLoadedScriptableObject(string name);

        UniTask<TextureData> LoadTexture(object owner, TextureRequestContext context, CancellationToken token);

        void ReleaseTexture(string resourceUrl, object owner);
    }

    public interface IServiceProvider :
        TPFive.Game.IServiceProvider
    {
        UniTask LoadBundledDataAsync(
            string bundleId,
            CancellationToken cancellationToken = default);

        UniTask UnloadBundleDataAsync(
            string bundleId,
            CancellationToken cancellationToken = default);

        UniTask<T> LoadAssetAsync<T>(
            object key,
            CancellationToken cancellationToken = default);

        UniTask<bool> UnloadAssetAsync<T>(
            object key,
            CancellationToken cancellationToken = default);

        UniTask<Scene> LoadSceneAsync(
            object key,
            LoadSceneMode loadSceneMode,
            CancellationToken cancellationToken = default);

        UniTask<bool> UnloadSceneAsync(
            object key,
            CancellationToken cancellationToken = default);

        Scene GetLoadedScene(string name);

        GameObject GetLoadedGameObject(string name);

        ScriptableObject GetLoadedScriptableObject(string name);

        UniTask<TextureData> LoadTexture(object owner, TextureRequestContext context, CancellationToken token);

        void Release(string resourceUrl, object owner);
    }
}
