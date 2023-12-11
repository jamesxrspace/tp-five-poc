using System;
using System.Threading;
using Animancer;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TPFive.Game.Avatar.Sit;
using TPFive.Game.Avatar.Talk;
using TPFive.Game.Avatar.Tracking;
using TPFive.Game.Extensions;
using UnityEngine;
using UnityEngine.Assertions;
using VContainer;
using VContainer.Unity;
using XR.Avatar;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using IMocapService = TPFive.Game.Mocap.IService;

namespace TPFive.Game.Avatar.Factory
{
    public class DefaultAvatarFactory : IAvatarFactory
    {
        private readonly AvatarFactorySettings settings;
        private readonly IObjectResolver objectResolver;
        private readonly ILoggerFactory loggerFactory;
        private readonly ILogger logger;
        private readonly IBinfileDownloader binfileDownloader;
        private readonly IMocapService mocapService;

        public DefaultAvatarFactory(
            ILoggerFactory loggerFactory,
            AvatarFactorySettings settings,
            IObjectResolver objectResolver,
            IBinfileDownloader binfileDownloader,
            IMocapService mocapService)
        {
            this.logger = loggerFactory.CreateLogger<DefaultAvatarFactory>();
            this.settings = settings;
            this.objectResolver = objectResolver;
            this.loggerFactory = loggerFactory;
            this.binfileDownloader = binfileDownloader;
            this.mocapService = mocapService;
        }

        public virtual async UniTask<(bool, GameObject)> Create(
            AvatarFormat avatarFormat,
            CancellationToken token)
        {
            return await Create(avatarFormat, Options.All, token);
        }

        public virtual async UniTask<(bool, GameObject)> Create(
            AvatarFormat avatarFormat,
            OptionBase optionBase,
            CancellationToken token)
        {
            Assert.IsNotNull(avatarFormat, $"{nameof(avatarFormat)} is null.");

            var categoryName = GetCategoryName(avatarFormat.gender);
            return await Create(categoryName, avatarFormat, optionBase, token);
        }

        public virtual async UniTask<(bool, GameObject)> Create(
            string presetName,
            AvatarFormat avatarFormat,
            CancellationToken token)
        {
            return await Create(presetName, avatarFormat, Options.All, token);
        }

        public virtual async UniTask<(bool, GameObject)> Create(
            string presetName,
            AvatarFormat avatarFormat,
            OptionBase optionBase,
            CancellationToken token)
        {
            Assert.IsFalse(string.IsNullOrEmpty(presetName), $"{nameof(presetName)} is null or empty.");
            Assert.IsNotNull(avatarFormat, $"{nameof(avatarFormat)} is null.");
            if (optionBase is not Options option)
            {
                throw new ArgumentException($"Invalid {nameof(option)}");
            }

            var preset = GetPreset(presetName);
            var go = Instantiate(preset.Prefab);
            try
            {
                var ok = await SetupByPreset(preset, go, avatarFormat, option, token);
                return (ok, go);
            }
            catch (OperationCanceledException)
            {
                UnityEngine.Object.Destroy(go);
                throw;
            }
        }

        public virtual async UniTask<bool> Setup(
            GameObject go,
            AvatarFormat avatarFormat,
            CancellationToken token)
        {
            return await Setup(go, avatarFormat, Options.All, token);
        }

        public virtual async UniTask<bool> Setup(
            GameObject go,
            AvatarFormat avatarFormat,
            OptionBase optionBase,
            CancellationToken token)
        {
            Assert.IsNotNull(avatarFormat, $"{nameof(avatarFormat)} is null.");
            if (optionBase is not Options option)
            {
                throw new ArgumentException($"Invalid {nameof(option)}");
            }

            var categoryName = GetCategoryName(avatarFormat.gender);
            Assert.IsFalse(string.IsNullOrEmpty(categoryName), $"{nameof(categoryName)} is null or empty.");

            var preset = GetPreset(categoryName);
            Assert.IsNotNull(preset, $"{nameof(preset)} is null.");
            Assert.IsNotNull(preset.Prefab, $"{nameof(preset.Prefab)} is null.");

            return await SetupByPreset(preset, go, avatarFormat, option, token);
        }

        public virtual async UniTask<bool> Setup(
            string presetName,
            GameObject go,
            AvatarFormat avatarFormat,
            CancellationToken token)
        {
            return await Setup(presetName, go, avatarFormat, Options.All, token);
        }

        public virtual async UniTask<bool> Setup(
            string presetName,
            GameObject go,
            AvatarFormat avatarFormat,
            OptionBase optionBase,
            CancellationToken token)
        {
            Assert.IsFalse(string.IsNullOrEmpty(presetName), $"{nameof(presetName)} is null or empty.");
            Assert.IsNotNull(avatarFormat, $"{nameof(avatarFormat)} is null.");
            if (optionBase is not Options option)
            {
                throw new ArgumentException($"Invalid {nameof(option)}");
            }

            var preset = GetPreset(presetName);
            Assert.IsNotNull(preset, $"{nameof(preset)} is null.");
            Assert.IsNotNull(preset.Prefab, $"{nameof(preset.Prefab)} is null.");

            return await SetupByPreset(preset, go, avatarFormat, option, token);
        }

        protected async UniTask<bool> SetupByPreset(
            AvatarFactoryPreset preset,
            GameObject go,
            AvatarFormat avatarFormat,
            Options options,
            CancellationToken cancellationToken)
        {
            string binfilePath = null;
            var shouldDownloadBinfile = options.EnableToLoadBinfile ?? preset.EnableToLoadBinfile;
            if (shouldDownloadBinfile)
            {
                var (binfile, error) = await binfileDownloader.DownloadAsync(
                    options.BinfileUrl,
                    settings.MaxDownloadRetries,
                    settings.ConnectTimeout,
                    settings.RequetTimeout,
                    cancellationToken);

                if (error != null)
                {
                    logger.LogWarning(
                        "Failed download binfile from {BinfileUrl}. error: {ErrorMessage}",
                        options.BinfileUrl,
                        error);
                }
                else
                {
                    binfilePath = binfile;
                }
            }

            var skinsManager = go.GetOrAddComponent<SkinsManager>();
            var shouldLoadStylizedAvatar = true;
            if (!string.IsNullOrEmpty(binfilePath))
            {
                var ok = await LoadAvatarFromCache(skinsManager, binfilePath, options.SkipBinfileLOD0, cancellationToken);
                if (ok)
                {
                    shouldLoadStylizedAvatar = false;
                }
            }

            if (shouldLoadStylizedAvatar)
            {
                var loadOptions = new LoadAvatarOptions
                {
                    avatarFormat = avatarFormat,
                    generateLod = options.EnableToGenerateLod ?? preset.EnableToGenerateLod,
                    combineBody = options.EnableToCombineMesh ?? preset.EnableToCombineMesh,
                    hideAvatarAfterLoad = true,
                };

                var ok = await LoadStylizedAvatar(skinsManager, loadOptions, cancellationToken);
                if (!ok)
                {
                    _ = go.GetOrAddComponent<AvatarContextProvider>();
                    return false;
                }
            }

            var provider = go.GetOrAddComponent<AvatarContextProvider>();
            provider.Controller = go.GetComponent<AvatarController>();
            provider.AvatarFormat = avatarFormat;
            provider.SkinManager = skinsManager;
            InitAnimationComponents(ref provider, go, preset, skinsManager, options);

            // play avatar base animation controller
            if (preset.BaseController != null)
            {
                _ = provider.Animancer.Play(preset.BaseController);

                // wait for one frame to make sure the animation is playing
                await UniTask.DelayFrame(1, cancellationToken: cancellationToken);
            }

            skinsManager.SetVisible(true);
            return true;
        }

        protected async UniTask<bool> LoadStylizedAvatar(SkinsManager skinManager, LoadAvatarOptions options, CancellationToken cancellationToken)
        {
            await skinManager.LoadStylizedAvatar(options);
            cancellationToken.ThrowIfCancellationRequested();

            var result = skinManager.loadAvatarResult;
            if (result.error != null)
            {
                logger.LogError(
                    "Failed to load avatar. error: {ErrorMessage}",
                    result.error.Error());
            }

            return result.error == null;
        }

        protected async UniTask<bool> LoadAvatarFromCache(SkinsManager skinsManager, string binfilePath, bool skipLOD0, CancellationToken cancellationToken)
        {
            await skinsManager.LoadFromCache(binfilePath, skipLOD0, true);
            cancellationToken.ThrowIfCancellationRequested();

            var result = skinsManager.loadAvatarResult;
            if (result.error != null)
            {
                if (result.NeedToUploadCache)
                {
                    logger.LogWarning("Found incompatible binfile. Please go to edit your avatar again.");
                }
                else
                {
                    logger.LogError(
                        "Failed to load avatar. Error: {ErrorMessage}",
                        result.error.Error());
                }
            }

            return result.error == null;
        }

        protected void InitAnimationComponents(
            ref AvatarContextProvider provider,
            GameObject go,
            AvatarFactoryPreset preset,
            SkinsManager skinsManager,
            Options options)
        {
            var animator = go.GetOrAddComponent<Animator>();
            var animancer = go.GetOrAddComponent<AnimancerComponent>();

            // when the avatar is disabled, the animancer will be paused.
            animancer.ActionOnDisable = AnimancerComponent.DisableAction.Pause;

            provider.Animator = animator;
            provider.Animancer = animancer;

            var anchorPointProvider = AvatarAnchorPointCreator.Create(
                go,
                preset.AnchorPointCategory.Definitions,
                animator);
            provider.AnchorPointProvider = anchorPointProvider;

            if (options.Features.HasFlag(Options.FeatureFlags.Motion))
            {
                provider.MotionManager = AvatarMotionCreator.Create(
                    go,
                    animancer,
                    anchorPointProvider,
                    preset.MotionCategory);

                var sitLayer = animancer.Layers[AvatarUtility.GetLayerIndex(AvatarAnimancerLayer.Sit)];
                provider.SitManager = new AvatarSitManager(loggerFactory, go.transform, sitLayer, preset.SitSettings);

                var talkLayer = animancer.Layers[AvatarUtility.GetLayerIndex(AvatarAnimancerLayer.Talk)];
                provider.TalkManager = new AvatarTalkManager(loggerFactory, talkLayer, preset.TalkSettings);

                var layerFadeDuration = preset.TrackingSettings.LayerFadeDuration;
                var bodyTrackingMotion = new BodyTrackingMotion(
                    animancer.Layers[AvatarUtility.GetLayerIndex(AvatarAnimancerLayer.BodyTracking)],
                    () => mocapService.CapturedHumanPose,
                    animator,
                    go.GetOrAddComponent<AnimationIKExecutor>(),
                    layerFadeDuration);
                var faceTrackingMotion = new FaceTrackingMotion(
                    animancer.Layers[AvatarUtility.GetLayerIndex(AvatarAnimancerLayer.FaceTracking)],
                    skinsManager.GetSkinByPart(ESkinPart.S0),
                    mocapService.FaceBlendShapeProvider,
                    layerFadeDuration);

                provider.TrackingManager = new AvatarTrackingManager(
                    bodyTrackingMotion,
                    faceTrackingMotion,
                    preset.TrackingSettings,
                    mocapService,
                    loggerFactory);

                var humanPoseHandler = new HumanPoseHandler(animator.avatar, animator.transform);
                provider.HumanPoseSynchronizer = new HumanPoseSynchronizer(humanPoseHandler);
            }
        }

        protected string GetCategoryName(int gender)
        {
            var categoryName = settings.GetCategoryName(gender);
            Assert.IsFalse(string.IsNullOrEmpty(categoryName), $"{nameof(categoryName)} is null or empty.");
            return categoryName;
        }

        protected AvatarFactoryPreset GetPreset(string presetName)
        {
            var preset = settings.GetPreset(presetName);
            Assert.IsNotNull(preset, $"{nameof(preset)} is null.");
            Assert.IsNotNull(preset.Prefab, $"{nameof(preset.Prefab)} is null.");
            return preset;
        }

        protected GameObject Instantiate(GameObject prefab, string name = null)
        {
            var go = objectResolver.Instantiate(prefab);
            if (!string.IsNullOrEmpty(name))
            {
                go.name = name;
            }

            return go;
        }
    }
}
