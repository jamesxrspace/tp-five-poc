using System.Reflection;
using TPFive.Game.Avatar.TimelineMotion.TimeMachine;
using UnityEngine.Timeline;
using XRSpace.PlayableMotion;

namespace TPFive.Creator.MotionConvertTool
{
    /// <summary>
    /// Convert <see cref="AvatarPlayAnimationAsset"/> to <see cref="AnimationPlayableAsset"/>
    /// </summary>
    public class AnimationPlayableCopy : PlayableCopy
    {
        public override void Copy(TimelineClip sourceClip, TimelineClip targetClip, TrackData trackData)
        {
            base.Copy(sourceClip, targetClip, trackData);

            if (sourceClip.asset is not AvatarPlayAnimationAsset sourceAsset)
            {
                return;
            }

            if (!sourceAsset.IsLoop())
            {
                return;
            }

            CreateLoopClip(sourceClip, trackData);
        }

        /// <summary>
        /// If the source clip is loop, create a loop behaviour in <see cref="TimeMachineTrack"/>
        /// </summary>
        private void CreateLoopClip(TimelineClip sourceClip, TrackData trackData)
        {
            var startAsset = trackData.TimeMachineTrack.CreateClip<TimeMachineEmptyPlayable>();
            startAsset.duration = 1f;
            startAsset.start = sourceClip.start;
            startAsset.displayName = "LoopStart";
            var tagField = typeof(TimeMachinePlayableBase).GetField("tag", BindingFlags.NonPublic | BindingFlags.Instance);
            tagField?.SetValue(startAsset.asset, "LoopStart");

            var endAsset = trackData.TimeMachineTrack.CreateClip<TimeMachineJumpPlayable>();
            endAsset.duration = 1f;
            endAsset.start = sourceClip.end;
            endAsset.displayName = "LoopEnd";
            var jumpField = typeof(TimeMachineJumpPlayable).GetField("jumpToTag", BindingFlags.NonPublic | BindingFlags.Instance);
            jumpField?.SetValue(endAsset.asset, "LoopStart");
        }
    }
}