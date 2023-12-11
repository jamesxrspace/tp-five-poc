using UnityEngine.Animations;
using UnityEngine.Playables;

namespace TPFive.Game.Avatar.Timeline.OutputWeight
{
    /// <summary>
    /// This behaviour is used to set the weight of all animation output in the avatar.
    /// </summary>
    public class AnimationOutputWeightBehaviour : PlayableBehaviour, IAvatarTimelineOutput
    {
        private AnimationPlayableOutput[] outputs;

        public float Weight { get; set; }

        public void SetOutputs(AnimationPlayableOutput[] outputs)
        {
            this.outputs = outputs;
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            foreach (var output in outputs)
            {
                output.SetWeight(Weight);
            }
        }
    }
}