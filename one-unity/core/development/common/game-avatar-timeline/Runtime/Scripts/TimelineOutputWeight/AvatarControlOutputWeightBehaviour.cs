using UnityEngine.Playables;

namespace TPFive.Game.Avatar.Timeline.OutputWeight
{
    /// <summary>
    /// This behaviour is used to set the weight of all control output in the avatar.
    /// </summary>
    public class AvatarControlOutputWeightBehaviour : PlayableBehaviour, IAvatarTimelineOutput
    {
        private ScriptPlayableOutput[] outputs;

        public float Weight { get; set; }

        public void SetOutputs(ScriptPlayableOutput[] outputs)
        {
            this.outputs = outputs;
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (outputs == null || outputs.Length == 0)
            {
                return;
            }

            foreach (var output in outputs)
            {
                output.SetWeight(Weight);
            }
        }
    }
}