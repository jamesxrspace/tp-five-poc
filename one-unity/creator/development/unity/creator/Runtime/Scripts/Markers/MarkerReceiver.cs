using MessagePipe;
using UnityEngine.Playables;

namespace TPFive.Creator
{
    using CreatorMessages = TPFive.Creator.Messages;
    using CrossBridge = TPFive.Cross.Bridge;

    /// <summary>
    /// Currently this receives any marker
    /// Might be good to be the base class for all the receivers
    /// Can receive and send events via message pipe.
    /// </summary>
    public class MarkerReceiver : ComponentBase, INotificationReceiver
    {
        // private ILogger _logger;
        // private ILogger Logger
        // {
        //     get
        //     {
        //         _logger ??= CommonServiceLocator.ServiceLocator.Current
        //             .GetInstance<ILoggerFactory>()?.CreateLogger<MarkerReceiver>();
        //
        //         return _logger;
        //     }
        // }

        // TODO: Move back to TPFive.Game.Logging.
        private const int LogLevelDebug = 1;

        public void OnNotify(Playable origin, INotification notification, object context)
        {
            CrossBridge.Logging?.Invoke(
                typeof(MarkerReceiver),
                LogLevelDebug,
                $"MarkerReceiver - OnNotify");

            if (notification is GeneralMarker generalMarker)
            {
                // Logger.LogDebug($"MarkerReceiver - OnNotify - generalMarker: {generalMarker}");
                CrossBridge.Logging?.Invoke(
                    typeof(MarkerReceiver),
                    LogLevelDebug,
                    $"MarkerReceiver - OnNotify - generalMarker: {generalMarker}");

                var pubMarkerMessage = GlobalMessagePipe.GetPublisher<CreatorMessages.MarkerMessage>();
                pubMarkerMessage?.Publish(new CreatorMessages.MarkerMessage { IntParams = generalMarker.IntParams, FloatParams = generalMarker.FloatParams, });
            }
        }
    }
}