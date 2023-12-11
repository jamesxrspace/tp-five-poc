using NUnit.Framework;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert;

namespace TPFive.Game.Utils.Tests
{
    public class EulerAnglesUtilityTests
    {
        private const float Tolerance = 0.0001f;

        [TestCase(0, 0)]
        [TestCase(80, 80)]
        [TestCase(-80, 280)]
        [TestCase(135, 135)]
        [TestCase(-135, 225)]
        [TestCase(789, 69)]
        [TestCase(-789, 291)]
        public void GetNormalizeDegree(float degree, float expected)
        {
            float actual = EulerAnglesUtility.GetNormalizeDegree(degree);

            Assert.AreApproximatelyEqual(
                expected,
                actual,
                Tolerance,
                $"Normalized degree is not '{expected}'.");
        }

        [TestCase(80, -80, 135, 80, 280, 135)]
        [TestCase(789, -789, 360, 69, 291, 0)]
        public void GetNormalized(
            float eulerAngleX,
            float eulerAngleY,
            float eulerAngleZ,
            float expectedX,
            float expectedY,
            float expectedZ)
        {
            Vector3 eulerAngles = new Vector3(eulerAngleX, eulerAngleY, eulerAngleZ);
            Vector3 expected = new Vector3(expectedX, expectedY, expectedZ);

            Vector3 actual = EulerAnglesUtility.GetNormalized(eulerAngles);

            Assert.AreApproximatelyEqual(
                expected.x,
                actual.x,
                Tolerance,
                $"X-axis degree is not '{expected}'.");

            Assert.AreApproximatelyEqual(
                expected.y,
                actual.y,
                Tolerance,
                $"Y-axis degree is not '{expected}'.");

            Assert.AreApproximatelyEqual(
                expected.z,
                actual.z,
                Tolerance,
                $"Z-axis degree is not '{expected}'.");
        }

        [TestCase(10, 0, 60, true)]
        [TestCase(0, 0, 60, true)]
        [TestCase(60, 0, 60, true)]
        [TestCase(-10, 0, 60, false)]
        [TestCase(70, 0, 60, false)]
        [TestCase(370, 0, 60, true)]
        [TestCase(50, 45, 90, true)]
        [TestCase(310, 45, 90, false)]
        [TestCase(450, 45, 90, true)]
        [TestCase(180, 45, 90, false)]
        [TestCase(270, 45, 90, false)]
        [TestCase(360, 45, 90, false)]
        [TestCase(10, -45, 45, true)]
        [TestCase(370, -45, 45, true)]
        [TestCase(315, -45, 45, true)]
        [TestCase(310, -45, 45, false)]
        [TestCase(90, -45, 45, false)]
        [TestCase(-360, -45, 45, true)]
        [TestCase(360, -45, 45, true)]
        [TestCase(180, 135, 45, true)]
        [TestCase(60, 135, 45, false)]
        [TestCase(395, 135, 45, true)]
        [TestCase(495, 135, 45, true)]
        [TestCase(465, 135, 45, false)]
        [TestCase(500, 135, 45, true)]
        public void IsInRange(
            float currentDegree,
            float minDegree,
            float maxDegree,
            bool expected)
        {
            bool result = EulerAnglesUtility.IsInRange(currentDegree, minDegree, maxDegree);

            Assert.AreEqual(expected, result, $"The result isn't correct. currentDegree: {currentDegree}, minDegree: {minDegree}, maxDegree: {maxDegree}");
        }
    }
}
