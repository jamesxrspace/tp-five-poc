using UnityEngine;
using UnityEngine.Playables;

namespace TPFive.Game.Avatar.Timeline.TimeMachine
{
    /// <summary>
    /// This class is a basic class for all TimeMachine playable.
    /// </summary>
    public class TimeMachineEmptyPlayable : TimeMachinePlayableBase
    {
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<TimeMachineEmptyBehaviour>.Create(graph);
            var behaviour = playable.GetBehaviour();
            behaviour.SetIdentification(Id, Tag);
            OnCreatePlayable(behaviour);
            return playable;
        }

        protected virtual void OnCreatePlayable(TimeMachineEmptyBehaviour behaviour)
        {
        }
    }
}