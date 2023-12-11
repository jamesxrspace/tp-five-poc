using System.Threading;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Game.Resource
{
    public sealed class ThumbnailLoader : AbstractAssetLoader
    {
        [SerializeField]
        private RawImage targetImage;
        private Texture originalTexture;
        private IService resourceService;
        private ILogger logger;

        [Inject]
        public void Construct(ILoggerFactory loggerFactory, IService resourceService)
        {
            logger = loggerFactory?.CreateLogger<ThumbnailLoader>();
            this.resourceService = resourceService;
        }

        protected override async UniTask Load(string bundleID, CancellationToken token)
        {
            if (string.IsNullOrEmpty(bundleID))
            {
                return;
            }

            var scriptableObject = await LoadScriptableObjectAssetAsync(bundleID, token);
            if (scriptableObject is not Creator.BundleDetailData bdd ||
                bdd.bundleKind != Creator.BundleKind.SceneObject)
            {
                logger.LogError("Failed to load asset. bundleId: {bundleId}", bundleID);
                return;
            }

            targetImage.texture = bdd.thumbnail;
        }

        protected override async UniTask Unload(string bundleId, CancellationToken token)
        {
            targetImage.texture = originalTexture;

            await UniTask.CompletedTask;
        }

        private async UniTask<ScriptableObject> LoadScriptableObjectAssetAsync(string bundleID, CancellationToken token)
        {
            await resourceService.LoadBundledDataAsync(bundleID, token);
            var scriptableObject = await resourceService.LoadAssetAsync<ScriptableObject>($"{bundleID}.asset", token);

            return scriptableObject;
        }

        private void Awake()
        {
            originalTexture = targetImage.texture;
        }

        private void OnDestroy()
        {
            if (targetImage.texture != null &&
                targetImage.texture != originalTexture)
            {
                targetImage.texture = null;
            }

            originalTexture = null;
        }
    }
}
