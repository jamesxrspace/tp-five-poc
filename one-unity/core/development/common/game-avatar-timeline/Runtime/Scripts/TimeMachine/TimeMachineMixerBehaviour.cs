using System.Collections.Generic;
using TPFive.Game.Avatar.Timeline;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TPFive.Game.Avatar.Timeline.TimeMachine
{
    /// <summary>
    /// This behaviour is used to control the time machine.
    /// When the timeline call the method of <see cref="OnBehaviourPlay"/>,
    /// it should initialize the all behaviour which implement from <see cref="TimeMachineEmptyBehaviour"/>.
    /// When the timeline call the method of <see cref="ProcessFrame"/>,
    /// it should invoke the <see cref="TimeMachineController.InvokeUpdateEvent"/> method to update time.
    /// </summary>
    public class TimeMachineMixerBehaviour : PlayableBehaviour
    {
        private TimeMachineController controller;

        public void Init(IAvatarTimelineManager manager, IEnumerable<TimelineClip> clips)
        {
            controller = new TimeMachineController(manager, clips);
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            int inputCount = playable.GetInputCount();

            for (int i = 0; i < inputCount; i++)
            {
                var inputPlayable = playable.GetInput(i);

                if (inputPlayable.GetPlayableType().BaseType != typeof(TimeMachineEmptyBehaviour))
                {
                    continue;
                }

                var timelinePlayable = (ScriptPlayable<TimeMachineEmptyBehaviour>)inputPlayable;
                var behaviour = timelinePlayable.GetBehaviour();
                behaviour.Initialize(controller, controller.ClipDictionaryById[behaviour.Id]);
            }
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (controller == null)
            {
                return;
            }

            controller.InvokeUpdateEvent();
        }
    }
}