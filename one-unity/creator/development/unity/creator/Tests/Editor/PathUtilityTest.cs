using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace TPFive.Creator.Editor.Tests
{
    public class PathUtilityTest
    {
        [SetUp]
        public void Setup()
        {
        }
        
        [Test]
        public void ReturnCorrectIndex()
        {
            var path = "Assets/_/1 - Game/Decorations/Bundle - D001/3ab1d6bb-4588-4944-9d80-769525d855ee.asset";
            var (index, adjustedPath) = Utility.GetAdjustedPathWithIndex(path);

            var expectedIndex = 4;
            var expectedAdjustedPath = "Assets/_/1 - Game/Decorations/Bundle - D001";

            Assert.AreEqual(expectedIndex, index);
            Assert.AreEqual(expectedAdjustedPath, adjustedPath);
        }

        [Test]
        public void ReturnInCorrectIndex()
        {
            var path = "Assets/3ab1d6bb-4588-4944-9d80-769525d855ee.asset";
            var (index, adjustedPath) = Utility.GetAdjustedPathWithIndex(path);

            var expectedIndex = -1;
            var expectedAdjustedPath = string.Empty;

            Assert.AreEqual(expectedIndex, index);
            Assert.AreEqual(expectedAdjustedPath, adjustedPath);
        }
    }
}