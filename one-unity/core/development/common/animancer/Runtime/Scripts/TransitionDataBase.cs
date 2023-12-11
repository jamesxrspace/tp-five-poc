using Animancer;
using TPFive.Game;
using UnityEngine;

namespace TPFive.Extended.Animancer
{
    public abstract class TransitionDataBase : ScriptableObject
    {
        [SerializeField]
        [Tooltip("Mask to control which human bones are affected. e.g. 'upper body' mask for 'talking' animation")]
        private AvatarMask mask;

        [SerializeField]
        [Tooltip("Offset position to apply to the game object (in world space)")]
        private Optional<Vector3> offsetPosition;

        [SerializeField]
        [Tooltip("Offset rotation to apply to the game object (in world space)")]
        private Optional<Vector3> offsetRotation;

        [SerializeField]
        [Tooltip("Duration to fade the layer (in seconds)")]
        private float layerFadeDuration;

        [SerializeField]
        [Tooltip("Clip to play when the transition starts. e.g. playing 'stand up' to 'sit down' animation")]
        private ClipTransition onStartClip;

        [SerializeField]
        [Tooltip("Clip to play when the transition is ended. e.g. playing 'sit down' to 'stand up' animation")]
        private ClipTransition onEndClip;

        public AvatarMask Mask => mask;

        public Optional<Vector3> OffsetPosition => offsetPosition;

        public Optional<Vector3> OffsetRotation => offsetRotation;

        public float LayerFadeDuration => layerFadeDuration;

        public ClipTransition OnStartClip => onStartClip;

        public ClipTransition OnEndClip => onEndClip;
    }
}
