using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TPFive.Game.Assist.Entry;
using UnityEngine;

namespace TPFive.Game.Assist.Entry.Tests
{
    public class MemoryDataTest
    {
        [Test]
        public void TestGetRuntimeMemorySizeLong()
        {
            var go = new GameObject();
            var size = MemoryData.GetRuntimeMemorySizeLong(go);
            Assert.Greater(size, 0);
        }
    }
}
