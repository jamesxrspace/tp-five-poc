using System.Threading;
using Cysharp.Threading.Tasks;
using TPFive.Game.Resource;
using UnityEngine;
using VContainer;
using IDecorationService = TPFive.Game.Decoration.IService;

namespace TPFive.Game.Decoration
{
    public sealed class DecorationObjectLoader : AbstractAssetLoader
    {
        private const string GroupId = "Default";
        private IDecorationService service;

        [Inject]
        public void Construct(
            IDecorationService service)
        {
            this.service = service;
        }

        public UniTask<GameObject> CreateInstance()
        {
            return service.InstantiateAsync(GroupId, BundleID, CancellationToken.None);
        }

        protected override UniTask Load(string bundleId, CancellationToken token)
        {
            return service.LoadAsset(GroupId, BundleID, token);
        }

        protected override UniTask Unload(string bundleId, CancellationToken token)
        {
            return service.UnloadAsset(GroupId, BundleID, token);
        }

        private void OnDestroy()
        {
            service = null;
        }
    }
}
