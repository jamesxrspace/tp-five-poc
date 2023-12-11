using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace TPFive.Game.SceneFlow
{
    public interface IService
    {
        IServiceProvider NullServiceProvider { get; }

        UniTask<Scene> LoadSceneAsync(
            object name,
            LoadSceneMode loadSceneMode,
            int categoryOrder,
            int subOrder,
            LifetimeScope lifetimeScope,
            bool loadFromScriptableObject = false,
            CancellationToken cancellationToken = default);

        UniTask<bool> UnloadSceneAsync(
            object name,
            int categoryOrder,
            int subOrder,
            bool loadFromScriptableObject = false,
            CancellationToken cancellationToken = default);

        UniTask<Scene> ReplaceSceneAsync(
            object currentName,
            int currentCategoryOrder,
            int currentSubOrder,
            object nextName,
            int nextCategoryOrder,
            int nextSubOrder,
            LifetimeScope lifetimeScope);

        void SetupTopmostLifetimeScope(LifetimeScope lifetimeScope);

        IEnumerator LoadScene(
            string name,
            LoadSceneMode loadSceneMode,
            int categoryOrder,
            int subOrder,
            LifetimeScope lifetimeScope,
            bool loadFromScriptableObject = false);

        IEnumerator UnloadScene(
            string name,
            int categoryOrder,
            int subOrder,
            bool loadFromScriptableObject = false);
    }

    public interface IServiceProvider :
        TPFive.Game.IServiceProvider
    {
        UniTask<Scene> LoadSceneAsync(
            LoadContext loadContext,
            bool loadFromScriptableObject = false,
            CancellationToken cancellationToken = default);

        UniTask<bool> UnloadSceneAsync(
            UnloadContext unloadContext,
            bool loadFromScriptableObject = false,
            CancellationToken cancellationToken = default);

        void SetupTopmostLifetimeScope(LifetimeScope lifetimeScope);
    }
}
