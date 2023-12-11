using System;
using TPFive.Game.Avatar.Timeline;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TPFive.Game.Avatar.Timeline.TimeMachine
{
    /// <summary>
    /// The track for the TimeMachine in timeline.
    /// Provide custom timeline lifecycle.
    /// Example: The <see cref="TimeMachineJumpPlayable"/> can be used to jump to a specific time.
    /// </summary>
    [TrackColor(1, 1, 0)]
    [TrackClipType(typeof(TimeMachineEmptyPlayable))]
    [TrackClipType(typeof(TimeMachineJumpPlayable))]
    public class TimeMachineTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            if (!go.TryGetComponent<IAvatarTimelineManager>(out var manager))
            {
                throw new Exception("The AvatarTimelineManager is null");
            }

            var scriptPlayable = ScriptPlayable<TimeMachineMixerBehaviour>.Create(graph, inputCount);
            var behaviour = scriptPlayable.GetBehaviour();
            behaviour.Init(manager, GetClips());
            return scriptPlayable;
        }
    }
}