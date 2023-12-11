using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using Microsoft.Extensions.Logging;
using TPFive.Game.FlutterUnityWidget;
using VContainer;
using VContainer.Unity;

namespace TPFive.Game.AvatarEdit.Entry
{
    internal sealed class Bootstrap : IAsyncStartable
    {
        private readonly ILogger _logger;
        private readonly IAvatarEditController _avatarEditorController;
        private readonly IPublisher<PostUnityMessage> _pubUnityMessage;

        [Inject]
        public Bootstrap(
            ILoggerFactory loggerFactory,
            IAvatarEditController avatarEditorController,
            IPublisher<PostUnityMessage> pubUnityMessage)
        {
            _logger = loggerFactory.CreateLogger<Bootstrap>();
            _avatarEditorController = avatarEditorController;
            _pubUnityMessage = pubUnityMessage;
        }

        async UniTask IAsyncStartable.StartAsync(CancellationToken cancellation)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("StartAsync");
            }

            _avatarEditorController.Initialize();
            _avatarEditorController.ShowSelectPresetWindow().Forget();

            _pubUnityMessage.Publish(new PostUnityMessage()
            {
                UnityMessage = new Model.UnityMessage()
                {
                    Type = Model.UnityMessageTypeEnum.ToAvatarEdit,
                    Data = string.Empty,
                },
            });
        }
    }
}
