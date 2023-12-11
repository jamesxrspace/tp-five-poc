using System.Runtime.InteropServices;
using TPFive.Extended.InputSystem.Controls;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace TPFive.Extended.InputSystem.Devices
{
    [StructLayout(LayoutKind.Explicit, Size = 16)]
    public struct OnScreenGamePadState : IInputStateTypeInfo
    {
        /// <summary>
        /// Left stick position. Each axis goes from -1 to 1 with
        /// 0 being center position.
        /// </summary>
        /// <value>Left stick position.</value>
        /// <seealso cref="OnScreenGamepad.LeftStick"/>
        [InputControl(layout = "UnlimitStick", usage = "Primary2DMotion", displayName = "Left Stick", shortDisplayName = "LS", offset = 0)]
        [FieldOffset(0)]
        public Vector2 LeftStick;

        /// <summary>
        /// Right stick position. Each axis from -1 to 1 with
        /// 0 being center position.
        /// </summary>
        /// <value>Right stick position.</value>
        /// <seealso cref="OnScreenGamepad.RightStick"/>
        [InputControl(layout = "UnlimitStick", usage = "Secondary2DMotion", displayName = "Right Stick", shortDisplayName = "RS", offset = 8)]
        [FieldOffset(8)]
        public Vector2 RightStick;

        public static FourCC Format => new FourCC('O', 'S', 'G', 'P');

        /// <summary>
        /// Gets the state format tag for GamepadState.
        /// </summary>
        /// <value>Returns "OSGP".</value>
        public FourCC format => Format;
    }

#if UNITY_EDITOR
    // Add the InitializeOnLoad attribute to automatically run the static
    // constructor of the class after each C# domain load.
    [UnityEditor.InitializeOnLoad]
#endif
    [InputControlLayout(displayName = "On-Screen Gamepad", stateType = typeof(OnScreenGamePadState), isGenericTypeOfDevice = false)]
    public class OnScreenGamepad : InputDevice
    {
        static OnScreenGamepad()
        {
            UnityEngine.InputSystem.InputSystem.RegisterLayout<OnScreenGamepad>("OnScreenGamepad");
        }

        /// <summary>
        /// Gets or Sets the left thumbstick on the gamepad.
        /// </summary>
        /// <value>Control representing the left thumbstick.</value>
        public UnlimitStickControl LeftStick { get; protected set; }

        /// <summary>
        /// Gets or Sets the right thumbstick on the gamepad.
        /// </summary>
        /// <value>Control representing the right thumbstick.</value>
        public UnlimitStickControl RightStick { get; protected set; }

        protected override void FinishSetup()
        {
            base.FinishSetup();

            LeftStick = GetChildControl<UnlimitStickControl>("LeftStick");
            RightStick = GetChildControl<UnlimitStickControl>("RightStick");
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
        }
    }
}
