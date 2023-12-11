using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TPFive.Creator;
using TPFive.Game.Messages;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace TPFive.Extended.SceneActivity
{
    using TPFive.Game.Logging;

    using GameSceneFlow = TPFive.Game.SceneFlow;

    public partial class ServiceProvider :
        GameSceneFlow.IServiceProvider
    {
        /// <summary>
        /// Used as the default lifetime scope if none can be found.
        /// </summary>
        private readonly Stack<GameSceneFlow.SceneContext> _sceneContextStack = new ();

        private LifetimeScope _topMostLifetimeScope;
        private Queue<TaskCompletionSource<(Scene, bool)>> _sceneTaskSignalQueue = new ();

        public void SetupTopmostLifetimeScope(LifetimeScope lifetimeScope)
        {
            _topMostLifetimeScope = lifetimeScope;
        }

        public async UniTask<Scene> LoadSceneAsync(
            GameSceneFlow.LoadContext context,
            bool loadFromScriptableObject = false,
            CancellationToken cancellationToken = default)
        {
            var scene = default(Scene);
            try
            {
                var key = loadFromScriptableObject ?
                    await ReadBundleIdFromScriptObject(context.Title.ToString()) : context.Title;
                var result = await LoadUnloadSceneAsync(
                    key,
                    context.Category,
                    context.CategoryOrder,
                    context.SubOrder,
                    true);

                scene = result.Item1;
            }
            catch (Exception e)
            {
                Logger.LogError("{Exception}", e);
            }

            return scene;
        }

        public async UniTask<bool> UnloadSceneAsync(
            GameSceneFlow.UnloadContext context,
            bool loadFromScriptableObject = false,
            CancellationToken cancellationToken = default)
        {
            var r = false;
            try
            {
                var key = loadFromScriptableObject ?
                    await ReadBundleIdFromScriptObject(context.Title.ToString()) : context.Title;
                var result = await LoadUnloadSceneAsync(
                    key,
                    context.Category,
                    context.CategoryOrder,
                    context.SubOrder,
                    false);

                r = result.Item2;
            }
            catch (Exception e)
            {
                Logger.LogError("{Exception}", e);
            }

            return r;
        }

        private bool ChangeActiveStateForGameObjectInScene<T>(Scene scene, bool active)
            where T : Component
        {
            var rootGOs = scene.GetRootGameObjects();
            var objs = rootGOs.Where(x => x.GetComponent<T>() != null).ToList();
            objs.ForEach(x => x.SetActive(active));
            return objs.Count > 0;
        }

        private void DeactivateEventSystemAndAudioListener()
        {
            _sceneContextStack
                .Select(x => x.Scene)
                .Append(SceneManager.GetActiveScene())
                .ToList()
                .ForEach(x =>
                {
                    ChangeActiveStateForGameObjectInScene<EventSystem>(x, false);
                    ChangeActiveStateForGameObjectInScene<AudioListener>(x, false);
                });
        }

        private void ActivateEventSystemAndAudioListener()
        {
            var sceneInOrder = _sceneContextStack
                .Select(x => x.Scene)
                .Append(SceneManager.GetActiveScene())
                .ToList();

            foreach (var scene in sceneInOrder)
            {
                if (ChangeActiveStateForGameObjectInScene<EventSystem>(scene, true))
                {
                    break;
                }
            }

            foreach (var scene in sceneInOrder)
            {
                if (ChangeActiveStateForGameObjectInScene<AudioListener>(scene, true))
                {
                    break;
                }
            }
        }

        private async UniTask<(Scene, LifetimeScope)> HandleSceneLoading(
            object title,
            int subOrder,
            LifetimeScope parentLifetimeScope,
            CancellationToken cancellationToken = default)
        {
            using LifetimeScope.ParentOverrideScope? parentOverrideScope = subOrder == 0 ? LifetimeScope.EnqueueParent(parentLifetimeScope) : null;
            var scene = await _resourceService.LoadSceneAsync(
                title,
                LoadSceneMode.Additive,
                cancellationToken);

            if (!scene.IsValid())
            {
                await Task.FromException(new System.Exception($"Loaded scene {title} is not valid"));
                return (scene, null);
            }

            var lifetimeScope = subOrder == 0 ? LifetimeScope.Find<LifetimeScope>(scene) : parentLifetimeScope;
            if (subOrder == 0 && lifetimeScope == null)
            {
                await Task.FromException(new System.Exception($"Loaded scene {title} has no lifetime scope"));
                return (scene, null);
            }

            return (scene, lifetimeScope);
        }

        private void PublishStartedMessage(
            string title,
            string category,
            int mainOrder,
            int subOrder,
            bool isLoad)
        {
            if (isLoad)
            {
                var message = new SceneLoading
                {
                    Title = title,
                    Category = category,
                    CategoryOrder = mainOrder,
                    SubOrder = subOrder,
                };
                _pubSceneLoading.Publish(message);
            }
            else
            {
                var message = new SceneUnloading
                {
                    Title = title,
                    Category = category,
                    CategoryOrder = mainOrder,
                    SubOrder = subOrder,
                };
                _pubSceneUnloading.Publish(message);
            }
        }

        private void PublishFinishedMessage(
            string title,
            string category,
            int mainOrder,
            int subOrder,
            bool isLoad)
        {
            if (isLoad)
            {
                var message = new SceneLoaded
                {
                    Title = title,
                    Category = category,
                    CategoryOrder = mainOrder,
                    SubOrder = subOrder,
                };
                _pubSceneLoaded.Publish(message);
            }
            else
            {
                var message = new SceneUnloaded
                {
                    Title = title,
                    Category = category,
                    CategoryOrder = mainOrder,
                    SubOrder = subOrder,
                };
                _pubSceneUnloaded.Publish(message);
            }
        }

        private async Task<(Scene, bool)> LoadUnloadSceneAsync(
            object title,
            string category,
            int mainOrder,
            int subOrder,
            bool isLoad,
            CancellationToken cancellationToken = default)
        {
            var sceneTaskSignal = new TaskCompletionSource<(Scene, bool)>();
            _sceneTaskSignalQueue.Enqueue(sceneTaskSignal);
            while (_sceneTaskSignalQueue.Peek() != sceneTaskSignal)
            {
                await Task.Yield();
            }

            try
            {
                DeactivateEventSystemAndAudioListener();

                cancellationToken.ThrowIfCancellationRequested();

                var targetKey = new GameSceneFlow.Key(mainOrder, subOrder);
                var anySceneWithSameOrder = _sceneContextStack
                    .Where(item => item.CompareTo(targetKey) == 0)
                    .Any();

                var listSceneStackOrderMsg = _sceneContextStack.Select(x => x.Key).OrderBy(x => x.CategoryOrder);
                var sceneOrderMsg = string.Join('\n', listSceneStackOrderMsg);
                Logger.LogEditorDebug($"Scene Order : \n {sceneOrderMsg}");

                // An identical order cannot be found in the loading case, and
                // a matched order should be found in the unloading case.
                if (anySceneWithSameOrder == isLoad)
                {
                    var errorMsg = isLoad ?
                        $"Order {mainOrder}:{subOrder} is already loaded. Abort loading scene." :
                        $"Order {mainOrder}:{subOrder} is not found. Abort unloading scene.";

                    Logger.LogEditorDebug(errorMsg);
                    sceneTaskSignal.SetResult((default, false));
                    return await sceneTaskSignal.Task;
                }

                PublishStartedMessage(title.ToString(), category, mainOrder, subOrder, isLoad);

                var startTime = UnityEngine.Time.realtimeSinceStartup;

                var loadingOrUnloading = isLoad ? "loading" : "unloading";
                SendTimeUsage("Beginning", title, loadingOrUnloading, startTime);

                // Pop and unload scene until a matched order is found.
                var restoringScenes = new Stack<GameSceneFlow.SceneContext>();
                while (_sceneContextStack.TryPeek(out var sceneContext))
                {
                    if (sceneContext.CompareTo(targetKey) < 0)
                    {
                        break;
                    }

                    var currentContext = _sceneContextStack.Pop();
                    var isUnderneath = sceneContext.CompareTo(targetKey) > 0;
                    var isSubScene = subOrder != 0;
                    var isDefferentEntry = mainOrder != sceneContext.Key.CategoryOrder;
                    var shouldRestore = isUnderneath && (isSubScene || isDefferentEntry);
                    if (shouldRestore)
                    {
                        restoringScenes.Push(currentContext);
                    }

                    await _resourceService.UnloadSceneAsync(sceneContext.Title);
                }

                // Restore scenes here. If isLoad == true, we push the scenes to be loaded into the stack
                // and restore the scenes underneath. If isLoad == false, we only restore the scenes underneath.
                var parentLifetimeScope = _sceneContextStack.FirstOrDefault()?.CurrentLifetimeScope;
                parentLifetimeScope = parentLifetimeScope == null ? _topMostLifetimeScope : parentLifetimeScope;
                GameSceneFlow.SceneContext contextToBeAdded = default;
                if (isLoad)
                {
                    contextToBeAdded = new GameSceneFlow.SceneContext
                    {
                        Title = title,
                        Key = targetKey,
                        ParentLifetimeScope = parentLifetimeScope,
                    };
                    restoringScenes.Push(contextToBeAdded);
                }

                foreach (var restoreContext in restoringScenes)
                {
                    restoreContext.ParentLifetimeScope = parentLifetimeScope;
                    var (tempScene, tempLifetimeScope) = await HandleSceneLoading(
                        restoreContext.Title,
                        restoreContext.Key.SubOrder,
                        parentLifetimeScope,
                        cancellationToken);

                    restoreContext.CurrentLifetimeScope = tempLifetimeScope;
                    restoreContext.Scene = tempScene;
                    _sceneContextStack.Push(restoreContext);
                    parentLifetimeScope = restoreContext.CurrentLifetimeScope;
                }

                var endTime = UnityEngine.Time.realtimeSinceStartup;

                SendTimeUsage("Finishing", title, loadingOrUnloading, endTime);

                PublishFinishedMessage(title.ToString(), category, mainOrder, subOrder, isLoad);
                sceneTaskSignal.TrySetResult((contextToBeAdded?.Scene ?? default, true));
            }
            catch (System.OperationCanceledException)
            {
                sceneTaskSignal.TrySetCanceled(cancellationToken);
            }
            catch (System.Exception e)
            {
                sceneTaskSignal.TrySetException(e);
            }
            finally
            {
                _sceneTaskSignalQueue.Dequeue();
                ActivateEventSystemAndAudioListener();
            }

            return await sceneTaskSignal.Task;
        }
    }
}
