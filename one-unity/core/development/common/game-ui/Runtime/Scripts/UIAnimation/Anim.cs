using System;
using UnityEngine;

namespace TPFive.Game.UI
{
    [Serializable]
    public class Anim
    {
        [SerializeField]
        private AnimationType type;

        [SerializeField]
        private Move move;

        [SerializeField]
        private Rotate rotate;

        [SerializeField]
        private Scale scale;

        [SerializeField]
        private Fade fade;

        /// <summary>
        /// Type of Animation. This changes the way the Animator percieves the set values.
        /// </summary>
        public enum AnimationType
        {
            In,
            Out,
            State,
        }

        /// <summary>
        /// Determines what type of animation this is. This changes the way the Animator percieves the set values.
        /// </summary>
        public AnimationType Type
        {
            get => type;
            set => type = value;
        }

        /// <summary>
        /// Movement animation settings.
        /// </summary>
        public Move Move
        {
            get => move;
            set => move = value;
        }

        /// <summary>
        /// Rotation animation settings.
        /// </summary>
        public Rotate Rotate
        {
            get => rotate;
            set => rotate = value;
        }

        /// <summary>
        /// Scale animation settings.
        /// </summary>
        public Scale Scale
        {
            get => scale;
            set => scale = value;
        }

        /// <summary>
        /// Fade (alpha) animation settings.
        /// </summary>
        public Fade Fade
        {
            get => fade;
            set => fade = value;
        }

        public bool Enabled => Move.Enabled || Rotate.Enabled || Scale.Enabled || Fade.Enabled;

        public float TotalDuration
        {
            get
            {
                return Mathf.Max(
                    Move.Enabled ? Move.TotalDuration : 0,
                    Rotate.Enabled ? Rotate.TotalDuration : 0,
                    Scale.Enabled ? Scale.TotalDuration : 0,
                    Fade.Enabled ? Fade.TotalDuration : 0);
            }
        }

        public float StartDelay
        {
            get
            {
                if (!Enabled)
                {
                    return 0;
                }

                return Mathf.Min(
                    Move.Enabled ? Move.StartDelay : 10000,
                    Rotate.Enabled ? Rotate.StartDelay : 10000,
                    Scale.Enabled ? Scale.StartDelay : 10000,
                    Fade.Enabled ? Fade.StartDelay : 10000);
            }
        }
    }
}
