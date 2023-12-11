using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MessagePipe;
using Microsoft.Extensions.Logging;
using TPFive.Creator.Components.Interactable;
using TPFive.Game;
using UniRx;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace TPFive.Creator.Entry
{
    using TPFive.Game.Logging;
    using TPFive.SCG.AsyncStartable.Abstractions;
    using TPFive.SCG.DisposePattern.Abstractions;
    using TPFive.SCG.ServiceEco.Abstractions;

    using CreatorMessages = TPFive.Creator.Messages;
    using CrossBridge = TPFive.Cross.Bridge;
    using GameActor = TPFive.Game.Actor;
    using GameAudio = TPFive.Game.Audio;
    using GameConfig = TPFive.Game.Config;
    using GameHud = TPFive.Game.Hud;
    using GameLocalization = TPFive.Game.Localization;
    using GameLoggingUtility = TPFive.Game.Logging.Utility;
    using GameMessages = TPFive.Game.Messages;
    using GameObjectPool = TPFive.Game.ObjectPool;
    using GameReferenceLocator = TPFive.Game.ReferenceLocator;
    using GameResource = TPFive.Game.Resource;
    using GameSceneFlow = TPFive.Game.SceneFlow;

#if HAS_TPFIVE_EXTENDED_ADDRESSABLE
    using ExtendedAddressable = TPFive.Extended.Addressable;
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

#if HAS_TPFIVE_EXTENDED_I2LOCALIZATION
    using ExtendedI2Localization = TPFive.Extended.I2Localization;
#endif

#if HAS_TPFIVE_EXTENDED_MASTER_AUDIO
    using ExtendedMasterAudio = TPFive.Extended.MasterAudio;
#endif

#if HAS_TPFIVE_EXTENDED_SCENE_ACTIVITY
    using ExtendedSceneActivity = TPFive.Extended.SceneActivity;
#endif

#if HAS_TPFIVE_EXTENDED_POOLKIT
    using ExtendedPoolKit = TPFive.Extended.PoolKit;
#endif

#if HAS_TPFIVE_EXTENDED_ECM2
    using ExtendedECM2 = TPFive.Extended.ECM2;
#endif

    using CreatorMessageRepo = TPFive.Creator.MessageRepo;

    using CreatorVSEventRepo = TPFive.Creator.VisualScripting.EventRepo;

    [AsyncStartable(ExceptionList = "System.OperationCanceledException, System.Exception")]
    [Dispose]
    [AsyncDispose]
    public sealed partial class Bootstrap :
        IAsyncStartable
    {
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        private readonly ILoggerFactory _loggerFactory;

        private readonly LifetimeScope _lifetimeScope;

        private readonly Settings _settings;
        private readonly LifetimeScope.SceneSettings _sceneSettings;

        private readonly GameConfig.IService _serviceConfig;
        private readonly GameResource.IService _serviceResource;
        private readonly GameSceneFlow.IService _serviceSceneFlow;
        private readonly GameObjectPool.IService _serviceObjectPool;
        private readonly GameAudio.IService _serviceAudio;
        private readonly GameHud.IService _serviceHud;
        private readonly GameLocalization.IService _serviceLocalization;
        private readonly GameActor.IService _serviceActor;
        private readonly GameReferenceLocator.IService _serviceReferenceLocator;
        private readonly Game.Interactable.Toolkit.IService _serviceInteractable;

        private readonly CreatorMessageRepo.IService _serviceMessageRepo;

        private readonly IPublisher<GameMessages.BootstrapJustStarted> _pubBootstrapJustStarted;
        private readonly IPublisher<GameMessages.BootstrapSetupDone> _pubBootstrapSetupDone;

        private readonly IPublisher<TPFive.Game.Messages.SceneLoading> _pubSceneLoading;
        private readonly IPublisher<TPFive.Game.Messages.SceneLoaded> _pubSceneLoaded;
        private readonly IPublisher<TPFive.Game.Messages.SceneUnloading> _pubSceneUnloading;
        private readonly IPublisher<TPFive.Game.Messages.SceneUnloaded> _pubSceneUnloaded;

        private readonly IPublisher<TPFive.Game.FlutterUnityWidget.PostUnityMessage> _pubPostUnityMessage;

        private readonly IAsyncPublisher<GameMessages.MultiPhaseSetupDone> _asyncPubMultiPhaseSetupDone;

        private readonly IPublisher<GameMessages.HudMessage> _pubHudMessage;

        private readonly IPublisher<CreatorMessages.MarkerMessage> _pubMarkerMessage;
        private readonly IPublisher<Game.Messages.UnloadContentLevel> _pubUnloadContentLevel;

        private readonly IPublisher<Game.Messages.ContentLevelFullyLoaded> _pubContentLevelFullyLoaded;
        private readonly IPublisher<ContentSubLevelLoaded> _pubContentSubLevelLoaded;

        private readonly IAsyncSubscriber<GameMessages.MultiPhaseSetupDone> _asyncSubMultiPhaseSetupDone;

        private readonly ISubscriber<GameMessages.HudMessage> _subHudMessage;

        private readonly ISubscriber<CreatorMessages.MarkerMessage> _subMarkerMessage;
        private readonly ISubscriber<TPFive.Game.Messages.BackToHome> _subBackToHome;

        private readonly ISubscriber<TPFive.Game.Messages.SceneLoading> _subSceneLoading;
        private readonly ISubscriber<TPFive.Game.Messages.SceneLoaded> _subSceneLoaded;
        private readonly ISubscriber<TPFive.Game.Messages.SceneUnloading> _subSceneUnloading;
        private readonly ISubscriber<TPFive.Game.Messages.SceneUnloaded> _subSceneUnloaded;

        private readonly ISubscriber<ContentSubLevelLoaded> _subContentSubLevelLoaded;

        private readonly List<TPFive.Game.IServiceProvider> _createdServiceProviders =
            new List<TPFive.Game.IServiceProvider>();

        private UniTaskCompletionSource<bool> _utcs = new UniTaskCompletionSource<bool>();

        [Inject]
        public Bootstrap(
            ILoggerFactory loggerFactory,
            LifetimeScope lifetimeScope,
            Settings settings,
            LifetimeScope.SceneSettings sceneSettings,
            GameConfig.IService serviceConfig,
            GameResource.IService serviceResource,
            GameSceneFlow.IService serviceSceneFlow,
            GameObjectPool.IService serviceObjectPool,
            GameAudio.IService serviceAudio,
            GameHud.IService serviceHud,
            GameLocalization.IService serviceLocalization,
            GameActor.IService serviceActor,
            GameReferenceLocator.IService serviceReferenceLocator,
            Game.Interactable.Toolkit.IService serviceInteractable,
            CreatorMessageRepo.IService serviceMessageRepo,
            IPublisher<GameMessages.BootstrapJustStarted> pubBootstrapJustStarted,
            IPublisher<GameMessages.BootstrapSetupDone> pubBootstrapSetupDone,
            IPublisher<TPFive.Game.Messages.SceneLoading> pubSceneLoading,
            IPublisher<TPFive.Game.Messages.SceneLoaded> pubSceneLoaded,
            IPublisher<TPFive.Game.Messages.SceneUnloading> pubSceneUnloading,
            IPublisher<TPFive.Game.Messages.SceneUnloaded> pubSceneUnloaded,
            IPublisher<TPFive.Game.FlutterUnityWidget.PostUnityMessage> pubPostUnityMessage,
            IAsyncPublisher<GameMessages.MultiPhaseSetupDone> asyncPubMultiPhaseSetupDone,
            IPublisher<GameMessages.HudMessage> pubHudMessage,
            IPublisher<CreatorMessages.MarkerMessage> pubMarkerMessage,
            IPublisher<Game.Messages.UnloadContentLevel> pubUnloadContentLevel,
            IPublisher<Game.Messages.ContentLevelFullyLoaded> pubContentLevelFullyLoaded,
            IPublisher<ContentSubLevelLoaded> pubContentSubLevelLoaded,
            IAsyncSubscriber<GameMessages.MultiPhaseSetupDone> asyncSubMultiPhaseSetupDone,
            ISubscriber<GameMessages.HudMessage> subHudMessage,
            ISubscriber<CreatorMessages.MarkerMessage> subMarkerMessage,
            ISubscriber<TPFive.Game.Messages.BackToHome> subBackToHome,
            ISubscriber<TPFive.Game.Messages.SceneLoading> subSceneLoading,
            ISubscriber<TPFive.Game.Messages.SceneLoaded> subSceneLoaded,
            ISubscriber<TPFive.Game.Messages.SceneUnloading> subSceneUnloading,
            ISubscriber<TPFive.Game.Messages.SceneUnloaded> subSceneUnloaded,
            ISubscriber<ContentSubLevelLoaded> subContentSubLevelLoaded)
        {
            _loggerFactory = loggerFactory;
            Logger = GameLoggingUtility.CreateLogger<Bootstrap>(loggerFactory);

            _lifetimeScope = lifetimeScope;

            _settings = settings;
            _sceneSettings = sceneSettings;

            _serviceConfig = serviceConfig;
            _serviceResource = serviceResource;
            _serviceSceneFlow = serviceSceneFlow;

            _serviceObjectPool = serviceObjectPool;

            _serviceAudio = serviceAudio;
            _serviceHud = serviceHud;
            _serviceLocalization = serviceLocalization;

            _serviceActor = serviceActor;

            _serviceReferenceLocator = serviceReferenceLocator;

            _serviceInteractable = serviceInteractable;

            _serviceMessageRepo = serviceMessageRepo;

            _pubBootstrapJustStarted = pubBootstrapJustStarted;
            _pubBootstrapSetupDone = pubBootstrapSetupDone;

            _pubSceneLoading = pubSceneLoading;
            _pubSceneLoaded = pubSceneLoaded;
            _pubSceneUnloading = pubSceneUnloading;
            _pubSceneUnloaded = pubSceneUnloaded;

            _pubPostUnityMessage = pubPostUnityMessage;

            _asyncPubMultiPhaseSetupDone = asyncPubMultiPhaseSetupDone;

            _pubHudMessage = pubHudMessage;

            _pubMarkerMessage = pubMarkerMessage;
            _pubUnloadContentLevel = pubUnloadContentLevel;
            _pubContentSubLevelLoaded = pubContentSubLevelLoaded;
            _pubContentLevelFullyLoaded = pubContentLevelFullyLoaded;

            _asyncSubMultiPhaseSetupDone = asyncSubMultiPhaseSetupDone;

            _subHudMessage = subHudMessage;
            _subMarkerMessage = subMarkerMessage;
            _subBackToHome = subBackToHome;

            _subSceneLoading = subSceneLoading;
            _subSceneLoaded = subSceneLoaded;
            _subSceneUnloading = subSceneUnloading;
            _subSceneUnloaded = subSceneUnloaded;

            _subContentSubLevelLoaded = subContentSubLevelLoaded;
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

            _pubSceneLoading.Publish(
                new GameMessages.SceneLoading
                {
                    Category = _settings.Category,
                    Title = _settings.Title,
                    CategoryOrder = 3,
                    SubOrder = 0,
                });

            _pubBootstrapJustStarted.Publish(
                new GameMessages.BootstrapJustStarted
                {
                    Category = _settings.Category,
                });

            await SetupServices(cancellationToken);
            await SetupExtServicesPhase1(cancellationToken);
            await SetupExtServicesPhase2(cancellationToken);

            Game.FlutterUnityWidget.Utility.SendGeneralMessage(
                _pubPostUnityMessage,
                $"{nameof(Bootstrap)} - Finish {nameof(SetupBegin)}");
        }

        private async UniTask SetupServices(CancellationToken cancellationToken = default)
        {
            Logger.LogEditorDebug(
                "{Method}",
                nameof(SetupServices));

            var setupTasks = new List<UniTask>();

#if HAS_HIGH_PRIORITY_ENTRY
#else
            {
                if (_serviceConfig is IAsyncStartable asyncStartable)
                {
                    setupTasks.Add(asyncStartable.StartAsync(cancellationToken));
                }
            }
            {
                if (_serviceResource is IAsyncStartable asyncStartable)
                {
                    setupTasks.Add(asyncStartable.StartAsync(cancellationToken));
                }
            }
            {
                if (_serviceSceneFlow is IAsyncStartable asyncStartable)
                {
                    setupTasks.Add(asyncStartable.StartAsync(cancellationToken));
                }
            }
            {
                if (_serviceAudio is IAsyncStartable asyncStartable)
                {
                    setupTasks.Add(asyncStartable.StartAsync(cancellationToken));
                }
            }
            {
                if (_serviceHud is IAsyncStartable asyncStartable)
                {
                    setupTasks.Add(asyncStartable.StartAsync(cancellationToken));
                }
            }
            {
                if (_serviceLocalization is IAsyncStartable asyncStartable)
                {
                    setupTasks.Add(asyncStartable.StartAsync(cancellationToken));
                }
            }
#endif
            {
                if (_serviceMessageRepo is IAsyncStartable asyncStartable)
                {
                    setupTasks.Add(asyncStartable.StartAsync(cancellationToken));
                }
            }

            await UniTask.WhenAll(setupTasks);
        }

        /// <summary>
        /// Setup Pahse 1. Mainly setup for config
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <remarks>
        /// The order of extended addressables service provider has to
        /// be the first. This indicates something wrong, should fix it.
        /// </remarks>
        private async UniTask SetupExtServicesPhase1(
            CancellationToken cancellationToken = default)
        {
            Logger.LogEditorDebug(
                "{Method}",
                nameof(SetupExtServicesPhase1));

#if HAS_HIGH_PRIORITY_ENTRY
#else

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
                    .AddServiceProvider(10, serviceProvider);
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
                    .AddServiceProvider(10, serviceProvider);
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
                    .AddServiceProvider(20, serviceProvider);
                await serviceProvider.StartAsync(cancellationToken);
            }
#endif

#if HAS_TPFIVE_EXTENDED_LOCAL_CONFIG
            {
                var nullServiceProvider = _serviceConfig.NullServiceProvider;
                var serviceProvider = new ExtendedLocalConfig.ServiceProvider(
                    _loggerFactory,
                    nullServiceProvider,
                    _asyncPubMultiPhaseSetupDone);

                _createdServiceProviders.Add(serviceProvider);
                await ((Game.IServiceProviderManagement)_serviceConfig)
                    .AddServiceProvider(30, serviceProvider);
                await serviceProvider.StartAsync(cancellationToken);
            }
#endif

#endif // HAS_HIGH_PRIORITY_ENTRY
            {
                var serviceProvider = new SnapZoneExtended();
                _createdServiceProviders.Add(serviceProvider);
                await ((IServiceProviderManagement)_serviceInteractable)
                   .AddServiceProvider(Game.Interactable.Toolkit.Service.SnapZoneServiceProviderIndex, serviceProvider);
            }
        }

        /// <summary>
        /// Setup Phase2. Service providers should be loaded according to
        /// configs.
        /// </summary>
        /// <param name="cancellationToken"></param>
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

#if HAS_HIGH_PRIORITY_ENTRY
#else

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

#if HAS_TPFIVE_EXTENDED_ECM2
            {
                var nullServiceProvider = _serviceActor.NullServiceProvider;
                var serviceProvider = new ExtendedECM2.ServiceProvider(
                    _loggerFactory,
                    _serviceConfig,
                    nullServiceProvider);

                _createdServiceProviders.Add(serviceProvider);
                await serviceProvider.StartAsync(cancellationToken);
                await ((Game.IServiceProviderManagement)_serviceActor).AddServiceProvider(10, serviceProvider);
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
                    _settings.OverviewSettings.Hud);

                _createdServiceProviders.Add(serviceProvider);
                await serviceProvider.StartAsync(cancellationToken);
                await ((Game.IServiceProviderManagement)_serviceHud)
                    .AddServiceProvider(10, serviceProvider);
            }
#endif
            _serviceSceneFlow.SetupTopmostLifetimeScope(_lifetimeScope);

#endif // HAS_HIGH_PRIORITY_ENTRY
            {
                const int VsEventRepoServiceProviderIndex = (int)Game.ServiceProviderKind.Rank1ServiceProvider;

                // TODO: This levelBundleId should be from _settings
                var levelBundleId = string.Empty;
                var nullServiceProvider = _serviceMessageRepo.NullServiceProvider;
                var serviceProvider = new CreatorVSEventRepo.ServiceProvider(
                    _loggerFactory,
                    nullServiceProvider,
                    levelBundleId,
                    _pubMarkerMessage,
                    _pubContentLevelFullyLoaded,
                    _pubUnloadContentLevel,
                    _subHudMessage,
                    _subMarkerMessage,
                    _subBackToHome);

                _createdServiceProviders.Add(serviceProvider);
                await serviceProvider.StartAsync(cancellationToken);
                await ((Game.IServiceProviderManagement)_serviceMessageRepo).AddServiceProvider(VsEventRepoServiceProviderIndex, serviceProvider);
            }
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
                    Category = _settings.Category,
                    Success = success,
                });

            _pubSceneLoaded.Publish(
                new GameMessages.SceneLoaded
                {
                    Category = _settings.Category,
                    Title = _settings.Title,
                    CategoryOrder = 5,
                    SubOrder = 0,
                });

            _utcs.TrySetResult(success);

            Game.FlutterUnityWidget.Utility.SendGeneralMessage(
                _pubPostUnityMessage,
                $"{nameof(Bootstrap)} - Finish {nameof(SetupEnd)}");
        }

        private async Task HandleStartAsyncOperationCanceledException(
            System.OperationCanceledException e, CancellationToken cancellationToken = default)
        {
            Logger.LogWarning("{Exception}", e);
        }

        private async Task HandleStartAsyncException(
            System.Exception e, CancellationToken cancellationToken = default)
        {
            Logger.LogError("{Exception}", e);
        }

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
            }
        }

        private async ValueTask HandleDisposeAsync()
        {
            await Task.Yield();
        }
    }
}
