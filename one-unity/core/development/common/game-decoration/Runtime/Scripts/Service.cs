using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TPFive.Game.Logging;
using TPFive.OpenApi.GameServer.Model;
using TPFive.SCG.AsyncStartable.Abstractions;
using TPFive.SCG.DisposePattern.Abstractions;
using UnityEngine;
using VContainer;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Game.Decoration
{
    using TPFive.SCG.ServiceEco.Abstractions;
    using GameLoggingUtility = TPFive.Game.Logging.Utility;

    /// <summary>
    /// This implement of Game.Decoration.IService.
    /// </summary>
    [AsyncStartable(ExceptionList = "System.OperationCanceledException, System.Exception")]
    [Dispose]
    [ServiceProviderManagement]
    [ServiceProvidedBy(typeof(TPFive.Game.Decoration.IServiceProvider))]
    [RegisterToContainer]
    public partial class Service : IService
    {
        public const int DecorationLoaderIndex = (int)ServiceProviderKind.Rank1ServiceProvider;

        private readonly ILogger _logger;
        private UniTaskCompletionSource<bool> _utcs = new ();

        [Inject]
        public Service(
            ILoggerFactory loggerFactory)
        {
            _logger = GameLoggingUtility.CreateLogger<Service>(loggerFactory);

            var nullServiceProvider = new NullServiceProvider((s, args) =>
            {
                _logger.LogDebug(s, args);
            });

            _serviceProviderTable.Add((int)ServiceProviderKind.NullServiceProvider, nullServiceProvider);
        }

        public IServiceProvider NullServiceProvider => _serviceProviderTable[(int)ServiceProviderKind.NullServiceProvider] as IServiceProvider;

        public UniTask LoadAsset(string groupId, string bundleId, CancellationToken token = default)
        {
            var serviceProvider = GetServiceProvider(DecorationLoaderIndex);
            return serviceProvider.LoadAsset(groupId, bundleId, token);
        }

        public UniTask UnloadAsset(string groupId, string bundleId, CancellationToken token = default)
        {
            var serviceProvider = GetServiceProvider(DecorationLoaderIndex);
            return serviceProvider.UnloadAsset(groupId, bundleId, token);
        }

        public UniTask<GameObject> InstantiateAsync(string groupId, string bundleID, CancellationToken token = default)
        {
            var serviceProvider = GetServiceProvider(DecorationLoaderIndex);
            return serviceProvider.InstantiateAsync(groupId, bundleID, token);
        }

        public UniTask<bool> DestroyAsync(string groupId, string bundleId, GameObject go, CancellationToken token = default)
        {
            var serviceProvider = GetServiceProvider(DecorationLoaderIndex);
            return serviceProvider.DestroyAsync(groupId, bundleId, go, token);
        }

        public UniTask Release(string groupId, CancellationToken token = default)
        {
            var serviceProvider = GetServiceProvider(DecorationLoaderIndex);
            return serviceProvider.Release(groupId, token);
        }

        public UniTask ReleaseAll(CancellationToken token = default)
        {
            var serviceProvider = GetServiceProvider(DecorationLoaderIndex);
            return serviceProvider.ReleaseAll(token);
        }

        public UniTask<TPFive.Game.Resource.XRSceneObject> InstantiateAsync(string groupId, TPFive.Game.Resource.XRObject data, CancellationToken token = default)
        {
            var serviceProvider = GetServiceProvider(DecorationLoaderIndex);
            return serviceProvider.InstantiateAsync(groupId, data, token);
        }

        public UniTask<bool> DestroyAsync(string groupId, TPFive.Game.Resource.XRSceneObject sceneObject, CancellationToken token = default)
        {
            var serviceProvider = GetServiceProvider(DecorationLoaderIndex);
            return serviceProvider.DestroyAsync(groupId, sceneObject, token);
        }

        public IReadOnlyList<TPFive.Game.Resource.XRSceneObject> GetDecorations(string groupId)
        {
            var serviceProvider = GetServiceProvider(DecorationLoaderIndex);
            return serviceProvider.GetDecorations(groupId);
        }

        public UniTask<List<CategoryItem>> GetCategoryListAsync(
            int size,
            int? offset,
            CancellationToken token)
        {
            var serviceProvider = GetServiceProvider(DecorationLoaderIndex);
            return serviceProvider.GetCategoryList(size, offset, token);
        }

        public UniTask<List<OpenApi.GameServer.Model.Decoration>> GetDecorationListAsync(
            int size,
            int? offset = default,
            string categoryId = default,
            CancellationToken token = default)
        {
            var serviceProvider = GetServiceProvider(DecorationLoaderIndex);
            return serviceProvider.GetDecorationList(size, offset, categoryId, token);
        }

        private async UniTask SetupBegin(CancellationToken cancellationToken = default)
        {
            _logger.LogEditorDebug(
                "{Method}",
                nameof(SetupBegin));

            await UniTask.CompletedTask;
        }

        private async UniTask SetupEnd(
            bool success,
            CancellationToken cancellationToken = default)
        {
            _logger.LogEditorDebug(
                "{Method}",
                nameof(SetupEnd));

            _utcs.TrySetResult(success);

            await UniTask.CompletedTask;
        }

        private void HandleDispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _utcs.TrySetCanceled(_cancellationTokenSource.Token);
                _utcs = default;
                _disposed = true;
            }
        }
    }
}