using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace TPFive.Extended.InputSystem.Composites.Tests
{
    /// <summary>
    /// Tests for <see cref="QuaternionToVector2Composite"/> Composite.
    /// </summary>
    internal class QuaternionToVector2CompositeTests : InputTestFixture
    {
        private const float Tolerance = 0.001f;

        public override void Setup()
        {
            base.Setup();

            UnityEngine.InputSystem.InputSystem.RegisterBindingComposite<QuaternionToVector2Composite>();
        }

        [TestCase(0, 0, 0, 0, 1)]
        [TestCase(0, 45, 0, 0.707f, 0.707f)] // 0.707 is from 1/sqrt(2)
        [TestCase(0, 90, 0, 1, 0)]
        [TestCase(0, 135, 0, 0.707f, -0.707f)]
        [TestCase(0, 180, 0, 0, -1)]
        [TestCase(0, 225, 0, -0.707f, -0.707f)]
        [TestCase(0, 270, 0, -1, 0)]
        [TestCase(0, 315, 0, -0.707f, 0.707f)]
        public void ConvertQuaternionToVector2(float xAxis, float yAxis, float zAxis, float expectedX, float expectedY)
        {
            var attitudeSensor = UnityEngine.InputSystem.InputSystem.AddDevice<AttitudeSensor>();

            var action = new InputAction("Convert Quaternion To Vector2", InputActionType.Value);
            action.AddCompositeBinding("QuaternionToVector2")
                .With("Source", "<AttitudeSensor>/attitude");
            action.Enable();

            Assert.That(attitudeSensor.attitude.IsActuated(), Is.False, "Attitude is actuated");
            Assert.That(action.triggered, Is.False, "Action is triggered");

            using (var trace = new InputActionTrace())
            {
                trace.SubscribeTo(action);

                Quaternion actualRotation = Quaternion.Euler(xAxis, yAxis, zAxis);
                Set(attitudeSensor.attitude, actualRotation);

                Assert.That(attitudeSensor.attitude.IsActuated(), Is.True, "Attitude is not actuated");

                Vector2 actualVector2 = action.ReadValue<Vector2>();

                // Because floating-point precision may that real value
                // has a little difference from the expected value,
                // so we use AreApproximatelyEqual instead of AreEqual.
                UnityEngine.Assertions.Assert.AreApproximatelyEqual(
                    expectedX,
                    actualVector2.x,
                    Tolerance,
                    $"Vector2.x is not {expectedX}");

                UnityEngine.Assertions.Assert.AreApproximatelyEqual(
                    expectedY,
                    actualVector2.y,
                    Tolerance,
                    $"Vector2.y is not {expectedX}");
            }

            action.Disable();
            UnityEngine.InputSystem.InputSystem.RemoveDevice(attitudeSensor);
        }
    }
}