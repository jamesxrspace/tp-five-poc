using System;
using Cysharp.Threading.Tasks;
using TPFive.Game.Mocap;

namespace TPFive.Game.Avatar.Tracking
{
    public interface IAvatarTrackingManager : IDisposable
    {
        event Action OnFaceTrackingStarted;

        event Action OnBodyTrackingStarted;

        event Action<bool> OnLossTrackingChanged;

        bool IsTracking { get; }

        UniTask StartTracking(CaptureOptions options);

        UniTask StopTracking();
    }
}