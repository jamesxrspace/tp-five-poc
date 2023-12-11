using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TPFive.Game.Avatar.Timeline.TimeMachine
{
    public abstract class TimeMachinePlayableBase : PlayableAsset, ITimelineClipAsset
    {
        /// <summary>
        /// The tag is set by user at Timeline editor window.
        /// The tag is used to help <see cref="TimeMachineController"/>,
        /// which can find the behaviour by tag.
        /// </summary>
        [SerializeField]
        private string tag;

        /// <summary>
        /// Gets or sets the uuid of TimeMachine behaviour.
        /// The id is set by <see cref="TimeMachineController"/>.
        /// </summary>
        /// <value> The uuid of TimeMachine behaviour.</value>
        public int Id { get; set; }

        public ClipCaps clipCaps => ClipCaps.None;

        public string Tag => tag;
    }
}