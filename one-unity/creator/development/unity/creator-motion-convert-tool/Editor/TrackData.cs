using TPFive.Game.Avatar.Timeline.AvatarObjectControl;
using TPFive.Game.Avatar.TimelineMotion.TimeMachine;
using UnityEngine.Timeline;

namespace TPFive.Creator.MotionConvertTool
{
    /// <summary>
    /// This struct is used to store the tracks of a timeline asset.
    /// </summary>
    public struct TrackData
    {
        public TimeMachineTrack TimeMachineTrack;
        public AnimationTrack AnimationTrack;
        public AudioTrack AudioTrack;
        public AvatarControlTrack AvatarControlTrack;
    }
}