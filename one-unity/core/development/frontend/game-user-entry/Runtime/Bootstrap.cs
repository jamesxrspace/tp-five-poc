using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MessagePipe;
using Microsoft.Extensions.Logging;
using TPFive.Game.Logging;
using TPFive.OpenApi.GameServer;
using TPFive.SCG.AsyncStartable.Abstractions;
using TPFive.SCG.DisposePattern.Abstractions;
using TPFive.SCG.ServiceEco.Abstractions;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace TPFive.Game.User.Entry
{
    [InjectService(
    Setup = "true",
    DeclareSettings = "true",
    AddLifetimeScope = "true",
#pragma warning disable SA1118 // Parameter should not span multiple lines
    ServiceList = @"
TPFive.Game.User.Service,
TPFive.Game.RealtimeChat.Service")]
#pragma warning restore SA1118 // Parameter should not span multiple lines
    [AsyncStartable]
    [Dispose]
    [AsyncDispose]
    public sealed partial class Bootstrap
    {
        private readonly ILogger _logger;
        private readonly LifetimeScope _lifetimeScope;
        private readonly SceneFlow.IService _serviceSceneFlow;
        private readonly TPFive.Game.User.IService _serviceUser;
        private readonly RealtimeChat.IService _serviceRealtimeChat;
        private readonly IAvatarApi _avatarApi;
        private readonly App.Entry.Settings _appEntrySettings;
        private readonly TPFive.Game.User.Entry.Settings _userEntrySettings;
        private readonly IPublisher<Messages.BootstrapJustStarted> _pubBootstrapJustStarted;
        private readonly IPublisher<Messages.BootstrapSetupDone> _pubBootstrapSetupDone;
        private readonly IPublisher<TPFive.Game.FlutterUnityWidget.PostUnityMessage> _pubPostUnityMessage;

        [Inject]
        public Bootstrap(
            ILoggerFactory loggerFactory,
            LifetimeScope lifetimeScope,
            SceneFlow.IService sceneFlowService,
            RealtimeChat.IService realtimeChatService,
            IAvatarApi avatarApi,
            App.Entry.Settings appEntrySettings,
            Settings userEntrySettings,
            IPublisher<Messages.BootstrapJustStarted> pubBootstrapJustStarted,
            IPublisher<Messages.BootstrapSetupDone> pubBootstrapSetupDone,
            IPublisher<TPFive.Game.FlutterUnityWidget.PostUnityMessage> pubPostUnityMessage)
        {
            _logger = loggerFactory.CreateLogger<Bootstrap>();

            _lifetimeScope = lifetimeScope;
            _serviceSceneFlow = sceneFlowService;
            _serviceRealtimeChat = realtimeChatService;
            _avatarApi = avatarApi;
            _appEntrySettings = appEntrySettings;
            _userEntrySettings = userEntrySettings;
            _pubBootstrapJustStarted = pubBootstrapJustStarted;
            _pubBootstrapSetupDone = pubBootstrapSetupDone;
            _pubPostUnityMessage = pubPostUnityMessage;
        }

        // AsyncStartable
        private async UniTask SetupBegin(CancellationToken cancellationToken = default)
        {
            Game.FlutterUnityWidget.Utility.SendGeneralMessage(
                _pubPostUnityMessage,
                $"{nameof(Bootstrap)} - Begin {nameof(SetupBegin)}");

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("SetupBegin");
            }

            _pubBootstrapJustStarted.Publish(
                new Messages.BootstrapJustStarted
                {
                    Category = _userEntrySettings.Category,
                });

            await SetupServices(cancellationToken);

            Game.FlutterUnityWidget.Utility.SendGeneralMessage(
                _pubPostUnityMessage,
                $"{nameof(Bootstrap)} - Finish {nameof(SetupBegin)}");
        }

        private async UniTask SetupEnd(bool success, CancellationToken cancellationToken = default)
        {
            Game.FlutterUnityWidget.Utility.SendGeneralMessage(
                _pubPostUnityMessage,
                $"{nameof(Bootstrap)} - Begin {nameof(SetupEnd)}");

            _logger.LogEditorDebug(
                "{Method}",
                nameof(SetupEnd));

            _pubBootstrapSetupDone.Publish(
                new Messages.BootstrapSetupDone
                {
                    Category = _userEntrySettings.Category,
                    Success = success,
                });

            try
            {
                if (!TryGetSceneProperty(_userEntrySettings.CurrentScene, out var currentProperty))
                {
                    LogMissingSceneError(_userEntrySettings.CurrentScene);
                    return;
                }

                var response = await _avatarApi.GetMyselfCurrentAvatarMetadataAsync();
                var sceneTitle = response.IsSuccess ? _userEntrySettings.NextScene : _userEntrySettings.EditorScene;

                if (!TryGetSceneProperty(sceneTitle, out var nextProperty))
                {
                    LogMissingSceneError(sceneTitle);
                    return;
                }

                await _serviceSceneFlow.LoadSceneAsync(
                    nextProperty.addressableKey,
                    LoadSceneMode.Additive,
                    nextProperty.categoryOrder,
                    nextProperty.subOrder,
                    _lifetimeScope);
            }
            catch (Exception e)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError(e, "Failed to go to next scene");
                }
            }

            Game.FlutterUnityWidget.Utility.SendGeneralMessage(
                _pubPostUnityMessage,
                $"{nameof(Bootstrap)} - Finish {nameof(SetupEnd)}");
        }

        private async Task HandleStartAsyncOperationCanceledException(
            System.OperationCanceledException e, CancellationToken cancellationToken = default)
        {
            _logger.LogWarning("{Exception}", e);

            await Task.CompletedTask;
        }

        private async Task HandleStartAsyncException(
            System.Exception e, CancellationToken cancellationToken = default)
        {
            _logger.LogError("{Exception}", e);

            await Task.CompletedTask;
        }

        private bool TryGetSceneProperty(string title, out SceneProperty property)
        {
            var index = _appEntrySettings.ScenePropertyList.FindIndex(x =>
            {
                return x.title.Equals(title, StringComparison.Ordinal);
            });
            property = index != -1 ? _appEntrySettings.ScenePropertyList[index] : null;
            return property != null;
        }

        private void LogMissingSceneError(string title)
        {
            if (_logger.Equals(LogLevel.Error))
            {
                _logger.LogError("Failed to find {0} scene property", title);
            }
        }

        // Dispose
        private void HandleDispose(bool disposing)
        {
            _logger.LogEditorDebug(
                "{Method} - disposing: {Disposing}",
                nameof(HandleDispose),
                disposing);

            if (disposing)
            {
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
