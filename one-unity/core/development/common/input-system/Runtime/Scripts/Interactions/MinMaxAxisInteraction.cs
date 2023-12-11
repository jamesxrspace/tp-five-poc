using System.ComponentModel;
using TPFive.Game.Utils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting;

namespace TPFive.Extended.InputSystem.Interactions
{
    /// <summary>
    /// Performs the action at specific float range.
    /// </summary>
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    [Preserve]
    [DisplayName("Mix Max Axis(float [0, 1])")]
    public class MinMaxAxisInteraction : IInputInteraction<float>
    {
#pragma warning disable SA1401
        /// <summary>
        /// Indicates the minimum value.
        /// </summary>
        public float Min;

        /// <summary>
        /// Indicates the maximum value.
        /// </summary>
        public float Max;

        /// <summary>
        /// Invert the output or not.
        /// </summary>
        public bool Invert;
#pragma warning restore SA1401

        private bool wasStartedOrPerformed;

        static MinMaxAxisInteraction()
        {
            UnityEngine.InputSystem.InputSystem.RegisterInteraction<MinMaxAxisInteraction>();
        }

        public void Process(ref InputInteractionContext context)
        {
            bool isActuated = context.ControlIsActuated();
            if (!isActuated)
            {
                if (wasStartedOrPerformed)
                {
                    wasStartedOrPerformed = false;
                    context.Canceled();
                }

                return;
            }

            float curtValue = context.ReadValue<float>();
            bool isInRange = MathUtility.IsInRange(curtValue, Min, Max);

            if (Invert)
            {
                isInRange = !isInRange;
            }

            if (!isInRange)
            {
                if (wasStartedOrPerformed)
                {
                    wasStartedOrPerformed = false;
                    context.Canceled();
                }

                return;
            }

            if (!wasStartedOrPerformed)
            {
                wasStartedOrPerformed = true;
                context.Started();
                return;
            }

            context.PerformedAndStayPerformed();
        }

        public void Reset()
        {
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
        }
    }
}