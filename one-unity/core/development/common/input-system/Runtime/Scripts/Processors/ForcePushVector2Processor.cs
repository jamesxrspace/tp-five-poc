using UnityEngine;
using UnityEngine.InputSystem;

namespace TPFive.Extended.InputSystem.Processors
{
    /// <summary>
    /// No matter what the current value is, force push to the specified <see cref="UnityEngine.Vector2"/>.
    /// </summary>
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public class ForcePushVector2Processor : InputProcessor<Vector2>
    {
#pragma warning disable SA1401
        public float X;
        public float Y;
#pragma warning restore SA1401

#if UNITY_EDITOR
        static ForcePushVector2Processor()
        {
            Initialize();
        }
#endif

        public override Vector2 Process(Vector2 value, InputControl control)
        {
            return new Vector2(X, Y);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            UnityEngine.InputSystem.InputSystem.RegisterProcessor<ForcePushVector2Processor>();
        }
    }
}