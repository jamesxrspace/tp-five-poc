using NUnit.Framework;

namespace TPFive.Game.Utils.Tests
{
    public class MathUtilityTests
    {
        // Negative <---> Positive
        [TestCase(-10, -9, 9, false)]
        [TestCase(-9, -9, 9, true)]
        [TestCase(-5, -9, 9, true)]
        [TestCase(0, -9, 9, true)]
        [TestCase(5, -9, 9, true)]
        [TestCase(9, -9, 9, true)]
        [TestCase(10, -9, 9, false)]

        // 0 <---> Positive
        [TestCase(-10, 0, 9, false)]
        [TestCase(-9, 0, 9, false)]
        [TestCase(-5, 0, 9, false)]
        [TestCase(0, 0, 9, true)]
        [TestCase(5, 0, 9, true)]
        [TestCase(9, 0, 9, true)]
        [TestCase(10, 0, 9, false)]

        // Negative <---> 0
        [TestCase(-10, -9, 0, false)]
        [TestCase(-9, -9, 0, true)]
        [TestCase(-5, -9, 0, true)]
        [TestCase(0, -9, 0, true)]
        [TestCase(5, -9, 0, false)]
        [TestCase(9, -9, 0, false)]
        [TestCase(10, -9, 0, false)]

        // Positive <---> Positive
        [TestCase(-10, 5, 9, false)]
        [TestCase(-9, 5, 9, false)]
        [TestCase(-5, 5, 9, false)]
        [TestCase(0, 5, 9, false)]
        [TestCase(5, 5, 9, true)]
        [TestCase(9, 5, 9, true)]
        [TestCase(10, 5, 9, false)]

        // Negative <---> Negative
        [TestCase(-10, -9, -5, false)]
        [TestCase(-9, -9, -5, true)]
        [TestCase(-5, -9, -5, true)]
        [TestCase(0, -9, -5, false)]
        [TestCase(5, -9, -5, false)]
        [TestCase(9, -9, -5, false)]
        [TestCase(10, -9, -5, false)]
        public void InRange(float curtValue, float minValue, float maxValue, bool expected)
        {
            Assert.AreEqual(expected, MathUtility.IsInRange(curtValue, minValue, maxValue));
        }
    }
}