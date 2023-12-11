using UnityEngine;

namespace TPFive.Game.Avatar.Timeline.TimeMachine
{
    public class TimeMachineJumpPlayable : TimeMachineEmptyPlayable<TimeMachineJumpBehaviour>
    {
        /// <summary>
        /// The tag of the playable behaviour to jump to.
        /// The tag is set by user at Timeline editor window.
        /// </summary>
        [SerializeField]
        private string jumpToTag;

        protected override void OnCreatePlayable(TimeMachineJumpBehaviour behaviour)
        {
            behaviour.SetJumpToTag(jumpToTag);
        }
    }
}