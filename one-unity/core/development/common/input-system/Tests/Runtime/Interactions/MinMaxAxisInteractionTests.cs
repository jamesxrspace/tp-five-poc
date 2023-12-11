using NUnit.Framework;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace TPFive.Extended.InputSystem.Interactions.Tests
{
    internal class MinMaxAxisInteractionTests : InputTestFixture
    {
        public override void Setup()
        {
            base.Setup();

            UnityEngine.InputSystem.InputSystem.RegisterInteraction<MinMaxAxisInteraction>();
        }

        [TestCase(0.7f, 0.6f, 1.0f, false, true)]
        [TestCase(0.7f, 0.6f, 1.0f, true, false)]
        [TestCase(0.2f, 0.6f, 1.0f, false, false)]
        [TestCase(0.2f, 0.6f, 1.0f, true, true)]
        public void MinMaxAxis(float actual, float min, float max, bool invert, bool expected)
        {
            var gamepad = UnityEngine.InputSystem.InputSystem.AddDevice<Gamepad>();

            var action = new InputAction("Min Max Axis", InputActionType.Value);
            action.AddBinding("<Gamepad>/leftTrigger", $"MinMaxAxis(Min={min},Max={max},invert={invert})");
            action.Enable();

            Assert.That(gamepad.leftTrigger.IsActuated(), Is.False, "Twist is actuated");
            Assert.That(action.triggered, Is.False, "Action is triggered");

            using (var trace = new InputActionTrace())
            {
                trace.SubscribeTo(action);

                Set(gamepad.leftTrigger, actual);

                if (expected)
                {
                    Assert.AreEqual(action.phase, InputActionPhase.Started, "Action is not actuated");
                }
                else
                {
                    Assert.AreEqual(action.phase, InputActionPhase.Waiting, "Action is actuated");
                }
            }

            action.Disable();
            UnityEngine.InputSystem.InputSystem.RemoveDevice(gamepad);
        }
    }
}