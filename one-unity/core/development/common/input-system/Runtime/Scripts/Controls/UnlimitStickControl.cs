using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;

namespace TPFive.Extended.InputSystem.Controls
{
#if UNITY_EDITOR
    // Add the InitializeOnLoad attribute to automatically run the static
    // constructor of the class after each C# domain load.
    [UnityEditor.InitializeOnLoad]
#endif
    public class UnlimitStickControl : Vector2Control
    {
        static UnlimitStickControl()
        {
            UnityEngine.InputSystem.InputSystem.RegisterLayout<UnlimitStickControl>("UnlimitStick");
        }

        [InputControl(useStateFrom = "y", processors = "axisDeadzone", parameters = "clamp=0", synthetic = true, displayName = "Up")]
        public ButtonControl Up { get; set; }

        [InputControl(useStateFrom = "y", processors = "axisDeadzone", parameters = "clamp=0,invert", synthetic = true, displayName = "Down")]
        public ButtonControl Down { get; set; }

        [InputControl(useStateFrom = "x", processors = "axisDeadzone", parameters = "clamp=0,invert", synthetic = true, displayName = "Left")]
        public ButtonControl Left { get; set; }

        [InputControl(useStateFrom = "x", processors = "axisDeadzone", parameters = "clamp=0", synthetic = true, displayName = "Right")]
        public ButtonControl Right { get; set; }

        protected override void FinishSetup()
        {
            base.FinishSetup();

            Up = GetChildControl<ButtonControl>("Up");
            Down = GetChildControl<ButtonControl>("Down");
            Left = GetChildControl<ButtonControl>("Left");
            Right = GetChildControl<ButtonControl>("Right");
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
        }
    }
}
