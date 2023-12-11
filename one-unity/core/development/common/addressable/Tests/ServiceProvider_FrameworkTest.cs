using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using NUnit.Framework;
using TPFive.Extended.Addressable;
using UnityEngine;
using UnityEngine.TestTools;

namespace TPFive.Extended.Addressable.Tests
{
    public class ServiceProvider_FrameworkTest
    {
        [Test]
        public void TestInternalIdTransformFunc()
        {
            MethodInfo methodInfo = typeof(ServiceProvider).GetMethod(
                "TransformInternalId",
                BindingFlags.NonPublic | BindingFlags.Static);
            object[] parameters =
            {
                "«PrefixPath»/foo/«IntermediatePath»/bar/baz.bundle",
                "http://dummy-location",
            };
            var result = methodInfo.Invoke(null, parameters);
            Assert.AreEqual(result, "http://dummy-location/foo/latest/bar/baz.bundle");
        }
    }
}
