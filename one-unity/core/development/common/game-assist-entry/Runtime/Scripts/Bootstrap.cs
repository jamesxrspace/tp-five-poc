using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MessagePipe;
using Microsoft.Extensions.Logging;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TPFive.Game.Assist.Entry
{
    using TPFive.Game.Logging;
    using TPFive.SCG.AsyncStartable.Abstractions;
    using TPFive.SCG.DisposePattern.Abstractions;
    using TPFive.SCG.ServiceEco.Abstractions;

    using GameConfig = TPFive.Game.Config;
    using GameLoggingUtility = TPFive.Game.Logging.Utility;
    using GameMessages = TPFive.Game.Messages;
    using GameResource = TPFive.Game.Resource;
    using GameSceneFlow = TPFive.Game.SceneFlow;
    using GameAssist = TPFive.Game.Assist;

#if HAS_TPFIVE_EXTENDED_QUANTUM_CONSOLE && HAS_HIGH_PRIORITY_ENTRY
    using ExtendedQuantumConsole = TPFive.Extended.QuantumConsole;
#endif

    [InjectService(
        Setup = "true",
        DeclareSettings = "true",
        AddLifetimeScope = "true",
        ServiceList = @"
TPFive.Game.Assist.Service")]
    [AsyncStartable(ExceptionList = "System.OperationCanceledException, System.Exception")]
    [Dispose]
    [AsyncDispose]
    public sealed partial class Bootstrap
    {
        private readonly CompositeDisposable _compositeDisposable = new ();

        private readonly Microsoft.Extensions.Logging.ILoggerFactory _loggerFactory;

        private readonly LifetimeScope _lifetimeScope;

        private readonly Settings _settings;
        private readonly LifetimeScope.SceneSettings _sceneSettings;

        private readonly GameConfig.IService _serviceConfig;
        private readonly GameSceneFlow.IService _serviceSceneFlow;
        private readonly GameAssist.IService _serviceAssist;

        private readonly IPublisher<GameMessages.BootstrapJustStarted> _pubBootstrapJustStarted;
        private readonly IPublisher<GameMessages.BootstrapSetupDone> _pubBootstrapSetupDone;

        private readonly IPublisher<TPFive.Game.Messages.SceneLoading> _pubSceneLoading;
        private readonly IPublisher<TPFive.Game.Messages.SceneLoaded> _pubSceneLoaded;
        private readonly IPublisher<TPFive.Game.Messages.SceneUnloading> _pubSceneUnloading;
        private readonly IPublisher<TPFive.Game.Messages.SceneUnloaded> _pubSceneUnloaded;

        private readonly IPublisher<TPFive.Game.FlutterUnityWidget.PostUnityMessage> _pubPostUnityMessage;

        private readonly IPublisher<Game.SceneFlow.ChangeScene> _pubChangeScene;

        private readonly IPublisher<TPFive.Game.Messages.BackToHome> _pubBackToHome;

        private readonly IPublisher<TPFive.Game.Messages.AssistMode> _pubAssistMode;

        private readonly ISubscriber<TPFive.Game.Messages.SceneLoading> _subSceneLoading;
        private readonly ISubscriber<TPFive.Game.Messages.SceneLoaded> _subSceneLoaded;
        private readonly ISubscriber<TPFive.Game.Messages.SceneUnloading> _subSceneUnloading;
        private readonly ISubscriber<TPFive.Game.Messages.SceneUnloaded> _subSceneUnloaded;

        private readonly ISubscriber<TPFive.Game.Messages.AssistMode> _subAssistMode;

        // This corrects the removal of service provider from service, but will only fix the part here
        // as assist scene keeps popping, pushing.
        // Fix to others will be done later.
        private readonly Dictionary<Game.IServiceProviderManagement, List<(int, Game.IServiceProvider)>> _serviceProviderManagementTable =
            new ();

        // InjectService
        [Inject]
        public Bootstrap(
            Microsoft.Extensions.Logging.ILoggerFactory loggerFactory,
            LifetimeScope lifetimeScope,
            Settings settings,
            LifetimeScope.SceneSettings sceneSettings,
            GameConfig.IService serviceConfig,
            GameSceneFlow.IService serviceSceneFlow,
            GameAssist.IService serviceAssist,
            IPublisher<GameMessages.BootstrapJustStarted> pubBootstrapJustStarted,
            IPublisher<GameMessages.BootstrapSetupDone> pubBootstrapSetupDone,
            IPublisher<TPFive.Game.Messages.SceneLoading> pubSceneLoading,
            IPublisher<TPFive.Game.Messages.SceneLoaded> pubSceneLoaded,
            IPublisher<TPFive.Game.Messages.SceneUnloading> pubSceneUnloading,
            IPublisher<TPFive.Game.Messages.SceneUnloaded> pubSceneUnloaded,
            IPublisher<TPFive.Game.FlutterUnityWidget.PostUnityMessage> pubPostUnityMessage,
            IPublisher<Game.SceneFlow.ChangeScene> pubChangeScene,
            IPublisher<TPFive.Game.Messages.BackToHome> pubBackToHome,
            IPublisher<TPFive.Game.Messages.AssistMode> pubAssistMode,
            ISubscriber<TPFive.Game.Messages.SceneLoading> subSceneLoading,
            ISubscriber<TPFive.Game.Messages.SceneLoaded> subSceneLoaded,
            ISubscriber<TPFive.Game.Messages.SceneUnloading> subSceneUnloading,
            ISubscriber<TPFive.Game.Messages.SceneUnloaded> subSceneUnloaded,
            ISubscriber<TPFive.Game.Messages.AssistMode> subAssistMode)
        {
            _loggerFactory = loggerFactory;
            Logger = GameLoggingUtility.CreateLogger<Bootstrap>(loggerFactory);

            _lifetimeScope = lifetimeScope;

            _settings = settings;
            _sceneSettings = sceneSettings;

            _serviceConfig = serviceConfig;
            _serviceSceneFlow = serviceSceneFlow;
            _serviceAssist = serviceAssist;

            _pubBootstrapJustStarted = pubBootstrapJustStarted;
            _pubBootstrapSetupDone = pubBootstrapSetupDone;

            _pubSceneLoading = pubSceneLoading;
            _pubSceneLoaded = pubSceneLoaded;
            _pubSceneUnloading = pubSceneUnloading;
            _pubSceneUnloaded = pubSceneUnloaded;

            _pubPostUnityMessage = pubPostUnityMessage;

            _pubChangeScene = pubChangeScene;
            _pubBackToHome = pubBackToHome;
            _pubAssistMode = pubAssistMode;

            _subSceneLoading = subSceneLoading;
            _subSceneLoaded = subSceneLoaded;
            _subSceneUnloading = subSceneUnloading;
            _subSceneUnloaded = subSceneUnloaded;

            _subAssistMode = subAssistMode;

            _funcOperationCanceledException = HandleStartAsyncOperationCanceledException;
            _funcException = HandleStartAsyncException;
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

            _pubSceneLoading.Publish(new GameMessages.SceneLoading
            {
                Category = _settings.category,
                Title = _settings.title,
                CategoryOrder = 5,
                SubOrder = 0,
            });

            _pubBootstrapJustStarted.Publish(new GameMessages.BootstrapJustStarted
            {
                Category = _settings.category,
            });

            await SetupServices(cancellationToken);
            await SetupExtServices(cancellationToken);

            Game.FlutterUnityWidget.Utility.SendGeneralMessage(
                _pubPostUnityMessage,
                $"{nameof(Bootstrap)} - Finish {nameof(SetupBegin)}");
        }

        private async Task SetupMessageHandling(CancellationToken cancellationToken = default)
        {
            _subSceneLoading
                .Subscribe(async x =>
                {
                })
                .AddTo(_compositeDisposable);

            _subSceneLoaded
                .Subscribe(async x =>
                {
                    if (string.CompareOrdinal(x.Category, _settings.category) == 0)
                    {
                        // Receive message from self
                    }
                    else
                    {
                        Logger.LogEditorDebug(
                            "Receive {Message} from {Category}",
                            nameof(GameMessages.SceneLoaded),
                            x.Category);
                    }
                })
                .AddTo(_compositeDisposable);

            _subSceneUnloading
                .Subscribe(async x =>
                {
                })
                .AddTo(_compositeDisposable);

            _subSceneUnloaded
                .Subscribe(async x =>
                {
                })
                .AddTo(_compositeDisposable);
        }

        private async UniTask SetupExtServices(CancellationToken cancellationToken = default)
        {
            Logger.LogEditorDebug(
                "{Method}",
                nameof(SetupExtServices));

#if HAS_TPFIVE_EXTENDED_QUANTUM_CONSOLE && HAS_HIGH_PRIORITY_ENTRY
            {
                const int quantumConsoleServiceProviderIndex = (int)Game.ServiceProviderKind.Rank1ServiceProvider;
                var nullServiceProvider = _serviceAssist.NullServiceProvider;
                var serviceProvider = new ExtendedQuantumConsole.ServiceProvider(
                    _loggerFactory,
                    _serviceConfig,
                    _serviceSceneFlow,
                    nullServiceProvider,
                    _sceneSettings.quantumConsole,
                    _pubChangeScene,
                    _pubBackToHome,
                    _subAssistMode);

                if (_serviceAssist is Game.IServiceProviderManagement spm)
                {
                    var result = _serviceProviderManagementTable.TryGetValue(spm, out var csps);
                    if (!result || csps == null)
                    {
                        csps = new List<(int, Game.IServiceProvider)>();
                    }

                    csps.Add((quantumConsoleServiceProviderIndex, serviceProvider));
                    _serviceProviderManagementTable[spm] = csps;

                    await spm.AddServiceProvider(quantumConsoleServiceProviderIndex, serviceProvider);
                    await serviceProvider.StartAsync(cancellationToken);
                }
            }
#endif
        }

        private async UniTask SetupEnd(bool success, CancellationToken cancellationToken = default)
        {
            Game.FlutterUnityWidget.Utility.SendGeneralMessage(
                _pubPostUnityMessage,
                $"{nameof(Bootstrap)} - Begin {nameof(SetupEnd)}");

            Logger.LogEditorDebug(
                "{Method}",
                nameof(SetupEnd));

            _pubBootstrapSetupDone.Publish(new GameMessages.BootstrapSetupDone
            {
                Category = _settings.category,
                Success = success,
            });

            _pubSceneLoaded.Publish(new GameMessages.SceneLoaded
            {
                Category = _settings.category,
                Title = _settings.title,
                CategoryOrder = 5,
                SubOrder = 0,
            });

            // Since the original UI is in scene, just move them back to here but actual
            // message is handled in service provider.
            Observable
                .FromEvent(
                    h => _sceneSettings.quantumConsole.OnActivate += h,
                    h => _sceneSettings.quantumConsole.OnActivate -= h)
                .Subscribe(_ =>
                {
                    if (_sceneSettings.buttonGameObject != null)
                    {
                        _sceneSettings.buttonGameObject.SetActive(false);
                    }
                })
                .AddTo(_compositeDisposable);

            Observable
                .FromEvent(
                    h => _sceneSettings.quantumConsole.OnDeactivate += h,
                    h => _sceneSettings.quantumConsole.OnDeactivate -= h)
                .Subscribe(_ =>
                {
                    if (_sceneSettings.buttonGameObject != null)
                    {
                        _sceneSettings.buttonGameObject.SetActive(true);
                    }
                })
                .AddTo(_compositeDisposable);

            if (_sceneSettings.buttonGameObject != null)
            {
                if (_sceneSettings.buttonGameObject.TryGetComponent(out UnityEngine.UI.Button button))
                {
                    button.onClick.AsObservable()
                        .Subscribe(_ =>
                        {
                            _sceneSettings.quantumConsole.Toggle();
                            _pubAssistMode.Publish(new GameMessages.AssistMode
                            {
                                On = _sceneSettings.quantumConsole.IsActive,
                                AssistModeGameObject = _sceneSettings.buttonGameObject,
                            });
                        })
                        .AddTo(_compositeDisposable);
                }
            }

            await UniTask.CompletedTask;

            Game.FlutterUnityWidget.Utility.SendGeneralMessage(
                _pubPostUnityMessage,
                $"{nameof(Bootstrap)} - Finish {nameof(SetupEnd)}");
        }

        private async Task HandleStartAsyncOperationCanceledException(
            System.OperationCanceledException e,
            CancellationToken cancellationToken = default)
        {
            Logger.LogWarning("{Exception}", e);

            await UniTask.CompletedTask;
        }

        private async Task HandleStartAsyncException(
            System.Exception e,
            CancellationToken cancellationToken = default)
        {
            Logger.LogError("{Exception}", e);

            await UniTask.CompletedTask;
        }

        // Dispose
        private void HandleDispose(bool disposing)
        {
            Logger.LogEditorDebug(
                "{Method} - disposing: {Disposing}",
                nameof(HandleDispose),
                disposing);

            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _compositeDisposable?.Dispose();

                _disposed = true;
            }
        }

        // AsyncDispose
        private async ValueTask HandleDisposeAsync()
        {
            foreach (var (key, value) in _serviceProviderManagementTable)
            {
                foreach (var (index, serviceProvider) in value)
                {
                    await key.RemoveServiceProvider(index);
                    if (serviceProvider is System.IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
            }
        }
    }
}
