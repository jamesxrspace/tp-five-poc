using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TPFive.Game.Resource;
using UnityEngine;
using GameLoggingUtility = TPFive.Game.Logging.Utility;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using IResourceService = TPFive.Game.Resource.IService;

namespace TPFive.Extended.Decoration
{
    /// <summary>
    /// Manage current decoration objects in scene that can easy release or lookup.
    /// </summary>
    public class SceneAssetManager
    {
        private readonly IResourceService _resourceService;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;

        private Dictionary<string/*bundle id*/, GameObjectReference> _referenceMap = new ();
        private Dictionary<string/*bundle id*/, IAssetProvider<GameObject>> _providerMap = new ();

        public SceneAssetManager(ILoggerFactory loggerFactory, IResourceService resourceService, string sceneId)
        {
            _loggerFactory = loggerFactory;
            _logger = GameLoggingUtility.CreateLogger<SceneAssetManager>(loggerFactory);
            _resourceService = resourceService;
            SceneId = sceneId;
        }

        public string SceneId { get; private set; }

        public async UniTask LoadAsset(string bundleId, CancellationToken token)
        {
            if (!_providerMap.TryGetValue(bundleId, out var provider))
            {
                provider = new GameObjectProvider(_loggerFactory, _resourceService, bundleId);
                _providerMap.Add(bundleId, provider);
            }

            await provider.LoadAsset(token);
        }

        public async UniTask UnloadAsset(string bundleId, CancellationToken token = default)
        {
            if (_providerMap.TryGetValue(bundleId, out var provider))
            {
                await provider.UnloadAsset(token);
            }
        }

        public async UniTask<GameObject> CreateInstance(string bundleId, CancellationToken token = default)
        {
            _logger.LogDebug($"{nameof(CreateInstance)} Start : {bundleId}");

            if (!_providerMap.TryGetValue(bundleId, out var provider))
            {
                provider = new GameObjectProvider(_loggerFactory, _resourceService, bundleId);
                _providerMap.Add(bundleId, provider);
            }

            var go = await provider.CreateInstance(token);

            if (!_referenceMap.TryGetValue(bundleId, out var reference))
            {
                reference = new GameObjectReference(bundleId);
                _referenceMap.Add(bundleId, reference);
            }

            reference.Add(go);

            _logger.LogDebug($"{nameof(CreateInstance)} Success : Count={reference.Count}, bundleId={bundleId}");

            return go;
        }

        public async UniTask<bool> DestroyInstance(string bundleId, GameObject go, CancellationToken token = default)
        {
            if (!_providerMap.TryGetValue(bundleId, out var provider))
            {
                throw new InvalidOperationException($"Cannot find GameObjectProvider. bundleId: {bundleId}");
            }

            await provider.ReleaseInstance(go, token);

            if (!_referenceMap.TryGetValue(bundleId, out var reference))
            {
                throw new InvalidOperationException($"Cannot find GameObjectReference. bundleId: {bundleId}");
            }

            reference.Remove(go);

            // When reference is empty, remove this reference and provider from this map.
            if (reference.Count <= 0)
            {
                _referenceMap.Remove(bundleId);

                // Release the bundle asset.
                _logger.LogDebug($"Instance is zero start unload asset : {bundleId}");
                await provider.UnloadAsset(token);
                _providerMap.Remove(bundleId);
            }

            return true;
        }

        public async UniTask ReleaseAll(CancellationToken token = default)
        {
            _logger.LogDebug($"ReleaseAll Start : SceneId={SceneId}");

            var keys = _referenceMap.Keys.ToArray();
            foreach (var bundleId in keys)
            {
                var gameObjects = _referenceMap[bundleId].EntityList.ToArray();

                _logger.LogDebug($"Current instances length : {gameObjects.Length}");

                foreach (var go in gameObjects)
                {
                    // When destroy instance to zero. It will auto release the asset.
                    await DestroyInstance(bundleId, go, token);
                }
            }

            _referenceMap.Clear();
            _providerMap.Clear();

            _logger.LogDebug($"ReleaseAll End : SceneId={SceneId}");
        }

        public bool IsInstanceEmpty(string bundleId)
        {
            if (!_referenceMap.TryGetValue(bundleId, out var reference))
            {
                return true;
            }

            return reference is { Count: 0 };
        }

        public List<GameObject> GetGameObjects()
        {
            return _referenceMap.Values.SelectMany(x => x.EntityList).ToList();
        }
    }
}