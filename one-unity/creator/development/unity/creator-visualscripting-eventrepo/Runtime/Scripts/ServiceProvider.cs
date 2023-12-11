using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MessagePipe;
using Microsoft.Extensions.Logging;
using UniRx;
using UnityEngine.Assertions;
using VContainer.Unity;

namespace TPFive.Creator.VisualScripting.EventRepo
{
    using TPFive.Game.Logging;

    using TPFive.SCG.AsyncStartable.Abstractions;
    using TPFive.SCG.DisposePattern.Abstractions;

    using CrossBridge = TPFive.Cross.Bridge;

    using GameMessages = TPFive.Game.Messages;
    using CreatorMessages = TPFive.Creator.Messages;

    using GameLoggingUtility = TPFive.Game.Logging.Utility;

    using CreatorMessageRepo = TPFive.Creator.MessageRepo;

    [Dispose]
    [AsyncDispose]
    public sealed partial class ServiceProvider :
        IAsyncStartable,
        CreatorMessageRepo.IServiceProvider
    {
        private readonly CompositeDisposable _compositeDisposable = new ();
        private readonly CreatorMessageRepo.IServiceProvider _nullServiceProvider;

        private readonly IPublisher<CreatorMessages.MarkerMessage> _pubMarkerMessage;
        private readonly IPublisher<Game.Messages.ContentLevelFullyLoaded> _pubContentLevelFullyLoaded;
        private readonly IPublisher<Game.Messages.UnloadContentLevel> _pubUnloadContentLevel;

        private readonly ISubscriber<GameMessages.HudMessage> _subHudMessage;
        private readonly ISubscriber<CreatorMessages.MarkerMessage> _subMarkerMessage;
        private readonly ISubscriber<GameMessages.BackToHome> _subBackToHome;

        private readonly string _levelBundleId;

        private CancellationTokenSource _cancellationTokenSource = default;

        public ServiceProvider(
            ILoggerFactory loggerFactory,
            CreatorMessageRepo.IServiceProvider nullServiceProvider,
            string levelBundleId,
            // Publishers
            IPublisher<CreatorMessages.MarkerMessage> pubMarkerMessage,
            IPublisher<Game.Messages.ContentLevelFullyLoaded> pubContentLevelFullyLoaded,
            IPublisher<Game.Messages.UnloadContentLevel> pubUnloadContentLevel,
            // Subscribers
            ISubscriber<GameMessages.HudMessage> subHudMessage,
            ISubscriber<CreatorMessages.MarkerMessage> subMarkerMessage,
            ISubscriber<GameMessages.BackToHome> subBackToHome)
        {
            Assert.IsNotNull(loggerFactory);

            Logger = GameLoggingUtility.CreateLogger<ServiceProvider>(loggerFactory);

            _nullServiceProvider = nullServiceProvider;

            _levelBundleId = levelBundleId;

            _pubMarkerMessage = pubMarkerMessage;
            _pubContentLevelFullyLoaded = pubContentLevelFullyLoaded;
            _pubUnloadContentLevel = pubUnloadContentLevel;

            _subHudMessage = subHudMessage;
            _subMarkerMessage = subMarkerMessage;
            _subBackToHome = subBackToHome;
        }

        private Microsoft.Extensions.Logging.ILogger Logger { get; }

        public async UniTask StartAsync(CancellationToken cancellationToken)
        {
            Logger.LogEditorDebug(
                "{Method}",
                nameof(StartAsync));

            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            var utcs = new UniTaskCompletionSource<bool>();
            var success = false;

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                await SetupMessageHandling(cancellationToken);
                success = true;
            }
            catch (System.OperationCanceledException e)
            {
                Logger.LogDebug("{Exception}", e);
                utcs.TrySetCanceled(cancellationToken);
            }
            catch (System.Exception e)
            {
                Logger.LogError("{Exception}", e);
            }
            finally
            {
                Logger.LogEditorDebug(
                    "{Method} reaches finally block",
                    nameof(StartAsync));

                utcs.TrySetResult(success);
            }

            await utcs.Task;
        }

        public void PublishMessage(string name, string stringParam)
        {
            Logger.LogEditorDebug("{Method} - Before comparing message", nameof(PublishMessage));
            if (name.Equals("CanUnloadRoom"))
            {
                // TODO: Adding the actual message publishing here
                Logger.LogEditorDebug("{Method} - CanUnloadRoom", nameof(PublishMessage));
                _pubUnloadContentLevel.Publish(new Game.Messages.UnloadContentLevel
                {
                    LevelBundleId = _levelBundleId,
                });
            }
            else if (name.Equals("AllContentSubSceneLoaded"))
            {
                Logger.LogEditorDebug("{Method} - AllContentSubSceneLoaded", nameof(PublishMessage));
                _pubContentLevelFullyLoaded.Publish(new Game.Messages.ContentLevelFullyLoaded
                {
                });
            }
        }

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

        private async ValueTask HandleDisposeAsync()
        {
            await Task.CompletedTask;
        }
    }
}
