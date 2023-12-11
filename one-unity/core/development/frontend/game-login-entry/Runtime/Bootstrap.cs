using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Loxodon.Framework.Binding.Paths;
using MessagePipe;
using Microsoft.Extensions.Logging;
using TPFive.Game.FlutterUnityWidget;
using TPFive.Game.GameServer;
using TPFive.Game.Login.Entry.Views;
using TPFive.Model;
using VContainer;
using VContainer.Unity;
using AppEntrySettings = TPFive.Game.App.Entry.Settings;
using AppLifetimeScope = TPFive.Game.App.Entry.LifetimeScope;

namespace TPFive.Game.Login.Entry
{
    public class Bootstrap : IAsyncStartable
    {
        private readonly ILogger logger;
        private readonly UI.IService uiService;
        private readonly IGameServerAuthTokenProvider gameServerAuthTokenProvider;
        private readonly AppEntrySettings appEntrySettings;
        private readonly Settings settings;
        private readonly MessagePipe.IPublisher<SceneFlow.ChangeScene> pubSceneLoading;
        private readonly AppLifetimeScope lifetimeScope;

        private readonly MessagePipe.IPublisher<PostUnityMessage> pubUnityMessage;
        private readonly IService loginService;

        [Inject]
        public Bootstrap(
            ILoggerFactory loggerFactory,
            UI.IService uiService,
            IGameServerAuthTokenProvider gameServerAuthTokenProvider,
            AppEntrySettings appEntrySettings,
            Settings settings,
            MessagePipe.IPublisher<SceneFlow.ChangeScene> pubSceneLoading,
            AppLifetimeScope lifetimeScope,
            IService loginService,
            MessagePipe.IPublisher<PostUnityMessage> pubUnityMessage)
        {
            logger = loggerFactory.CreateLogger<Bootstrap>();
            this.uiService = uiService;
            this.appEntrySettings = appEntrySettings;
            this.settings = settings;
            this.gameServerAuthTokenProvider = gameServerAuthTokenProvider;
            this.pubSceneLoading = pubSceneLoading;
            this.pubUnityMessage = pubUnityMessage;
            this.lifetimeScope = lifetimeScope;

            this.loginService = loginService;
            this.pubUnityMessage = pubUnityMessage;
        }

        private int[] TokenRefreshIntervals => settings.TokenRefreshIntervals;

        async UniTask IAsyncStartable.StartAsync(CancellationToken cancellation)
        {
            Game.FlutterUnityWidget.Utility.SendGeneralMessage(
                pubUnityMessage,
                $"{nameof(Bootstrap)} - Begin {nameof(IAsyncStartable.StartAsync)}");

            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug("StartAsync");
            }

            if (GameApp.IsFlutter)
            {
                pubUnityMessage.Publish(new PostUnityMessage()
                {
                    UnityMessage = new UnityMessage()
                    {
                        Type = UnityMessageTypeEnum.RequestAccessToken,
                        Data = string.Empty,
                        SessionId = string.Empty,
                    },
                    Ack = (token) =>
                    {
                        logger.LogDebug($"AuthToken = {token.FlutterMessage.Data}");
                        loginService.SetAccessToken(token.FlutterMessage.Data);
                        GoToNextScene();
                    },
                    timeout = 300,
                });
                /*
                string authToken = null;
                for (var i = 0; i < TokenRefreshIntervals.Length; ++i)
                {
                    if (TokenRefreshIntervals[i] > 0)
                    {
                        await UniTask.Delay(TimeSpan.FromSeconds(TokenRefreshIntervals[i]));
                    }

                    authToken = await gameServerAuthTokenProvider.RefreshTokenAsync(cancellation);
                    if (authToken != null)
                    {
                        logger.LogDebug($"AuthToken = {authToken}");
                        break;
                    }
                }

                if (authToken == null)
                {
                    logger.LogError("Refresh token fail");
                    return;
                }

                GoToNextScene();
                */
            }
            else
            {
                var (isCanceled, window) = await uiService.ShowWindow<LoginWindow>()
                    .AttachExternalCancellation(cancellation)
                    .SuppressCancellationThrow();
                if (isCanceled && window != null)
                {
                    _ = window.Dismiss(true);
                }
            }

            Game.FlutterUnityWidget.Utility.SendGeneralMessage(
                pubUnityMessage,
                $"{nameof(Bootstrap)} - Finish {nameof(IAsyncStartable.StartAsync)}");
        }

        private void GoToNextScene()
        {
            var errMsg = "Failed to find {0} scene property";
            if (!appEntrySettings.TryGetSceneProperty(settings.CurrentScene, out var currentProperty))
            {
                logger.LogError(errMsg, settings.CurrentScene);
                return;
            }

            if (!appEntrySettings.TryGetSceneProperty(settings.NextScene, out var nextProperty))
            {
                logger.LogError(errMsg, settings.NextScene);
                return;
            }

            pubSceneLoading.Publish(new Game.SceneFlow.ChangeScene()
            {
                FromCategory = currentProperty.category,
                FromTitle = currentProperty.addressableKey,
                FromCategoryOrder = currentProperty.categoryOrder,
                FromSubOrder = currentProperty.subOrder,
                ToCategory = nextProperty.category,
                ToTitle = nextProperty.addressableKey,
                ToCategoryOrder = nextProperty.categoryOrder,
                ToSubOrder = nextProperty.subOrder,
                LifetimeScope = lifetimeScope,
            });
        }
    }
}
