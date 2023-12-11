using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace TPFive.Extended.InputSystem.Composites.Tests
{
    internal class QuaternionToAxisCompositeTests : InputTestFixture
    {
        public override void Setup()
        {
            base.Setup();

            UnityEngine.InputSystem.InputSystem.RegisterBindingComposite<QuaternionToAxisComposite>();
        }

        [TestCase(0, 0, 0, 0)]
        [TestCase(0, 0, 180, 1)]
        public void ConvertQuaternionToAxis(float xAxis, float yAxis, float zAxis, float expected)
        {
            var attitudeSensor = UnityEngine.InputSystem.InputSystem.AddDevice<AttitudeSensor>();

            var action = new InputAction("Convert Quaternion To Axis", InputActionType.Value);
            action.AddCompositeBinding("QuaternionToAxis(IgnoreY=true,MinX=300,MinY=300,MinZ=120,MaxX=60,MaxY=60,MaxZ=200)")
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

                // Because floating-point precision may that real value
                // has a little difference from the expected value,
                // so we use AreApproximatelyEqual instead of AreEqual.
                UnityEngine.Assertions.Assert.AreApproximatelyEqual(
                    expected,
                    action.ReadValue<float>(),
                    0.0001f,
                    $"Action value is not {expected}");
            }

            action.Disable();
            UnityEngine.InputSystem.InputSystem.RemoveDevice(attitudeSensor);
        }
    }
}