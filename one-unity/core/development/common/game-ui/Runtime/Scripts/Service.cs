using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Loxodon.Framework.Views;
using Microsoft.Extensions.Logging;
using TPFive.Extended.LoxodonFramework;
using TPFive.Game.Extensions;
using TPFive.SCG.ServiceEco.Abstractions;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using VContainer;
using GameLoggingUtility = TPFive.Game.Logging.Utility;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Game.UI
{
    [ServiceProviderManagement]
    [RegisterToContainer]
    public sealed partial class Service : IService
    {
        private readonly IUIViewFactory _viewFactory;
        private readonly Dictionary<UIRootType, UIRoot> _roots;
        private readonly WindowAssetLocator _windowAssetLocator;
        private readonly UniTaskCompletionSource<bool> _utcs = new UniTaskCompletionSource<bool>();

        [Inject]
        public Service(
            ILoggerFactory loggerFactory,
            IUIViewLocator viewLocator,
            IObjectResolver container,
            WindowAssetLocator windowAssetLocator)
        {
            Logger = GameLoggingUtility.CreateLogger<Service>(loggerFactory);
            _viewFactory = new UIViewFactory(viewLocator, container);
            _funcOperationCanceledException = HandleStartAsyncOperationCanceledException;
            _funcException = HandleStartAsyncException;
            _roots = new Dictionary<UIRootType, UIRoot>();
            _windowAssetLocator = windowAssetLocator;
            AddRoots(UnityEngine.Object.FindObjectsOfType<UIRoot>());
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private ILogger Logger { get; }

        public UniTask<WindowBase> ShowWindow(Type type)
        {
            Assert.IsNotNull(type);

            var prefabPath = _windowAssetLocator.GetLocation(type);
            if (string.IsNullOrEmpty(prefabPath))
            {
                Logger.LogError("Can't find {PrefabPath} by {Type}", nameof(prefabPath), type);
                return UniTask.FromResult<WindowBase>(default);
            }

            return ShowWindow(prefabPath, null);
       }

        public UniTask<WindowBase> ShowWindow(Type type, IBundle bundle)
        {
            Assert.IsNotNull(type);
            Assert.IsNotNull(bundle);

            var prefabPath = _windowAssetLocator.GetLocation(type);
            return ShowWindow(prefabPath, bundle);
        }

        public async UniTask<WindowBase> ShowWindow(string prefabPath)
        {
            Assert.IsFalse(string.IsNullOrEmpty(prefabPath));
            return await ShowWindow(prefabPath, null);
        }

        public async UniTask<WindowBase> ShowWindow(string prefabPath, IBundle bundle)
        {
            Assert.IsFalse(string.IsNullOrEmpty(prefabPath));

            var window = await _viewFactory.GetWindowAsync<WindowBase>(prefabPath);

            if (window.RootType == null)
            {
                Logger.LogError($"{nameof(WindowBase.RootType)} is null. {prefabPath}");
                UnityEngine.Object.Destroy(window.gameObject);
                return default;
            }

            if (!_roots.TryGetValue(window.RootType, out var root))
            {
                Logger.LogError($"Can not find \"{window.RootType.RootName}\". {prefabPath}");
                UnityEngine.Object.Destroy(window.gameObject);
                return default;
            }

            window.WindowManager = root.WindowManager;
            window.Create(bundle);
            await root.WindowManager.Show(window);
            return window;
        }

        public async UniTask<T> ShowWindow<T>()
            where T : WindowBase
        {
            var prefabPath = _windowAssetLocator.GetLocation(typeof(T));
            return (T)await ShowWindow(prefabPath, null);
        }

        public async UniTask<T> ShowWindow<T>(IBundle bundle)
            where T : WindowBase
        {
            Assert.IsNotNull(bundle);

            var prefabPath = _windowAssetLocator.GetLocation(typeof(T));
            if (string.IsNullOrEmpty(prefabPath))
            {
                Logger.LogError("Can't find {PrefabPath} by {Type}", nameof(prefabPath), typeof(T));
                return await UniTask.FromResult<T>(default);
            }

            return (T)await ShowWindow(prefabPath, bundle);
        }

        public async UniTask<T> ShowWindow<T>(string prefabPath)
            where T : WindowBase
        {
            Assert.IsFalse(string.IsNullOrEmpty(prefabPath));
            return (T)await ShowWindow(prefabPath, null);
        }

        public async UniTask<T> ShowWindow<T>(string prefabPath, IBundle bundle)
            where T : WindowBase
        {
            Assert.IsFalse(string.IsNullOrEmpty(prefabPath));
            Assert.IsNotNull(bundle);
            return (T)await ShowWindow(prefabPath, bundle);
        }

        public T FindWindow<T>()
            where T : WindowBase
        {
            foreach (var root in _roots.Values)
            {
                if (root == null || root.WindowManager == null)
                {
                    continue;
                }

                var window = root.WindowManager.Find<T>();
                if (window != null)
                {
                    return window;
                }
            }

            return null;
        }

        public T[] FindWindows<T>()
            where T : WindowBase
        {
            return _roots.Values.Where(x => x != null && x.WindowManager != null)
                .SelectMany(x => x.WindowManager.FindAll<T>())
                .ToArray();
        }

        public T FindWindowInRoot<T>(UIRootType type)
            where T : WindowBase
        {
            if (type == null)
            {
                return default;
            }

            if (!_roots.TryGetValue(type, out var root))
            {
                return default;
            }

            return root.WindowManager.Find<T>();
        }

        public T[] FindWindowsInRoot<T>(UIRootType type)
            where T : WindowBase
        {
            if (type == null)
            {
                return Array.Empty<T>();
            }

            if (!_roots.TryGetValue(type, out var root))
            {
                return Array.Empty<T>();
            }

            return root.WindowManager.FindAll<T>().ToArray();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            AddRoots(scene.GetComponentsInScene<UIRoot>());
        }

        private void AddRoots(UIRoot[] roots)
        {
            foreach (var root in roots)
            {
                _roots.TryAdd(root.Type, root);
            }
        }

        private void OnSceneUnloaded(Scene scene)
        {
            // After scene unloaded, all roots in the scene will be destroyed.
            _roots.Where(kvp => kvp.Value == null)
                .Select(kvp => kvp.Key)
                .ToList()
                .ForEach(key => _roots.Remove(key));
        }
    }
}
