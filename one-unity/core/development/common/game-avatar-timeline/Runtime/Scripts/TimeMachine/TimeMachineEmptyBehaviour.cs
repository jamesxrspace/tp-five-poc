using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TPFive.Game.Avatar.Timeline.TimeMachine
{
    /// <summary>
    /// This class is a base class for all TimeMachine behaviour.
    /// </summary>
    public class TimeMachineEmptyBehaviour : PlayableBehaviour
    {
        /// <summary>
        /// Gets the uuid of TimeMachine behaviour.
        /// The id is set by <see cref="TimeMachineController"/>.
        /// </summary>
        /// <value>The uuid of TimeMachine behaviour.</value>
        public int Id { get; private set; }

        /// <summary>
        /// Gets the tag of TimeMachine behaviour.
        /// It is used to search TimeMachine behaviour.
        /// </summary>
        /// <value>The tag of TimeMachine behaviour.</value>
        public string Tag { get; private set; }

        protected TimeMachineController Controller { get; private set; }

        protected TimelineClip Clip { get; private set; }

        public void SetIdentification(int id, string tag)
        {
            Id = id;
            Tag = tag;
        }

        /// <summary>
        /// This method is called by <see cref="TimeMachineMixerBehaviour"/>.
        /// </summary>
        /// <param name="controller">The controller of TimeMachine.</param>
        /// <param name="clip">The clip which this behaviour to handle.</param>
        public void Initialize(TimeMachineController controller, TimelineClip clip)
        {
            this.Controller = controller;
            this.Clip = clip;
            OnTimelineStart();
            controller.UpdateEvent += OnTimelineUpdate;
            controller.Manager.OnStop += OnStop;
        }

        protected virtual void OnTimelineStart()
        {
        }

        protected virtual void OnTimelineUpdate(double time)
        {
        }

        protected virtual void OnTimelineEnd()
        {
        }

        private void OnStop()
        {
            OnTimelineEnd();
            Controller.UpdateEvent -= OnTimelineUpdate;
            Controller.Manager.OnStop -= OnStop;
        }
    }
}