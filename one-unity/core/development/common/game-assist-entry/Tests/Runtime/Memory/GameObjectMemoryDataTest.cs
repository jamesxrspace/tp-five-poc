using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TPFive.Game.Assist.Entry;
using UnityEngine;

namespace TPFive.Game.Assist.Entry.Tests
{
    public class GameObjectMemoryDataTest
    {
        [Test]
        public void TestGetReport()
        {
            var go = new GameObject();
            var goMemoryData = GameObjectMemoryData.Record(go, true);
            var expected = @"== GO: New Game Object
==
";
            Assert.AreEqual(goMemoryData.ToReport(), expected);
        }

        [TestCase(0, "0.00 KB")]
        [TestCase(100, "0.10 KB")]
        [TestCase(10000, "9.77 KB")]
        [TestCase(1000000, "0.95 MB")]
        [Test]
        public void TestHumanReadable(long size, string expected)
        {
            var value = GameObjectMemoryData.HumanReadable((double)size);
            Assert.AreEqual(value, expected);
        }
    }
}
