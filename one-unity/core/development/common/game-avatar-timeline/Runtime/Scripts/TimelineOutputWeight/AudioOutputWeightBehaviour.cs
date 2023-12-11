using UnityEngine.Audio;
using UnityEngine.Playables;

namespace TPFive.Game.Avatar.Timeline.OutputWeight
{
    /// <summary>
    /// This behaviour is used to set the weight of all Audio output in the avatar.
    /// </summary>
    public class AudioOutputWeightBehaviour : PlayableBehaviour, IAvatarTimelineOutput
    {
        private AudioPlayableOutput[] outputs;

        public float Weight { get; set; }

        public void SetOutputs(AudioPlayableOutput[] outputs)
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