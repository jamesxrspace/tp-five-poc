using System.Threading;
using Microsoft.Extensions.Logging;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TPFive.Extended.ResourceLoader
{
    using System;
    using Cysharp.Threading.Tasks;
    using TPFive.Game.Resource;
    using UnityEngine.Assertions;

    /// <summary>
    /// ServiceProvider of ResourceLoader.
    /// </summary>
    public sealed partial class ServiceProvider :
        TPFive.Game.Resource.IServiceProvider
    {
        private TextureManager _textureManager;

        public ServiceProvider(
            ILoggerFactory loggerFactory,
            TextureManager textureManager)
        {
            Assert.IsNotNull(loggerFactory);

            Logger = Game.Logging.Utility.CreateLogger<ServiceProvider>(loggerFactory);

            _textureManager = textureManager;
        }

        public Microsoft.Extensions.Logging.ILogger Logger { get; set; }

        public async UniTask LoadBundledDataAsync(
            string name,
            CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public async UniTask UnloadBundleDataAsync(
            string name,
            CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public async UniTask<T> LoadAssetAsync<T>(
            string name,
            CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public async UniTask<bool> UnloadAssetAsync<T>(
            string name,
            CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public async UniTask<Scene> LoadSceneAsync(string name, LoadSceneMode loadSceneMode, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public async UniTask<bool> UnloadSceneAsync(string name, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public UniTask<T> LoadAssetAsync<T>(object name, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public UniTask<bool> UnloadAssetAsync<T>(object name, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public UniTask<Scene> LoadSceneAsync(object name, LoadSceneMode loadSceneMode, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public UniTask<bool> UnloadSceneAsync(object name, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Scene GetLoadedScene(string name)
        {
            throw new System.NotImplementedException();
        }

        public GameObject GetLoadedGameObject(string name)
        {
            throw new System.NotImplementedException();
        }

        public ScriptableObject GetLoadedScriptableObject(string name)
        {
            throw new System.NotImplementedException();
        }

        public async UniTask<TextureData> LoadTexture(object owner, TextureRequestContext context, CancellationToken token)
        {
            var promise = new UniTaskCompletionSource<TextureData>();
            var textureRequest = new TextureRequest(owner, context, data =>
            {
                promise.TrySetResult(data);
            });

            token.Register(() =>
            {
                promise.TrySetCanceled(token);
            });

            _textureManager.Load(textureRequest);

            try
            {
                return await promise.Task;
            }
            catch (OperationCanceledException)
            {
                _textureManager.Abort(textureRequest);
            }

            return null;
        }

        public void Release(string resourceUrl, object owner)
        {
            _textureManager.Release(resourceUrl, owner);
        }
    }
}
