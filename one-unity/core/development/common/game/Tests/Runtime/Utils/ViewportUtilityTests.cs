using NUnit.Framework;
using UnityEngine;

namespace TPFive.Game.Utils.Tests
{
    public class ViewportUtilityTests
    {
        [TestCase(1920, 1080, 1024, 1024, 1024, 576)]
        [TestCase(1080, 1920, 1024, 1024, 576, 1024)]
        [TestCase(720, 720, 1024, 1024, 1024, 1024)]
        [TestCase(2048, 2048, 1024, 1024, 1024, 1024)]
        public void TestTransformViewport(
            int contentX, int contentY,
            int targetX, int targetY,
            int expectedX, int expectedY)
        {
            Assert.AreEqual(new Vector2(expectedX, expectedY),
                ViewportUtility.TransformViewport(
                    new Vector2(contentX, contentY), new Vector2(targetX, targetY)));
        }

        [TestCase(1920, 1080, 1024, 1024, 1000f, 1778f, 0, -389f)]
        [TestCase(1080, 1920, 1024, 1024, 1778f, 1000f, -389f, 0)]
        [TestCase(720, 720, 1024, 1024, 1000f, 1000f, 0, 0)]
        [TestCase(2048, 2048, 1024, 1024, 1000f, 1000f, 0, 0)]
        public void TestTransformViewportToUV(
            float contentX, float contentY,
            float targetX, float targetY,
            float expectedTilingX, float expectedTilingY,
            float expectedOffsetX, float expectedOffsetY)
        {
            var expectedTiling = new Vector2(expectedTilingX, expectedTilingY);
            var expectedOffset = new Vector2(expectedOffsetX, expectedOffsetY);

            var result = ViewportUtility.TransformViewportToUV(
                new Vector2(contentX, contentY), new Vector2(targetX, targetY));

            var roundUp = 1000f;
            result[0].x = Mathf.Round(result[0].x * roundUp);
            result[0].y = Mathf.Round(result[0].y * roundUp);
            result[1].x = Mathf.Round(result[1].x * roundUp);
            result[1].y = Mathf.Round(result[1].y * roundUp);

            Debug.Log($"{result[0].ToString("F3")}, {result[1].ToString("F3")}");

            Assert.AreEqual(expectedTiling, result[0]);
            Assert.AreEqual(expectedOffset, result[1]);
        }
    }
}