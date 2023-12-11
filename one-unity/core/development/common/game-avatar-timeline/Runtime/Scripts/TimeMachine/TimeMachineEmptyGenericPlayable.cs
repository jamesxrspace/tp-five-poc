using UnityEngine;
using UnityEngine.Playables;

namespace TPFive.Game.Avatar.Timeline.TimeMachine
{
    /// <summary>
    /// This class is a basic class for all TimeMachine playable.
    /// </summary>
    /// <typeparam name="T">The custom behaviour should implement from <see cref="TimeMachinePlayableBase"/>.</typeparam>
    public class TimeMachineEmptyPlayable<T> : TimeMachinePlayableBase
        where T : TimeMachineEmptyBehaviour, new()
    {
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<T>.Create(graph);
            var behaviour = playable.GetBehaviour();
            behaviour.SetIdentification(Id, Tag);
            OnCreatePlayable(behaviour);
            return playable;
        }

        protected virtual void OnCreatePlayable(T behaviour)
        {
        }
    }
}