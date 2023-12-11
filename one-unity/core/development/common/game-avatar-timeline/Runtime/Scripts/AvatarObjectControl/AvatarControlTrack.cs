using UnityEngine;
using UnityEngine.Timeline;

namespace TPFive.Game.Avatar.Timeline.AvatarObjectControl
{
    /// <summary>
    /// It's a copy of <see cref="ControlTrack"/>
    /// A Track whose clips control time-related elements on a GameObject.
    /// </summary>
    [TrackClipType(typeof(AvatarControlPlayableAsset), false)]
    [ExcludeFromPreset]
    public class AvatarControlTrack : TrackAsset
    {
    }
}