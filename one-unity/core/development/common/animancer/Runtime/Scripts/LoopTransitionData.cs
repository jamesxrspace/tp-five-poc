using Animancer;
using UnityEngine;

namespace TPFive.Extended.Animancer
{
    [CreateAssetMenu(fileName = "LoopTransitionData", menuName = "TPFive/Avatar/Animation Transition/Create new Loop Transition Data")]
    public class LoopTransitionData : TransitionDataBase
    {
        [SerializeField]
        [Tooltip("Clip loop playing when performing. e.g. talking animation")]
        private ClipTransition[] onPerformClips;

        public ClipTransition[] OnPerformClips => onPerformClips;
    }
}
