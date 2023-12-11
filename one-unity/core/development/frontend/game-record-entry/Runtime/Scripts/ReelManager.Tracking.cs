using System;
using Cysharp.Threading.Tasks;
using TPFive.Game.Avatar.Tracking;
using TPFive.Game.Mocap;

namespace TPFive.Game.Record.Entry
{
    public partial class ReelManager
    {
        private IAvatarTrackingManager trackingManager;

        public event Action OnFaceTrackingStarted;

        public event Action OnBodyTrackingStarted;

        public event Action<bool> OnLossTrackingChanged;

        private IAvatarTrackingManager TrackingManager
        {
            get
            {
                if (trackingManager is not null)
                {
                    return trackingManager;
                }

                if (userPlayer == null)
                {
                    throw new ArgumentException("Cannot find the user avatar.");
                }

                trackingManager = userPlayer.Avatar.TrackingManager;

                if (trackingManager is null)
                {
                    throw new ArgumentException($"Cannot get the {nameof(IAvatarTrackingManager)} at the main avatar.");
                }

                trackingManager.OnFaceTrackingStarted += OnFaceTrackingStarted;
                trackingManager.OnBodyTrackingStarted += OnBodyTrackingStarted;
                trackingManager.OnLossTrackingChanged += OnLossTrackingChanged;

                return trackingManager;
            }
        }

        public async UniTask SetTrackingMode(bool enableFace, bool enableBody)
        {
            if (!enableFace && !enableBody)
            {
                await TrackingManager.StopTracking();
                return;
            }

            var options = CaptureOptions.None;

            if (enableFace)
            {
                options.EnableFace();
            }

            if (enableBody)
            {
                options.EnableUpperBody();
            }

            await TrackingManager.StartTracking(options);
        }
    }
}
