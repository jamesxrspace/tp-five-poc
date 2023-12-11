using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace TPFive.Extended.InputSystem.Processors.Tests
{
    internal class ForcePushVector2ProcessorTests : InputTestFixture
    {
        private const float Tolerance = 0.001f;

        public override void Setup()
        {
            base.Setup();

            UnityEngine.InputSystem.InputSystem.RegisterProcessor<ForcePushVector2Processor>();
        }

        [TestCase(0.7f, 0.6f)]
        [TestCase(-0.7f, 0.6f)]
        [TestCase(0.2f, 0.6f)]
        [TestCase(0.2f, -0.6f)]
        public void ForcePushVector2(float xAxis, float yAxis)
        {
            var gamepad = UnityEngine.InputSystem.InputSystem.AddDevice<Gamepad>();

            var action = new InputAction("Force Push Vector2", InputActionType.Value);
            action.AddBinding("<Gamepad>/leftStick", processors: "ForcePushVector2(X=0,Y=1)");
            action.Enable();

            Assert.That(gamepad.leftStick.IsActuated(), Is.False, "LeftStick is actuated");
            Assert.That(action.triggered, Is.False, "Action is triggered");

            using (var trace = new InputActionTrace())
            {
                trace.SubscribeTo(action);

                Set(gamepad.leftStick, new Vector2(xAxis, yAxis));

                Vector2 actualVector2 = action.ReadValue<Vector2>();

                UnityEngine.Assertions.Assert.AreApproximatelyEqual(
                    0f,
                    actualVector2.x,
                    Tolerance,
                    $"Vector2.x is not 0");

                UnityEngine.Assertions.Assert.AreApproximatelyEqual(
                    1f,
                    actualVector2.y,
                    Tolerance,
                    $"Vector2.y is not 1");
            }

            action.Disable();
            UnityEngine.InputSystem.InputSystem.RemoveDevice(gamepad);
        }
    }
}