namespace TPFive.Game.Avatar.Timeline.TimeMachine
{
    /// <summary>
    /// This behaviour will jump to the specified tag of playable behaviour,
    /// when the time is in the range of this clip.
    /// </summary>
    public class TimeMachineJumpBehaviour : TimeMachineEmptyBehaviour
    {
        /// <summary>
        /// The tag of the playable behaviour to jump to.
        /// </summary>
        private string jumpToTag;

        /// <summary>
        /// Tis method will be called when create the playable on playable graph.
        /// Can trace the caller at <see cref="TimeMachineJumpPlayable"/>.
        /// </summary>
        /// <param name="tag"> The tag of the playable behaviour to jump to. </param>
        public void SetJumpToTag(string tag)
        {
            jumpToTag = tag;
        }

        protected override void OnTimelineUpdate(double time)
        {
            if (time < Clip.start || time > Clip.end)
            {
                return;
            }

            if (string.IsNullOrEmpty(jumpToTag))
            {
                return;
            }

            if (Controller.ClipDictionaryByTag.TryGetValue(jumpToTag, out var targetClip))
            {
                Controller.SetTime((float)targetClip.start);
            }
        }
    }
}