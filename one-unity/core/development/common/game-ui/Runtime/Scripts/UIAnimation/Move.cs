using System;
using DG.Tweening;
using UnityEngine;

namespace TPFive.Game.UI
{
    [Serializable]
    public class Move
    {
        [SerializeField]
        private bool enabled;

        [SerializeField]
        private Anim.AnimationType animationType;

        [SerializeField]
        private MoveDirection direction;

        [SerializeField]
        private Vector3 customPosition;

        [SerializeField]
        private UIAnimator.EaseType easeType;

        [SerializeField]
        private Ease ease = UIAnimator.DefaultEase;

        [SerializeField]
        private AnimationCurve animationCurve;

        [SerializeField]
        private float startDelay = UIAnimator.DefaultStartDelay;

        [SerializeField]
        private float duration = UIAnimator.DefaultDuration;

        public enum MoveDirection
        {
            Left = 0,
            Right = 1,
            Top = 2,
            Bottom = 3,
            TopLeft = 4,
            TopCenter = 5,
            TopRight = 6,
            MiddleLeft = 7,
            MiddleCenter = 8,
            MiddleRight = 9,
            BottomLeft = 10,
            BottomCenter = 11,
            BottomRight = 12,
            CustomPosition = 13,
        }

        /// <summary>
        /// If TRUE, this animation will get executed by the Animator when triggered, FALSE otherwise (default: false).
        /// </summary>
        public bool Enabled
        {
            get => enabled;
            set => enabled = value;
        }

        /// <summary>
        /// Determines what type of animation this is. This changes the way the Animator percieves the set values.
        /// </summary>
        public Anim.AnimationType AnimationType
        {
            get => animationType;
            set => animationType = value;
        }

        /// <summary>
        /// Depending on the animation type, the direction is considered either IN (eg. Move IN from Left) or OUT (eg. Move OUT to Left)
        /// </summary>
        public MoveDirection Direction
        {
            get => direction;
            set => direction = value;
        }

        /// <summary>
        /// Depending on the animation type, this is considered either the TO or the FROM position (when moveDirection is set to CustomPosition).
        /// If it's a STATE animation type this is considered the move by variable.
        /// </summary>
        public Vector3 CustomPosition
        {
            get => customPosition;
            set => customPosition = value;
        }

        /// <summary>
        /// Use an Ease or an AnimationCurve in order to calculate the rate of change of the animation over time.
        /// </summary>
        public UIAnimator.EaseType EaseType
        {
            get => easeType;
            set => easeType = value;
        }

        /// <summary>
        /// Sets the ease of the tween. Easing functions specify the rate of change of a parameter over time.
        /// <para>To see how default ease curves look, check out easings.net.</para>
        /// </summary>
        public Ease Ease
        {
            get => ease;
            set => ease = value;
        }

        /// <summary>
        /// If the easeType is set to AnimationCurve, this will be used in order to calculate the rate of change of the animation over time.
        /// </summary>
        public AnimationCurve AnimationCurve
        {
            get => animationCurve;
            set => animationCurve = value;
        }

        /// <summary>
        /// Start delay for the animation.
        /// </summary>
        public float StartDelay
        {
            get => startDelay;
            set => startDelay = value;
        }

        /// <summary>
        /// The duration of the animation.
        /// </summary>
        public float Duration
        {
            get => duration;
            set => duration = value;
        }

        public float TotalDuration => StartDelay + Duration;
    }
}
