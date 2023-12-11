using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Firebase.Extensions;
using MessagePipe;
using Microsoft.Extensions.Logging;
// App created references.
using TPFive.Game.Logging;
using TPFive.SCG.AsyncStartable.Abstractions;
using TPFive.SCG.DisposePattern.Abstractions;
using TPFive.SCG.ServiceEco.Abstractions;
using UniRx;
using UnityEngine.SceneManagement;
using VContainer;
// Using aliases.
using GameLoggingUtility = TPFive.Game.Logging.Utility;
using GameMessages = TPFive.Game.Messages;

// Using aliases by conditions.
#if HAS_TPFIVE_EXTENDED_ADDRESSABLE
using ExtendedAddressable = TPFive.Extended.Addressable;
#endif

#if HAS_TPFIVE_EXTENDED_CAMERA
using ExtendedCamera = TPFive.Extended.Camera;
#endif

#if HAS_TPFIVE_EXTENDED_VIDEO_AVPRO
using ExtendedVideoAVPro = TPFive.Extended.Video.AVPro;
#endif

#if HAS_TPFIVE_EXTENDED_DOOZY
using ExtendedDoozy = TPFive.Extended.Doozy;
#endif

#if HAS_TPFIVE_EXTENDED_FIREBASE_REMOTECONFIG
using ExtendedFirebaseRemoteConfig = TPFive.Extended.FirebaseRemoteConfig;
#endif

#if HAS_TPFIVE_EXTENDED_UNITY_REMOTECONFIG
using ExtendedUnityRemoteConfig = TPFive.Extended.UnityRemoteConfig;
#endif

#if HAS_TPFIVE_EXTENDED_LOCAL_CONFIG
using ExtendedLocalConfig = TPFive.Extended.LocalConfig;
#endif

#if HAS_TPFIVE_EXTENDED_ECM2
using ExtendedECM2 = TPFive.Extended.ECM2;
#endif

#if HAS_TPFIVE_EXTENDED_INPUT_SYSTEM
using TPFive.Extended.InputSystem;
#endif

#if HAS_TPFIVE_EXTENDED_MASTER_AUDIO
using ExtendedMasterAudio = TPFive.Extended.MasterAudio;
#endif

#if HAS_TPFIVE_EXTENDED_POOLKIT
using ExtendedPoolKit = TPFive.Extended.PoolKit;
#endif

#if HAS_TPFIVE_EXTENDED_ZONE
using ExtendedZone = TPFive.Extended.Zone;
#endif

#if HAS_TPFIVE_EXTENDED_DECORATION
using ExtendedDecoration = TPFive.Extended.Decoration;
#endif

#if HAS_TPFIVE_EXTENDED_SCENE_ACTIVITY
using ExtendedSceneActivity = TPFive.Extended.SceneActivity;
#endif

using TPFive.Game.UI;
using ExtendedGoogleAnalytics = TPFive.Extended.GoogleAnalytics;

namespace TPFive.Game.App.Entry
{
    // TODO: This tag becomes too huge, should deal with it later

    /// <summary>
    /// This is the first class for each entry scene
    /// </summary>
    [InjectService(
        Setup = "true",
        DeclareSettings = "true",
        AddLifetimeScope = "true",
        ServiceList = @"
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
TPFive.Game.Login.Service,
TPFive.Game.Zone.Service,
TPFive.Game.Decoration.Service",
        PubMessageList = @"
TPFive.Game.Messages.BootstrapJustStarted")]
    [AsyncStartable(ExceptionList = "System.OperationCanceledException, System.Exception")]
    [Dispose]
    [AsyncDispose]
    public sealed partial class Bootstrap
    {
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        private readonly UniTaskCompletionSource<bool> _utcs = new UniTaskCompletionSource<bool>();

        private readonly LifetimeScope _lifetimeScope;

        private readonly Settings _settings;
        private readonly LifetimeScope.SceneSettings _sceneSettings;

        private readonly TPFive.Game.Logging.IService _service;
        private readonly TPFive.Game.Camera.IService _serviceCamera;
        private readonly TPFive.Game.Video.IService _serviceVideo;
        private readonly TPFive.Game.Config.IService _serviceConfig;
        private readonly TPFive.Game.Resource.IService _serviceResource;
        private readonly TPFive.Game.SceneFlow.IService _serviceSceneFlow;
        private readonly TPFive.Game.ObjectPool.IService _serviceObjectPool;
        private readonly TPFive.Game.Audio.IService _serviceAudio;
        private readonly TPFive.Game.UI.IService _serviceUI;
        private readonly TPFive.Game.Hud.IService _serviceHud;
        private readonly TPFive.Game.Input.IService _serviceInput;
        private readonly TPFive.Game.Localization.IService _serviceLocalization;
        private readonly TPFive.Game.Actor.IService _serviceActor;
        private readonly TPFive.Game.Analytics.IService _serviceAnalytics;
        private readonly TPFive.Game.Diagnostics.IService _serviceDiagnostics;
        private readonly TPFive.Game.Login.IService _serviceLogin;
        private readonly TPFive.Game.Zone.IService _serviceZone;
        private readonly TPFive.Game.Decoration.IService _serviceDecoration;
        private readonly TPFive.Game.ReferenceLocator.IService _serviceReferenceLocator;
        private readonly TPFive.Game.FlutterUnityWidget.IService _serviceFlutterUnityWidget;
        private readonly TPFive.Game.PlayerPrefs.IService _servicePlayerPrefs;
        private readonly TPFive.OpenApi.GameServer.IDecorationApi _decorationApi;
        private readonly TPFive.Extended.ResourceLoader.TextureManager _textureManager;

        private readonly TPFive.Room.IOpenRoomCmd _openRoomCmd;
        private readonly IPublisher<TPFive.Game.Messages.BootstrapJustStarted> _pubBootstrapJustStarted;
        private readonly IPublisher<TPFive.Game.Messages.BootstrapSetupDone> _pubBootstrapSetupDone;

        private readonly IPublisher<TPFive.Game.Messages.SceneLoading> _pubSceneLoading;
        private readonly IPublisher<TPFive.Game.Messages.SceneLoaded> _pubSceneLoaded;
        private readonly IPublisher<TPFive.Game.Messages.SceneUnloading> _pubSceneUnloading;
        private readonly IPublisher<TPFive.Game.Messages.SceneUnloaded> _pubSceneUnloaded;

        private readonly IPublisher<TPFive.Game.Messages.HudMessage> _pubHudMessage;
        private readonly IPublisher<TPFive.Game.Messages.FirebaseInitialize> _pubFirebaseInitialize;

        private readonly IPublisher<TPFive.Game.FlutterUnityWidget.PostUnityMessage> _pubPostUnityMessage;

        private readonly IPublisher<TPFive.Game.Messages.BackToHome> _pubBackToHome;
        private readonly IPublisher<TPFive.Game.Messages.LoadContentLevel> _pubLoadContentLevel;
        private readonly IPublisher<TPFive.Game.Messages.UnloadContentLevel> _pubUnloadContentLevel;
        private readonly IPublisher<TPFive.Game.Messages.NotifyNetLoaderToUnload> _pubNotifyNetLoaderToUnload;
        private readonly IPublisher<TPFive.Game.Messages.ContentLevelFullyLoaded> _pubContentLevelFullyLoaded;

        private readonly IAsyncPublisher<GameMessages.MultiPhaseSetupDone> _asyncPubMultiPhaseSetupDone;

        private readonly ISubscriber<TPFive.Game.Messages.SceneLoading> _subSceneLoading;
        private readonly ISubscriber<TPFive.Game.Messages.SceneLoaded> _subSceneLoaded;
        private readonly ISubscriber<TPFive.Game.Messages.SceneUnloading> _subSceneUnloading;
        private readonly ISubscriber<TPFive.Game.Messages.SceneUnloaded> _subSceneUnloaded;
        private readonly ISubscriber<TPFive.Game.Messages.FirebaseInitialize> _subFirebaseInitialize;

        private readonly ISubscriber<TPFive.Game.Messages.BackToHome> _subBackToHome;
        private readonly ISubscriber<TPFive.Game.Messages.LoadContentLevel> _subLoadContentLevel;
        private readonly ISubscriber<TPFive.Game.Messages.UnloadContentLevel> _subUnloadContentLevel;
        private readonly ISubscriber<TPFive.Game.Messages.ContentLevelFullyLoaded> _subContentLevelFullyLoaded;

        private readonly IAsyncSubscriber<GameMessages.MultiPhaseSetupDone> _asyncSubMultiPhaseSetupDone;

        private readonly List<IServiceProvider> _createdServiceProviders =
            new List<IServiceProvider>();

        private Microsoft.Extensions.Logging.ILoggerFactory _loggerFactory;
        private SceneTransitionHandler _sceneTransitionHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="Bootstrap"/> class.
        /// </summary>
        /// <param name="loggerFactory">Microsoft logger factor.</param>
        /// <param name="lifetimeScope">LifetimeScope in game.app.</param>
        /// <param name="settings">Settings in game.app.</param>
        /// <param name="sceneSettings">SceneSettings in game.app.</param>
        /// <remarks>
        /// The order is kept so it looks easier to read.
        /// </remarks>
        [Inject]
        public Bootstrap(
            Microsoft.Extensions.Logging.ILoggerFactory loggerFactory,
            // VContainer.
            LifetimeScope lifetimeScope,
            // Settings.
            Settings settings,
            LifetimeScope.SceneSettings sceneSettings,
            // Service Parameters.
            TPFive.Game.Logging.IService serviceLogging,
            TPFive.Game.Camera.IService serviceCamera,
            TPFive.Game.Video.IService serviceVideo,
            TPFive.Game.Config.IService serviceConfig,
            TPFive.Game.Resource.IService serviceResource,
            TPFive.Game.SceneFlow.IService serviceSceneFlow,
            TPFive.Game.ObjectPool.IService serviceObjectPool,
            TPFive.Game.Audio.IService serviceAudio,
            TPFive.Game.UI.IService serviceUI,
            TPFive.Game.Hud.IService serviceHud,
            TPFive.Game.Input.IService serviceInput,
            TPFive.Game.Localization.IService serviceLocalization,
            TPFive.Game.Actor.IService serviceActor,
            TPFive.Game.Analytics.IService serviceAnalytics,
            TPFive.Game.Diagnostics.IService serviceDiagnostics,
            TPFive.Game.Login.IService serviceLogin,
            TPFive.Game.Zone.IService serviceZone,
            TPFive.Game.Decoration.IService serviceDecoration,
            TPFive.Game.ReferenceLocator.IService serviceReferenceLocator,
            TPFive.Game.FlutterUnityWidget.IService serviceFlutterUnityWidget,
            TPFive.Game.PlayerPrefs.IService servicePlayerPrefs,
            TPFive.OpenApi.GameServer.IDecorationApi decorationApi,
            TPFive.Extended.ResourceLoader.TextureManager textureManager,
            // Command
            TPFive.Room.IOpenRoomCmd openRoomCmd,
            // Publish Message Parameters.
            IPublisher<TPFive.Game.Messages.BootstrapJustStarted> pubBootstrapJustStarted,
            IPublisher<TPFive.Game.Messages.BootstrapSetupDone> pubBootstrapSetupDone,
            IPublisher<TPFive.Game.Messages.SceneLoading> pubSceneLoading,
            IPublisher<TPFive.Game.Messages.SceneLoaded> pubSceneLoaded,
            IPublisher<TPFive.Game.Messages.SceneUnloading> pubSceneUnloading,
            IPublisher<TPFive.Game.Messages.SceneUnloaded> pubSceneUnloaded,
            IPublisher<TPFive.Game.Messages.HudMessage> pubHudMessage,
            IPublisher<TPFive.Game.Messages.FirebaseInitialize> pubFirebaseInitialize,
            IPublisher<TPFive.Game.FlutterUnityWidget.PostUnityMessage> pubPostUnityMessage,
            IPublisher<TPFive.Game.Messages.BackToHome> pubBackToHome,
            IPublisher<TPFive.Game.Messages.LoadContentLevel> pubLoadContentLevel,
            IPublisher<TPFive.Game.Messages.NotifyNetLoaderToUnload> pubNotifyNetLoaderToUnload,
            IPublisher<TPFive.Game.Messages.ContentLevelFullyLoaded> pubContentLevelFullyLoaded,
            // Async Publish Message Parameters.
            IAsyncPublisher<GameMessages.MultiPhaseSetupDone> asyncPubMultiPhaseSetupDone,
            // Subscribe Message Parameters.
            ISubscriber<TPFive.Game.Messages.SceneLoading> subSceneLoading,
            ISubscriber<TPFive.Game.Messages.SceneLoaded> subSceneLoaded,
            ISubscriber<TPFive.Game.Messages.SceneUnloading> subSceneUnloading,
            ISubscriber<TPFive.Game.Messages.SceneUnloaded> subSceneUnloaded,
            ISubscriber<TPFive.Game.Messages.FirebaseInitialize> subFirebaseInitialize,
            ISubscriber<TPFive.Game.Messages.BackToHome> subBackToHome,
            ISubscriber<TPFive.Game.Messages.LoadContentLevel> subLoadContentLevel,
            ISubscriber<TPFive.Game.Messages.UnloadContentLevel> subUnloadContentLevel,
            ISubscriber<TPFive.Game.Messages.ContentLevelFullyLoaded> subContentLevelFullyLoaded,
            // Async Subscribe Message Parameters.
            IAsyncSubscriber<GameMessages.MultiPhaseSetupDone> asyncSubMultiPhaseSetupDone,
            ISceneTransition sceneTransition)
        {
            _loggerFactory = loggerFactory;
            Logger = GameLoggingUtility.CreateLogger<Bootstrap>(loggerFactory);

            _lifetimeScope = lifetimeScope;

            _settings = settings;
            _sceneSettings = sceneSettings;
            _textureManager = textureManager;

            _service = serviceLogging;
            _serviceCamera = serviceCamera;
            _serviceVideo = serviceVideo;
            _serviceConfig = serviceConfig;
            _serviceResource = serviceResource;
            _serviceSceneFlow = serviceSceneFlow;
            _serviceObjectPool = serviceObjectPool;
            _serviceAudio = serviceAudio;
            _serviceUI = serviceUI;
            _serviceHud = serviceHud;
            _serviceInput = serviceInput;
            _serviceLocalization = serviceLocalization;
            _serviceActor = serviceActor;
            _serviceAnalytics = serviceAnalytics;
            _serviceDiagnostics = serviceDiagnostics;
            _serviceLogin = serviceLogin;
            _serviceZone = serviceZone;
            _serviceDecoration = serviceDecoration;
            _serviceReferenceLocator = serviceReferenceLocator;
            _serviceFlutterUnityWidget = serviceFlutterUnityWidget;
            _servicePlayerPrefs = servicePlayerPrefs;
            _decorationApi = decorationApi;

            _openRoomCmd = openRoomCmd;

            _pubBootstrapJustStarted = pubBootstrapJustStarted;
            _pubBootstrapSetupDone = pubBootstrapSetupDone;

            _pubSceneLoading = pubSceneLoading;
            _pubSceneLoaded = pubSceneLoaded;
            _pubSceneUnloading = pubSceneUnloading;
            _pubSceneUnloaded = pubSceneUnloaded;
            _pubFirebaseInitialize = pubFirebaseInitialize;
            _pubPostUnityMessage = pubPostUnityMessage;

            _pubBackToHome = pubBackToHome;
            _pubLoadContentLevel = pubLoadContentLevel;
            _pubNotifyNetLoaderToUnload = pubNotifyNetLoaderToUnload;
            _pubContentLevelFullyLoaded = pubContentLevelFullyLoaded;

            _pubHudMessage = pubHudMessage;

            _asyncPubMultiPhaseSetupDone = asyncPubMultiPhaseSetupDone;

            _subSceneLoading = subSceneLoading;
            _subSceneLoaded = subSceneLoaded;
            _subSceneUnloading = subSceneUnloading;
            _subSceneUnloaded = subSceneUnloaded;
            _subFirebaseInitialize = subFirebaseInitialize;
            _subBackToHome = subBackToHome;
            _subLoadContentLevel = subLoadContentLevel;
            _subUnloadContentLevel = subUnloadContentLevel;
            _subContentLevelFullyLoaded = subContentLevelFullyLoaded;

            _asyncSubMultiPhaseSetupDone = asyncSubMultiPhaseSetupDone;

            _funcOperationCanceledException = HandleStartAsyncOperationCanceledException;
            _funcException = HandleStartAsyncException;

            _sceneTransitionHandler = new SceneTransitionHandler(
                subSceneLoading,
                subSceneLoaded,
                sceneTransition,
                loggerFactory);
        }

        private Microsoft.Extensions.Logging.ILogger Logger { get; }

        // AsyncStartable
        private async UniTask SetupBegin(CancellationToken cancellationToken = default)
        {
            Game.FlutterUnityWidget.Utility.SendGeneralMessage(
                _pubPostUnityMessage,
                $"{nameof(Bootstrap)} - Begin {nameof(SetupBegin)}");

            Logger.LogEditorDebug(
                "{Method}",
                nameof(SetupBegin));

            await SetupMessageHandling(cancellationToken);

            _pubBootstrapJustStarted.Publish(
                new GameMessages.BootstrapJustStarted
                {
                    Category = _settings.category,
                });

            await SetupServices(cancellationToken);
            await SetupExtServicesPhase1(cancellationToken);
            await SetupExtServicesPhase2(cancellationToken);

            Game.FlutterUnityWidget.Utility.SendGeneralMessage(
                _pubPostUnityMessage,
                $"{nameof(Bootstrap)} - Finish {nameof(SetupBegin)}");
        }

        private async UniTask SetupExtServicesPhase1(
            CancellationToken cancellationToken = default)
        {
            Logger.LogEditorDebug(
                "{Method}",
                nameof(SetupExtServicesPhase1));

#if HAS_TPFIVE_EXTENDED_ADDRESSABLE
            {
                var nullServiceProvider = _serviceResource.NullServiceProvider;
                var serviceProvider = new ExtendedAddressable.ServiceProvider(
                    _loggerFactory,
                    _serviceConfig,
                    nullServiceProvider,
                    _pubPostUnityMessage,
                    _asyncSubMultiPhaseSetupDone);

                _createdServiceProviders.Add(serviceProvider);
                await serviceProvider.StartAsync(cancellationToken);
                await ((Game.IServiceProviderManagement)_serviceResource)
                    .AddServiceProvider((int)ServiceProviderKind.Rank1ServiceProvider, serviceProvider);
            }
#endif
            {
                var nullServiceProvider = _serviceResource.NullServiceProvider;
                var serviceProvider = new Extended.ResourceLoader.ServiceProvider(_loggerFactory, _textureManager);
                _createdServiceProviders.Add(serviceProvider);
                await ((Game.IServiceProviderManagement)_serviceResource)
                    .AddServiceProvider((int)ServiceProviderKind.Rank2ServiceProvider, serviceProvider);
            }
#if HAS_TPFIVE_EXTENDED_LOCAL_CONFIG
            {
                var nullServiceProvider = _serviceConfig.NullServiceProvider;
                var serviceProvider = new ExtendedLocalConfig.ServiceProvider(
                    _loggerFactory,
                    nullServiceProvider,
                    _asyncPubMultiPhaseSetupDone);

                _createdServiceProviders.Add(serviceProvider);
                await ((Game.IServiceProviderManagement)_serviceConfig)
                    .AddServiceProvider(Config.Constants.RuntimeLocalProviderIndex, serviceProvider);
                await serviceProvider.StartAsync(cancellationToken);

                // Setup config of entry settings
                await SetupLocalConfig(cancellationToken);
            }
#endif

#if HAS_TPFIVE_EXTENDED_FIREBASE_REMOTECONFIG
            {
                var nullServiceProvider = _serviceConfig.NullServiceProvider;
                var serviceProvider = new ExtendedFirebaseRemoteConfig.ServiceProvider(
                    _loggerFactory,
                    nullServiceProvider,
                    _asyncPubMultiPhaseSetupDone);

                _createdServiceProviders.Add(serviceProvider);
                await ((Game.IServiceProviderManagement)_serviceConfig)
                    .AddServiceProvider(Config.Constants.FirebaseProviderIndex, serviceProvider);
                await serviceProvider.StartAsync(cancellationToken);
            }
#endif

#if HAS_TPFIVE_EXTENDED_UNITY_REMOTECONFIG
            {
                var nullServiceProvider = _serviceConfig.NullServiceProvider;
                var serviceProvider = new ExtendedUnityRemoteConfig.ServiceProvider(
                    _loggerFactory,
                    nullServiceProvider,
                    _asyncPubMultiPhaseSetupDone);

                _createdServiceProviders.Add(serviceProvider);
                await ((Game.IServiceProviderManagement)_serviceConfig)
                    .AddServiceProvider(Config.Constants.UnityProviderIndex, serviceProvider);
                await serviceProvider.StartAsync(cancellationToken);
            }
#endif

#if HAS_TPFIVE_EXTENDED_VIDEO_AVPRO
            {
                var serviceProvider = new ExtendedVideoAVPro.AVProVideoServiceProvider(
                    _loggerFactory,
                    _lifetimeScope.Container);

                if (serviceProvider != null)
                {
                    _createdServiceProviders.Add(serviceProvider);
                    await ((Game.IServiceProviderManagement)_serviceVideo)
                        .AddServiceProvider((int)ServiceProviderKind.Rank1ServiceProvider, serviceProvider);
                }
            }
#endif

#if HAS_TPFIVE_EXTENDED_CAMERA
            {
                Camera.IServiceProvider serviceProvider = null;

                if (GameApp.IsMobilePhone || GameApp.IsStandalone)
                {
                    serviceProvider = new ExtendedCamera.MobileServiceProvider(
                        _loggerFactory);
                }

                if (serviceProvider != null)
                {
                    _createdServiceProviders.Add(serviceProvider);
                    await ((Game.IServiceProviderManagement)_serviceCamera)
                        .AddServiceProvider((int)ServiceProviderKind.Rank1ServiceProvider, serviceProvider);
                }
            }
#endif

#if HAS_TPFIVE_EXTENDED_INPUT_SYSTEM
            {
                var serviceProvider = new DefaultInputActionServiceProvider(
                    _loggerFactory);

                _createdServiceProviders.Add(serviceProvider);
                await ((Game.IServiceProviderManagement)_serviceInput)
                    .AddServiceProvider((int)ServiceProviderKind.Rank1ServiceProvider, serviceProvider);
            }
#endif

            {
                var nullServiceProvider = _serviceAnalytics.NullServiceProvider;
                var serviceProvider = new ExtendedGoogleAnalytics.ServiceProvider(
                    _loggerFactory,
                    nullServiceProvider,
                    _subFirebaseInitialize,
                    _asyncPubMultiPhaseSetupDone);

                _createdServiceProviders.Add(serviceProvider);
                await ((Game.IServiceProviderManagement)_serviceAnalytics)
                    .AddServiceProvider((int)ServiceProviderKind.Rank1ServiceProvider, serviceProvider);
                await serviceProvider.StartAsync(cancellationToken);
            }

            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus != Firebase.DependencyStatus.Available)
                {
                    Logger.LogError(
                        "Could not resolve all Firebase dependencies: {dependencyStatus}",
                        dependencyStatus);
                    return;
                }

                _pubFirebaseInitialize.Publish(new GameMessages.FirebaseInitialize
                {
                    Success = true,
                });
            });
        }

        /// <summary>
        /// Setup Phase2. Service providers should be loaded according to
        /// configs.
        /// </summary>
        /// <param name="cancellationToken">Cancellation usage.</param>
        /// <remarks>
        /// Service of Config should be fully functional, and should be
        /// used for creating other service providers.
        /// </remarks>
        private async UniTask SetupExtServicesPhase2(
            CancellationToken cancellationToken = default)
        {
            Logger.LogEditorDebug(
                "{Method}",
                nameof(SetupExtServicesPhase2));

#if HAS_TPFIVE_EXTENDED_SCENE_ACTIVITY
            {
                var nullServiceProvider = _serviceSceneFlow.NullServiceProvider;
                var serviceProvider = new ExtendedSceneActivity.ServiceProvider(
                    _loggerFactory,
                    _serviceConfig,
                    _serviceResource,
                    nullServiceProvider,
                    _pubSceneLoading,
                    _pubSceneLoaded,
                    _pubSceneUnloading,
                    _pubSceneUnloaded,
                    _pubPostUnityMessage,
                    _subSceneLoading,
                    _subSceneLoaded,
                    _subSceneUnloading,
                    _subSceneUnloaded);

                _createdServiceProviders.Add(serviceProvider);
                await serviceProvider.StartAsync(cancellationToken);
                await ((Game.IServiceProviderManagement)_serviceSceneFlow)
                    .AddServiceProvider(10, serviceProvider);
            }
#endif

#if HAS_TPFIVE_EXTENDED_MASTER_AUDIO
            {
                var nullServiceProvider = _serviceAudio.NullServiceProvider;
                var serviceProvider = new ExtendedMasterAudio.ServiceProvider(
                    _loggerFactory,
                    _serviceConfig,
                    nullServiceProvider);

                _createdServiceProviders.Add(serviceProvider);
                await serviceProvider.StartAsync(cancellationToken);
                await ((Game.IServiceProviderManagement)_serviceAudio)
                    .AddServiceProvider(10, serviceProvider);
            }
#endif

#if HAS_TPFIVE_EXTENDED_POOLKIT
            {
                var nullServiceProvider = _serviceObjectPool.NullServiceProvider;
                var serviceProvider = new ExtendedPoolKit.ServiceProvider(
                    _loggerFactory,
                    _serviceConfig,
                    nullServiceProvider);

                _createdServiceProviders.Add(serviceProvider);
                await serviceProvider.StartAsync(cancellationToken);
                await ((Game.IServiceProviderManagement)_serviceObjectPool)
                    .AddServiceProvider(10, serviceProvider);
            }
#endif

#if HAS_TPFIVE_EXTENDED_DOOZY
            {
                var nullServiceProvider = _serviceHud.NullServiceProvider;
                var serviceProvider = new ExtendedDoozy.ServiceProvider(
                    _loggerFactory,
                    _serviceConfig,
                    nullServiceProvider,
                    _pubHudMessage,
                    _subSceneLoading,
                    _subSceneLoaded,
                    _subSceneUnloading,
                    _subSceneUnloaded,
                    _settings.overviewSettings.hud);

                _createdServiceProviders.Add(serviceProvider);
                await serviceProvider.StartAsync(cancellationToken);
                await ((Game.IServiceProviderManagement)_serviceHud)
                    .AddServiceProvider(10, serviceProvider);
            }
#endif

#if HAS_TPFIVE_EXTENDED_ECM2
            {
                var nullServiceProvider = _serviceActor.NullServiceProvider;
                var serviceProvider = new ExtendedECM2.ServiceProvider(
                    _loggerFactory,
                    _serviceConfig,
                    nullServiceProvider);

                _createdServiceProviders.Add(serviceProvider);
                await serviceProvider.StartAsync(cancellationToken);
                await ((Game.IServiceProviderManagement)_serviceActor)
                    .AddServiceProvider(10, serviceProvider);
            }
#endif

#if HAS_TPFIVE_EXTENDED_ZONE
            {
                var zoneServiceProviderIndex = (int)ServiceProviderKind.Rank1ServiceProvider;
                var nullServiceProvider = _serviceZone.NullServiceProvider;
                var serviceProvider = new ExtendedZone.ServiceProvider(
                    _loggerFactory,
                    _serviceConfig,
                    _serviceResource,
                    _serviceSceneFlow,
                    nullServiceProvider,
                    _openRoomCmd,
                    _pubNotifyNetLoaderToUnload,
                    _pubContentLevelFullyLoaded,
                    _subBackToHome,
                    _subLoadContentLevel,
                    _subUnloadContentLevel,
                    _subContentLevelFullyLoaded);

                _createdServiceProviders.Add(serviceProvider);
                await serviceProvider.StartAsync(cancellationToken);
                await ((Game.IServiceProviderManagement)_serviceZone)
                    .AddServiceProvider(zoneServiceProviderIndex, serviceProvider);
            }
#if HAS_TPFIVE_EXTENDED_DECORATION
            {
                var decorationServiceProviderIndex = (int)ServiceProviderKind.Rank1ServiceProvider;
                var nullServiceProvider = _serviceDecoration.NullServiceProvider;
                var serviceProvider = new ExtendedDecoration.ServiceProvider(
                    _loggerFactory,
                    _decorationApi,
                    _serviceResource,
                    _subSceneUnloading,
                    nullServiceProvider);
                _createdServiceProviders.Add(serviceProvider);
                await serviceProvider.StartAsync(cancellationToken);
                await ((Game.IServiceProviderManagement)_serviceDecoration)
                    .AddServiceProvider(decorationServiceProviderIndex, serviceProvider);
            }
#endif

#endif
            _serviceSceneFlow.SetupTopmostLifetimeScope(_lifetimeScope);
        }

        private async UniTask SetupEnd(bool success, CancellationToken cancellationToken = default)
        {
            Game.FlutterUnityWidget.Utility.SendGeneralMessage(
                _pubPostUnityMessage,
                $"{nameof(Bootstrap)} - Begin {nameof(SetupEnd)}");

            Logger.LogEditorDebug(
                "{Method}",
                nameof(SetupEnd));

            _pubBootstrapSetupDone.Publish(
                new GameMessages.BootstrapSetupDone
                {
                    Category = _settings.category,
                    Success = success,
                });

            _utcs.TrySetResult(success);

            foreach (var sceneProperty in _settings.ScenePropertyList)
            {
                Logger.LogEditorDebug(
                    "{Method} Will load Assist scene",
                    nameof(SetupEnd));

                if (!sceneProperty.load)
                {
                    continue;
                }

                await _serviceSceneFlow.LoadSceneAsync(
                    sceneProperty.addressableKey,
                    LoadSceneMode.Additive,
                    sceneProperty.categoryOrder,
                    sceneProperty.subOrder,
                    _lifetimeScope,
                    false,
                    cancellationToken);
            }

            var loginSceneProperty = GetSceneProperty("Login");
            await _serviceSceneFlow.LoadSceneAsync(
                loginSceneProperty.addressableKey,
                LoadSceneMode.Additive,
                loginSceneProperty.categoryOrder,
                loginSceneProperty.subOrder,
                _lifetimeScope,
                false,
                cancellationToken);

            Game.FlutterUnityWidget.Utility.SendGeneralMessage(
                _pubPostUnityMessage,
                $"{nameof(Bootstrap)} - Finish {nameof(SetupEnd)}");
        }

        private SceneProperty GetSceneProperty(string name)
        {
            return _settings.ScenePropertyList.FirstOrDefault(x => x.category == name);
        }

        private async Task SetupLocalConfig(CancellationToken cancellationToken = default)
        {
            Logger.LogEditorDebug(
                "{Method}",
                nameof(SetupLocalConfig));

            const ServiceProviderKind localServiceProvider = ServiceProviderKind.Rank1ServiceProvider;

            // This stores whole app settings to config service, as some service needs this settings
            // for some use cases.
            await _serviceConfig.SetScriptableObjectValueAsync(
                localServiceProvider,
                "AppSettings",
                _settings,
                cancellationToken);

            // Add scene info to config so it can be used later.
            const string homeEntry = "HomeEntry";
            const string roomEntry = "RoomEntry";
            const string contentEntry = "ContentEntry";

            var spHome = _settings.ScenePropertyList.FirstOrDefault(x => x.title.Equals(homeEntry));
            var spRoom = _settings.ScenePropertyList.FirstOrDefault(x => x.title.Equals(roomEntry));
            var spContent = _settings.ScenePropertyList.FirstOrDefault(x => x.title.Equals(contentEntry));

            if (spHome != null)
            {
                await _serviceConfig.SetSystemObjectValueAsync(Config.Constants.RuntimeLocalProviderKind, homeEntry, spHome, cancellationToken);
            }

            if (spRoom != null)
            {
                await _serviceConfig.SetSystemObjectValueAsync(Config.Constants.RuntimeLocalProviderKind, roomEntry, spRoom, cancellationToken);
            }

            if (spContent != null)
            {
                await _serviceConfig.SetSystemObjectValueAsync(Config.Constants.RuntimeLocalProviderKind, contentEntry, spContent, cancellationToken);
            }

            await _serviceConfig.SetIntValueAsync(
                Config.Constants.RuntimeLocalProviderKind,
                "RoomContentLoadMaxTime",
                180);

            // Original config service has this settings passed in as constructor, move the config
            // settings here.
            if (_settings.overviewSettings.config is Extended.LocalConfig.LocalConfigData localConfigData)
            {
                foreach (var item in localConfigData.ItemStringList)
                {
                    await _serviceConfig.SetStringValueAsync(
                        localServiceProvider,
                        item.Key,
                        item.Value,
                        cancellationToken);
                }
            }
        }

        private async Task HandleStartAsyncOperationCanceledException(
            System.OperationCanceledException e,
            CancellationToken cancellationToken = default)
        {
            Logger.LogWarning("{Exception}", e);
        }

        private async Task HandleStartAsyncException(
            System.Exception e,
            CancellationToken cancellationToken = default)
        {
            Logger.LogError("{Exception}", e);
        }

        // Dispose
        private void HandleDispose(bool disposing)
        {
            Logger.LogEditorDebug(
                "{Method} - disposing: {Disposing}",
                nameof(HandleDispose),
                disposing);

            if (disposing)
            {
                foreach (var serviceProvider in _createdServiceProviders)
                {
                    if (serviceProvider is System.IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }

                _compositeDisposable?.Dispose();

                // _utcs.TrySetCanceled(_cancellationTokenSource.Token);

                _sceneTransitionHandler?.Dispose();

                _disposed = true;
            }
        }

        // AsyncDispose
        private async ValueTask HandleDisposeAsync()
        {
            await Task.Yield();
        }
    }
}
