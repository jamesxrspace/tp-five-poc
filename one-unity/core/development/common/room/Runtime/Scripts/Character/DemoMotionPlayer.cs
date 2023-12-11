using Fusion;
using Loxodon.Framework.Views;
using Microsoft.Extensions.Logging;
using TPFive.Game;
using TPFive.Game.Avatar;
using UnityEngine;
using VContainer;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using IUIService = TPFive.Game.UI.IService;

namespace TPFive.Room
{
    /// <summary>
    /// This is a demo script for playing avatar motion.
    /// </summary>
    public sealed class DemoMotionPlayer : NetworkBehaviour
    {
        [SerializeField]
        private AvatarLoader avatarLoader;

        [SerializeField]
        private MotionNetworkController motionNetworkController;

        [SerializeField]
        private AvatarDemoUI uiPrefab;

        private IUIService uiService;
        private ILogger logger;
        private IAvatarContextProvider provider;

        [Inject]
        public void Construct(
            IUIService uiService,
            ILoggerFactory loggerFactory)
        {
            this.uiService = uiService;
            logger = loggerFactory.CreateLogger<DemoMotionPlayer>();
        }

        private void Awake()
        {
            // Only mobile phone can use this demo.
            if (GameApp.PlatformGroup != PlatformGroup.MobilePhone)
            {
                return;
            }

            if (avatarLoader == null)
            {
                logger.LogError($"{nameof(avatarLoader)} is null");

                return;
            }

            if (avatarLoader.IsDone)
            {
                OnAvatarInitialized();

                return;
            }

            avatarLoader.OnLoaded.AddListener(OnAvatarInitialized);
        }

        private void OnAvatarInitialized()
        {
            avatarLoader.OnLoaded.RemoveListener(OnAvatarInitialized);

            if (!HasInputAuthority || !avatarLoader.IsDone)
            {
                return;
            }

            provider = GetComponentInChildren<IAvatarContextProvider>();

            if (!provider.IsAlive())
            {
                logger.LogError($"Missing {nameof(IAvatarContextProvider)} component");
                return;
            }

            if (uiPrefab == null)
            {
                logger.LogError($"{nameof(uiPrefab)} is null");
                return;
            }

            var bundle = new Bundle();
            bundle.Put(nameof(MotionNetworkController), motionNetworkController);
            bundle.Put(nameof(IAvatarContextProvider), provider);
            uiService.ShowWindow<AvatarDemoUI>(bundle);
        }
    }
}