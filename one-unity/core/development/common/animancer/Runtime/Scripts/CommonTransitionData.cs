using Animancer;
using UnityEngine;

namespace TPFive.Extended.Animancer
{
    [CreateAssetMenu(fileName = "CommonTransitionData", menuName = "TPFive/Avatar/Animation Transition/Create new Common Transition Data")]
    public class CommonTransitionData : TransitionDataBase
    {
        [SerializeField]
        [Tooltip("Clip to play when the transition is performing. e.g. playing 'sit down' loop animation")]
        private ClipTransition onPerformClip;

        public ClipTransition OnPerformClip => onPerformClip;
    }
}
