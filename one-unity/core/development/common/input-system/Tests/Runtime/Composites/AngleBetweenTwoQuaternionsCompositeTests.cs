#if XR_HANDS
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.XR.Hands;

namespace TPFive.Extended.InputSystem.Composites.Tests
{
    /// <summary>
    /// Tests for <see cref="AngleBetweenTwoQuaternionsComposite"/> Composite.
    /// </summary>
    /// <remarks>
    /// Because <see cref="AngleBetweenTwoQuaternionsComposite"/> needs two quaternion values,
    /// but the generic <see cref="UnityEngine.InputSystem.InputDevice"/> type doesn't
    /// have two quaternion values in same device,
    /// so we use <see cref="UnityEngine.XR.Hands.XRHandDevice"/> to test this composite.
    /// </remarks>
    internal class AngleBetweenTwoQuaternionsCompositeTests : InputTestFixture
    {
        public override void Setup()
        {
            base.Setup();

            UnityEngine.InputSystem.InputSystem.RegisterBindingComposite<AngleBetweenTwoQuaternionsComposite>();
        }

        [TestCase(0)]
        [TestCase(30)]
        [TestCase(125)]
        [TestCase(180)]
        [TestCase(200)]
        [TestCase(325)]
        [TestCase(360)]
        public void CalcAngleBetweenGripAndPoke(float pokeAxisXAngle)
        {
            var handDevice = UnityEngine.InputSystem.InputSystem.AddDevice<XRHandDevice>();
            UnityEngine.InputSystem.InputSystem.SetDeviceUsage(handDevice, CommonUsages.LeftHand);

            var action = new InputAction("Calc Angle Between Grip And Poke", InputActionType.Value);
            action.AddCompositeBinding("AngleBetweenTwoQuaternions")
                .With("From", "<XRHandDevice>{LeftHand}/pokeRotation")
                .With("To", "<XRHandDevice>{LeftHand}/gripRotation");
            action.Enable();

            // Starting below threshold
            Assert.That(handDevice.pokeRotation.IsActuated(), Is.False, "Poke Rotation is actuated");
            Assert.That(handDevice.gripRotation.IsActuated(), Is.False, "Grip Rotation is actuated");
            Assert.That(action.triggered, Is.False, "Action is triggered");

            float expectedAngle = pokeAxisXAngle > 180 ? 360 - pokeAxisXAngle : pokeAxisXAngle;

            using (var trace = new InputActionTrace())
            {
                trace.SubscribeTo(action);

                Quaternion pokeRotation = Quaternion.Euler(pokeAxisXAngle, 0, 180);
                Quaternion gripRotation = Quaternion.Euler(0, 0, 180);

                Set(handDevice.pokeRotation, pokeRotation);
                Set(handDevice.gripRotation, gripRotation);

                Assert.That(handDevice.pokeRotation.IsActuated(), Is.True, "Poke Rotation is not actuated");
                Assert.That(handDevice.gripRotation.IsActuated(), Is.True, "Grip Rotation is not actuated");

                // Because floating-point precision may that real value
                // has a little difference from the expected value,
                // so we use AreApproximatelyEqual instead of AreEqual.
                UnityEngine.Assertions.Assert.AreApproximatelyEqual(
                    expectedAngle,
                    action.ReadValue<float>(),
                    0.0001f,
                    $"Action value is not {expectedAngle}");
            }

            action.Disable();
            UnityEngine.InputSystem.InputSystem.RemoveDevice(handDevice);
        }
    }
}
#endif // XR_HANDS