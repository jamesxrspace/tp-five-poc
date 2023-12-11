using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.Scripting;

namespace TPFive.Extended.InputSystem.Composites
{
    /// <summary>
    /// The angle (in degrees) between the two quaternions.
    /// </summary>
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    [Preserve]
    public class AngleBetweenTwoQuaternionsComposite : InputBindingComposite<float>
    {
#pragma warning disable SA1401
        /// <summary>
        /// The first input control to evaluate.
        /// </summary>
        [InputControl(layout = "Quaternion")]
        public int From;

        [InputControl(layout = "Quaternion")]
        public int To;
#pragma warning restore SA1401

        static AngleBetweenTwoQuaternionsComposite()
        {
            UnityEngine.InputSystem.InputSystem.RegisterBindingComposite<AngleBetweenTwoQuaternionsComposite>();
        }

        public override float ReadValue(ref InputBindingCompositeContext context)
        {
            var fromRot = context.ReadValueAsObject(From);
            if (fromRot is not Quaternion fromQuaternion)
            {
                return 0f;
            }

            var toRot = context.ReadValueAsObject(To);
            if (toRot is not Quaternion toQuaternion)
            {
                return 0f;
            }

            Vector3 fromForward = fromQuaternion * Vector3.forward;
            Vector3 toForward = toQuaternion * Vector3.forward;
            float angle = Vector3.Angle(fromForward, toForward);

            return angle;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            // Will execute the static constructor as a side effect.
        }
    }
}