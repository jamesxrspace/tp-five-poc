using UnityEngine;

namespace TPFive.Game.Avatar.Tracking
{
    public interface IAvatarTrackingSettings
    {
        AvatarMask UpperBodyMask { get; }

        float LayerFadeDuration { get; }
    }
}