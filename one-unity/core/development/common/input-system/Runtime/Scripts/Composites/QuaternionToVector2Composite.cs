using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.Scripting;

namespace TPFive.Extended.InputSystem.Composites
{
    /// <summary>
    /// Convert quaternion to vector2.
    /// </summary>
    /// <remarks>
    /// Convert quaternion to forward vector, and then convert forward vector to vector2.
    /// </remarks>
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    [Preserve]
    public class QuaternionToVector2Composite : InputBindingComposite<Vector2>
    {
#pragma warning disable SA1401
        [InputControl(layout = "Quaternion")]
        public int Source;
#pragma warning restore SA1401

        static QuaternionToVector2Composite()
        {
            UnityEngine.InputSystem.InputSystem.RegisterBindingComposite<QuaternionToVector2Composite>();
        }

        public override Vector2 ReadValue(ref InputBindingCompositeContext context)
        {
            var value = context.ReadValueAsObject(Source);
            if (value is not Quaternion quaternion)
            {
                return Vector2.zero;
            }

            Vector3 forward = quaternion * Vector3.forward;
            Vector2 result = new Vector2(forward.x, forward.z);

            return result.normalized;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            // Will execute the static constructor as a side effect.
        }
    }
}