using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Loxodon.Framework.Views;
using MessagePipe;
using Microsoft.Extensions.Logging;
using TPFive.Game;
using TPFive.Game.App.Entry;
using TPFive.Game.Extensions;
using TPFive.Game.FlutterUnityWidget;
using TPFive.Game.Home.Entry;
using TPFive.Game.Record;
using TPFive.Game.SceneFlow;
using TPFive.Game.UI;
using TPFive.Model;
using TPFive.OpenApi.GameServer;
using TPFive.Room;
using TPFive.SCG.DisposePattern.Abstractions;
using UniRx;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Home.Entry
{
    [Dispose]
    internal partial class Bootstrap : IAsyncStartable
    {
        private readonly CompositeDisposable compositeDisposable = new CompositeDisposable();

        private readonly ILogger log;
        private readonly ILoggerFactory loggerFactory;
        private readonly LifetimeScope lifetimeScope;
        private readonly Game.SceneFlow.IService sceneFlowService;
        private readonly Settings sceneSettings;
        private readonly Game.Space.IService spaceService;
        private readonly Game.UI.IService gameUIService;
        private readonly Game.Config.IService configService;
        private readonly Game.Resource.IService resourceService;
        private readonly IPublisher<ChangeScene> pubSceneLoading;
        private readonly Settings appEntrySettings;
        private readonly ISubscriber<FlutterMessage> subFlutterMessage;
        private readonly IPublisher<PostUnityMessage> pubUnityMessage;
        private readonly ISpaceApi spaceApi;
        private readonly IOpenRoomCmd openRoomCmd;
        private readonly string homeEntryTitle = "Home";
        private readonly ReelEntryTemplateSetting reelEntryTemplateSetting;

        [Inject]
        public Bootstrap(
            ILoggerFactory loggerFactory,
            LifetimeScope lifetimeScope,
            Game.SceneFlow.IService sceneFlowService,
            Settings sceneSettings,
            Game.Space.IService spaceService,
            Game.UI.IService gameUIService,
            Game.Config.IService configService,
            Game.Resource.IService resourceService,
            IPublisher<ChangeScene> pubSceneLoading,
            Settings appEntrySettings,
            ReelEntryTemplateSetting reelEntryTemplateSetting,
            ISubscriber<FlutterMessage> subFlutterMessage,
            IPublisher<PostUnityMessage> pubUnityMessage,
            ISpaceApi spaceApi,
            IOpenRoomCmd openRoomCmd)
        {
            this.log = Game.Logging.Utility.CreateLogger<Bootstrap>(loggerFactory);
            this.loggerFactory = loggerFactory;
            this.lifetimeScope = lifetimeScope;
            this.sceneFlowService = sceneFlowService;
            this.sceneSettings = sceneSettings;
            this.spaceService = spaceService;
            this.gameUIService = gameUIService;
            this.configService = configService;
            this.resourceService = resourceService;
            this.pubSceneLoading = pubSceneLoading;
            this.appEntrySettings = appEntrySettings;
            this.subFlutterMessage = subFlutterMessage;
            this.pubUnityMessage = pubUnityMessage;
            this.spaceApi = spaceApi;
            this.openRoomCmd = openRoomCmd;
            this.reelEntryTemplateSetting = reelEntryTemplateSetting;

            this.subFlutterMessage
                .Subscribe(OnFlutterRequestToReel, arg => arg.Type == Model.FlutterMessageTypeEnum.RequestToReelPage)
                .AddTo(compositeDisposable);
            this.subFlutterMessage
                .Subscribe(OnFlutterRequestToSpace, arg => arg.Type == FlutterMessageTypeEnum.RequestToSpace)
                .AddTo(compositeDisposable);
        }

        async UniTask IAsyncStartable.StartAsync(CancellationToken cancellation)
        {
            if (sceneSettings == null)
            {
                throw new NullReferenceException("scene setting is null.");
            }

            if (!sceneSettings.TryGetSceneProperty(homeEntryTitle, out var homeEntryProperty))
            {
                log.LogError("Can't retrieve {title} scene property", homeEntryTitle);
            }

            var scene = await LoadSceneAsync(homeEntryProperty, cancellation);

            if (!scene.IsValid())
            {
                await ShowRetryLoadingDialog();
                throw new InvalidOperationException($"Load scene {homeEntryProperty.addressableKey} failed.");
            }

            bool success = SceneManager.SetActiveScene(scene);
            log.LogInformation($"Active home({scene.name}) {(success ? "successful" : "failure")}.");

            var spaceServiceProvider = new TPFive.Extended.Space.ServiceProvider(spaceApi, loggerFactory);
            await ((IServiceProviderManagement)spaceService).AddServiceProvider(
                (int)ServiceProviderKind.Rank1ServiceProvider,
                spaceServiceProvider);

            if (GameApp.IsFlutter)
            {
                pubUnityMessage.Publish(new PostUnityMessage()
                {
                    UnityMessage = new Model.UnityMessage()
                    {
                        Type = Model.UnityMessageTypeEnum.SwitchedToSocialLobbyPage,
                        Data = "Success",
                        SessionId = string.Empty,
                    },
                });
            }

            ShowMainMenuWindow(cancellation).Forget();
        }

        private async UniTaskVoid ShowMainMenuWindow(CancellationToken cancellation)
        {
            await gameUIService.ShowWindow<MainMenuWindow>();
        }

        private void OnFlutterRequestToReel(Model.FlutterMessage flutterMessage)
        {
            log.LogDebug("{Method}: flutter message: {Message}", nameof(OnFlutterRequestToReel), flutterMessage.Data);

            var message = ReelConfig.FromJson(flutterMessage.Data);
            if (message.Entry == null)
            {
                log.LogError(
                    "{Method}: Doesn't have entry value",
                    nameof(OnFlutterRequestToReel));

                return;
            }

            if (!message.Entry.TryConvertTo<ReelSceneEntryParameter.EntryType>(out var entryType))
            {
                log.LogError(
                    "{Method}: Invalid entry value: {Entry}",
                    nameof(OnFlutterRequestToReel),
                    message.Entry);

                return;
            }

            var entryParam = new ReelSceneEntryParameter()
            {
                Entry = entryType,
                SceneDesc = reelEntryTemplateSetting.GetReelEntryTemplate(message.SceneName).ReelSceneDesc,
            };

            configService.SetSystemObjectValue(
                Game.Config.Constants.RuntimeLocalProviderKind,
                nameof(ReelSceneEntryParameter),
                entryParam);

            if (!appEntrySettings.TryGetSceneProperty("HomeEntry", out var fromEntryProperty) ||
                !appEntrySettings.TryGetSceneProperty("RecordEntry", out var toEntryProperty))
            {
                log.LogError(
                    "{Method}: Can't retrieve 'home' or 'record' scene property",
                    nameof(OnFlutterRequestToReel));
                return;
            }

            pubSceneLoading.Publish(new Game.SceneFlow.ChangeScene()
            {
                FromCategory = fromEntryProperty.category,
                FromTitle = fromEntryProperty.addressableKey,
                FromCategoryOrder = fromEntryProperty.categoryOrder,
                FromSubOrder = fromEntryProperty.subOrder,
                ToCategory = toEntryProperty.category,
                ToTitle = toEntryProperty.addressableKey,
                ToCategoryOrder = toEntryProperty.categoryOrder,
                ToSubOrder = toEntryProperty.subOrder,
                LifetimeScope = lifetimeScope,
            });
        }

        private void OnFlutterRequestToSpace(FlutterMessage flutterMessage)
        {
            log.LogDebug("{Method}: flutter message: {Message}", nameof(OnFlutterRequestToSpace), flutterMessage.Data);

            var message = RoomConfig.FromJson(flutterMessage.Data);
            if (!appEntrySettings.TryGetSceneProperty("HomeEntry", out var fromEntryProperty) ||
                !appEntrySettings.TryGetSceneProperty("RoomEntry", out var toEntryProperty))
            {
                log.LogError(
                    "{Method}: Can't retrieve 'home' or 'room' scene property",
                    nameof(OnFlutterRequestToReel));
                return;
            }

            openRoomCmd.Set(message.SpaceId, message.SceneKey);
            pubSceneLoading.Publish(new Game.SceneFlow.ChangeScene()
            {
                FromCategory = fromEntryProperty.category,
                FromTitle = fromEntryProperty.addressableKey,
                FromCategoryOrder = fromEntryProperty.categoryOrder,
                FromSubOrder = fromEntryProperty.subOrder,
                ToCategory = toEntryProperty.category,
                ToTitle = toEntryProperty.addressableKey,
                ToCategoryOrder = toEntryProperty.categoryOrder,
                ToSubOrder = toEntryProperty.subOrder,
                LifetimeScope = lifetimeScope,
            });
        }

        private void HandleDispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                compositeDisposable?.Dispose();
            }

            _disposed = true;
        }

        private async UniTask<Scene> LoadSceneAsync(SceneProperty sceneProperty, CancellationToken cancellation)
        {
            if (sceneProperty.isRemote)
            {
                await resourceService.LoadBundledDataAsync(sceneProperty.addressableKey, cancellation);

                return await sceneFlowService.LoadSceneAsync($"{sceneProperty.addressableKey}.asset", LoadSceneMode.Additive, sceneProperty.categoryOrder, sceneProperty.subOrder, lifetimeScope, true, cancellation);
            }

            return await sceneFlowService.LoadSceneAsync(
                sceneProperty.addressableKey,
                LoadSceneMode.Additive,
                sceneProperty.categoryOrder,
                sceneProperty.subOrder,
                lifetimeScope,
                false,
                cancellation);
        }

        private async UniTask ShowRetryLoadingDialog()
        {
            Bundle bundle = new Bundle();
            bundle.Put("description", "Download home scene failed." as object);
            bundle.Put("confirmText", "Retry" as object);
            var window = await gameUIService.ShowWindow<OneBtnDialog>(bundle);
            window.ClickAction = () =>
            {
                ((IAsyncStartable)this).StartAsync(CancellationToken.None).Forget();
            };
        }
    }
}
