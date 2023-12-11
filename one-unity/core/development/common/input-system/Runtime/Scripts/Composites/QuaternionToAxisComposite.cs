using TPFive.Game.Utils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.Scripting;

namespace TPFive.Extended.InputSystem.Composites
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    [Preserve]
    public class QuaternionToAxisComposite : InputBindingComposite<float>
    {
#pragma warning disable SA1401
        [InputControl(layout = "Quaternion")]
        public int Source;

        /// <summary>
        /// Ignore check the x-axis of euler angle.
        /// </summary>
        public bool IgnoreX;

        /// <summary>
        /// Ignore check the y-axis of euler angle.
        /// </summary>
        public bool IgnoreY;

        /// <summary>
        /// Ignore check the z-axis of euler angle.
        /// </summary>
        public bool IgnoreZ;

        /// <summary>
        /// Indicates the minimum x-axis of euler angle. degree: [0, 360).
        /// </summary>
        public float MinX = 0f;

        /// <summary>
        /// Indicates the minimum y-axis of euler angle. degree: [0, 360).
        /// </summary>
        public float MinY = 0f;

        /// <summary>
        /// Indicates the minimum z-axis of euler angle. degree: [0, 360).
        /// </summary>
        public float MinZ = 0f;

        /// <summary>
        /// Indicates the maximum x-axis of euler angle. degree: [0, 360).
        /// </summary>
        public float MaxX = 360f;

        /// <summary>
        /// Indicates the maximum y-axis of euler angle. degree: [0, 360).
        /// </summary>
        public float MaxY = 360f;

        /// <summary>
        /// Indicates the maximum z-axis of euler angle. degree: [0, 360).
        /// </summary>
        public float MaxZ = 360f;
#pragma warning restore SA1401

        private bool isInit;
        private Vector3 minEulerAngles;
        private Vector3 maxEulerAngles;
        private Vector3 offset;
        private Vector3 offsetMaxEulerAngles;

        static QuaternionToAxisComposite()
        {
            UnityEngine.InputSystem.InputSystem.RegisterBindingComposite<QuaternionToAxisComposite>();
        }

        public override float ReadValue(ref InputBindingCompositeContext context)
        {
            InitRangeIfNeed();

            var value = context.ReadValueAsObject(Source);
            if (value is not Quaternion quaternion)
            {
                return 0f;
            }

            var curtEulerAngles = quaternion.eulerAngles;
            var curtRotation = EulerAnglesUtility.GetNormalized(curtEulerAngles + offset);

            bool isInRange = true;
            if (!IgnoreX)
            {
                isInRange &= MathUtility.IsInRange(curtRotation.x, 0, offsetMaxEulerAngles.x);
            }

            if (!IgnoreY)
            {
                isInRange &= MathUtility.IsInRange(curtRotation.y, 0, offsetMaxEulerAngles.y);
            }

            if (!IgnoreZ)
            {
                isInRange &= MathUtility.IsInRange(curtRotation.z, 0, offsetMaxEulerAngles.z);
            }

            return isInRange ? 1f : 0f;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            // Will execute the static constructor as a side effect.
        }

        private void InitRangeIfNeed()
        {
            if (isInit)
            {
                return;
            }

            minEulerAngles = new Vector3(MinX, MinY, MinZ);
            maxEulerAngles = new Vector3(MaxX, MaxY, MaxZ);

            offset = new Vector3(360f - minEulerAngles.x, 360f - minEulerAngles.y, 360f - minEulerAngles.z);
            offsetMaxEulerAngles = EulerAnglesUtility.GetNormalized(maxEulerAngles + offset);

            isInit = true;
        }
    }
}