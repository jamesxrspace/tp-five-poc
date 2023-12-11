using Loxodon.Framework.Binding;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Views;
using Loxodon.Log;
using MessagePipe;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.Unity3D;
using TPFive.Extended.Camera;
using TPFive.Extended.InputXREvent;
using TPFive.Game.ApplicationConfiguration;
using TPFive.Game.Avatar.Factory;
using TPFive.Game.Mocap;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TPFive.Game.App.Entry
{
    // App created references.
    using TPFive.Extended.ResourceLoader;
    using TPFive.Game.FlutterUnityWidget;
    using TPFive.Game.GameServer;
    using TPFive.Game.Hud;
    using TPFive.Game.Messages;
    using TPFive.Game.PlayerPrefs;
    using TPFive.Game.UI;
    using TPFive.Model;
    using TPFive.OpenApi.GameServer;
    using TPFive.Room;
    using TPFive.SCG.ServiceEco.Abstractions;

    // Using aliases.
    using GameMessages = TPFive.Game.Messages;

    /// <summary>
    /// This is the first lifetime scope that is created in the application.
    /// </summary>
    [RegisterService(ServiceList = @"
TPFive.Game.Logging.Service,
TPFive.Game.Camera.Service,
TPFive.Game.Video.Service,
TPFive.Game.Config.Service,
TPFive.Game.Resource.Service,
TPFive.Game.SceneFlow.Service,
TPFive.Game.ObjectPool.Service,
TPFive.Game.Audio.Service,
TPFive.Game.UI.Service,
TPFive.Game.Hud.Service,
TPFive.Game.Input.Service,
TPFive.Game.Localization.Service,
TPFive.Game.Actor.Service,
TPFive.Game.Analytics.Service,
TPFive.Game.Diagnostics.Service,
TPFive.Game.ReferenceLocator.Service,
TPFive.Game.Account.Service,
TPFive.Game.Login.Service,
TPFive.Game.Mocap.Service,
TPFive.Game.FlutterUnityWidget.Service,
TPFive.Game.Zone.Service,
TPFive.Game.Decoration.Service,
TPFive.Game.PlayerPrefs.Service,
TPFive.Extended.InputDeviceProvider.OnScreen.Service")]
    public sealed partial class LifetimeScope : VContainer.Unity.LifetimeScope
    {
        /// <summary>
        /// Settings is a class that contains settings for the application.
        /// </summary>
        public Settings settings;

        /// <summary>
        /// SceneSettings is a class that contains settings for the scene.
        /// </summary>
        public SceneSettings sceneSettings;

        /// <summary>
        /// Login options.
        /// </summary>
        public Login.Options loginOptions;

        private static DefaultLogFactory logFactory;

        [SerializeField]
        private UIConfigurationBase uiConfiguration;
        [SerializeField]
        private TextureManager textureManager;

        [SerializeField]
        private MocapServiceSettings mocapServiceSettings;

        [SerializeField]
        private AvatarFactorySettings defaultAvatarFactorySettings;

        [SerializeField]
        private PassFadeVFX passFadeVFX;

        [SerializeField]
        private WindowAssetLocatorSetting windowAssetLocatorSetting;

        [SerializeField]
        private CinemachinePriorityConfig cinemachinePriorityConfig;

        [SerializeField]
        private PlatformInputSettings platformInputSettings;

        private LoggerConfiguration loggerConfiguration;
        private Serilog.Core.Logger log;

        private IAppConfigService appConfigService;

        /// <summary>
        /// This method is called when the lifetime scope is created.
        /// </summary>
        /// <param name="builder">Container builder.</param>
        protected override void Configure(IContainerBuilder builder)
        {
            RegisterLoggerUseDependencies(builder);

            RegisterLoxodonframeworkDependencies(builder);

            var options = builder.RegisterMessagePipe(pipeOptions => { });
            builder.RegisterBuildCallback(c => GlobalMessagePipe.SetProvider(c.AsServiceProvider()));

            RegisterMessageUseDependencies(builder, options);
            RegisterInstallers(builder, options);
            RegisterAppInfoDependencies(builder);
            InstallGameServerApis(builder);

            builder.RegisterInstance(settings);
            builder.RegisterInstance(sceneSettings);
            builder.RegisterInstance(loginOptions);
            builder.RegisterInstance<IUIConfiguration>(uiConfiguration);
            builder.RegisterInstance(textureManager);
            var windowAssetLocator = GetWindowLocator(GameApp.PlatformGroup, Application.isEditor);
            builder.RegisterInstance(windowAssetLocator);
            builder.RegisterInstance(cinemachinePriorityConfig);

            builder.Register<Game.ReferenceLocator.Service>(Lifetime.Scoped).As<Game.ReferenceLocator.IService>();
            builder.Register<OpenRoomCmd>(Lifetime.Singleton).As<IOpenRoomCmd>();
            builder.RegisterComponent(passFadeVFX).As<ISceneTransition>();
            builder.Register<BinfileHandler>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<DefaultAvatarFactory>(Lifetime.Singleton)
                .AsSelf()
                .As<IAvatarFactory>();
            builder.RegisterInstance(mocapServiceSettings);
            builder.RegisterInstance(defaultAvatarFactorySettings);
            builder.RegisterInstance(platformInputSettings);
            builder.RegisterEntryPoint<UnityEventBridge>();
            builder.RegisterEntryPoint<Bootstrap>();
        }

        private WindowAssetLocator GetWindowLocator(PlatformGroup platformGroup, bool isEditor)
        {
            var windowAssetLocator = windowAssetLocatorSetting.GetAsset(platformGroup, isEditor);
            windowAssetLocator = windowAssetLocator != null ? windowAssetLocator : ScriptableObject.CreateInstance<WindowAssetLocator>();
            return windowAssetLocator;
        }

        private static void RegisterLoxodonframeworkDependencies(IContainerBuilder builder)
        {
            var context = Context.GetApplicationContext();
            var container = context.GetContainer();

            // Initialize the data binding service
            var bundle = new BindingServiceBundle(container);
            bundle.Start();

            builder.Register<IUIViewLocator, AddressableUIViewLocator>(Lifetime.Singleton);
            builder.RegisterInstance(context.GetGlobalPreferences());

            // Enable window state broadcast
            GlobalSetting.enableWindowStateBroadcast = true;

            // Use the CanvasGroup.blocksRaycasts instead of the CanvasGroup.interactable
            // to control the interactivity of the view
            GlobalSetting.useBlocksRaycastsInsteadOfInteractable = true;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnSubsystemRegistration()
        {
            if (logFactory == null)
            {
                logFactory = new DefaultLogFactory();
                LogManager.Registry(logFactory);
            }
        }

        private void RegisterLoggerUseDependencies(IContainerBuilder builder)
        {
            var factory = new LoggerFactory();

            loggerConfiguration = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Unity3D(outputTemplate: "[{Level:u3}][{SourceContext}] {Message:j}{NewLine}{Exception}\n");

            loggerConfiguration.MinimumLevel.Debug();

            loggerConfiguration
                .WriteTo.Seq("http://localhost:5341");

            log = loggerConfiguration.CreateLogger();

            factory.AddSerilog(log);

            builder.RegisterInstance<ILoggerFactory>(factory);
        }

        private void RegisterMessageUseDependencies(IContainerBuilder builder, MessagePipeOptions options)
        {
            builder.RegisterMessageBroker<GameMessages.BootstrapJustStarted>(options);
            builder.RegisterMessageBroker<GameMessages.BootstrapSetupDone>(options);

            builder.RegisterMessageBroker<TPFive.Game.SceneFlow.ChangeScene>(options);
            builder.RegisterMessageBroker<GameMessages.SceneLoading>(options);
            builder.RegisterMessageBroker<GameMessages.SceneLoaded>(options);
            builder.RegisterMessageBroker<GameMessages.SceneUnloading>(options);
            builder.RegisterMessageBroker<GameMessages.SceneUnloaded>(options);

            builder.RegisterMessageBroker<GameMessages.MultiPhaseSetupDone>(options);

            builder.RegisterMessageBroker<LoginSuccess>(options);
            builder.RegisterMessageBroker<LogoutComplete>(options);

            builder.RegisterMessageBroker<GameMessages.ApplicationQuit>(options);
            builder.RegisterMessageBroker<GameMessages.ApplicationFoucs>(options);
            builder.RegisterMessageBroker<GameMessages.ApplicationPause>(options);

            builder.RegisterMessageBroker<GameMessages.HudMessage>(options);
            builder.RegisterMessageBroker<GameMessages.FirebaseInitialize>(options);

            builder.RegisterMessageBroker<FlutterMessage>(options);
            builder.RegisterMessageBroker<PostUnityMessage>(options);

            builder.RegisterMessageBroker<GameMessages.BackToHome>(options);
            builder.RegisterMessageBroker<GameMessages.LoadContentLevel>(options);
            builder.RegisterMessageBroker<GameMessages.UnloadContentLevel>(options);
            builder.RegisterMessageBroker<GameMessages.ContentLevelFullyLoaded>(options);
            builder.RegisterMessageBroker<GameMessages.NotifyNetLoaderToUnload>(options);

            builder.RegisterMessageBroker<GameMessages.AvatarSitDownMessage>(options);
            builder.RegisterMessageBroker<GameMessages.AvatarStandUpMessage>(options);

            builder.RegisterMessageBroker<PrefsVariance>(options);
        }

        private void RegisterAppInfoDependencies(IContainerBuilder builder)
        {
            appConfigService = new AppConfigService();

            // Preload the app config.
            var appConfig = appConfigService.GetAppInfo();

            builder.RegisterInstance(appConfig).As<IReadOnlyAppInfo>();
        }

        private void InstallGameServerApis(IContainerBuilder builder)
        {
            builder.Register<IGameServerBaseUriProvider, GameServer.GameServerBaseUriProvider>(Lifetime.Singleton);
            builder.Register<IGameServerAuthTokenProvider, GameServer.GameServerAuthTokenProvider>(Lifetime.Singleton);

            var gameServerApiInstaller = new GameServerApiInstaller(Lifetime.Singleton);
            gameServerApiInstaller.Install(builder);
        }

        partial void RegisterInstallers(IContainerBuilder builder, MessagePipeOptions options);

        /// <summary>
        /// SceneSettings is a class that contains settings for the scene.
        /// </summary>
        [System.Serializable]
        public class SceneSettings
        {
            // TODO: Might not be necessary.
            public GameObject networkSceneManagerDefaultGO;
        }
    }
}
