using System;
using UnityEngine.Timeline;

namespace TPFive.Game.Avatar.Timeline
{
    public interface IAvatarTimelineManager
    {
        // The event when motion end.
        event Action OnStop;

        // The state of motion at avatar.
        bool IsPlaying { get; }

        // The time of motion at avatar.
        double Time { get; set; }

        // The duration of motion at avatar.
        double Duration { get; }

        // The weight of all output control behaviour.
        float Weight { get; set; }

        // Play the TimelineAsset with avatar.
        void Play(TimelineAsset asset, bool loop = false);

        // Stop the playable director at avatar.
        void Stop();
    }
}