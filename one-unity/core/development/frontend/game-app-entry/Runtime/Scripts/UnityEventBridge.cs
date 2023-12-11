using MessagePipe;
using Microsoft.Extensions.Logging;
using TPFive.Game.Messages;
using VContainer;
using VContainer.Unity;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Game.App.Entry
{
    public sealed class UnityEventBridge : IStartable
    {
        private readonly IPublisher<ApplicationQuit> applicationQuitPublisher;
        private readonly IPublisher<ApplicationFoucs> applicationFocusPublisher;
        private readonly IPublisher<ApplicationPause> applicationPausePublisher;
        private readonly ILogger logger;

        [Inject]
        public UnityEventBridge(
            ILoggerFactory loggerFactory,
            IPublisher<ApplicationQuit> applicationQuitPublisher,
            IPublisher<ApplicationFoucs> applicationFocusPublisher,
            IPublisher<ApplicationPause> applicationPausePublisher)
        {
            logger = Logging.Utility.CreateLogger<UnityEventBridge>(loggerFactory);
            this.applicationQuitPublisher = applicationQuitPublisher;
            this.applicationFocusPublisher = applicationFocusPublisher;
            this.applicationPausePublisher = applicationPausePublisher;
        }

        public void Start()
        {
            GameApp.OnApplicationQuit += OnApplicationQuit;
            GameApp.OnApplicationFocus += OnApplicationFocus;
            GameApp.OnApplicationPause += OnApplicationPause;
        }

        private void OnApplicationQuit()
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug("Application Quit");
            }

            applicationQuitPublisher.Publish(default);
        }

        private void OnApplicationFocus(bool focus)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug($"Application {(focus ? "Focus" : "Unfocus")}");
            }

            applicationFocusPublisher.Publish(new ApplicationFoucs
            {
                Focus = focus,
            });
        }

        private void OnApplicationPause(bool pause)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug($"Application {(pause ? "Pasue" : "Unpause")}");
            }

            applicationPausePublisher.Publish(new ApplicationPause
            {
                Pause = pause,
            });
        }
    }
}
