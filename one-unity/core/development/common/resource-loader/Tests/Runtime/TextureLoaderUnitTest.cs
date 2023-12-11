using System.Collections;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using TPFive.Game.Resource;
using UnityEngine;
using UnityEngine.TestTools;
using Debug = UnityEngine.Debug;

namespace TPFive.Extended.ResourceLoader.Tests
{
    public class TextureLoaderUnitTest
    {
        private const string TestUrl = "https://d10cttm21ldbr4.cloudfront.net/space/thumbnail/0989010f5368ff08a92037dab9cdbdb65e281e550a9bc378eaaeb757f770cc47.png";
        private TextureManager textureManager;
        private Texture2D loadedTexture;

        [SetUp]
        public void Setup()
        {
            var go = new GameObject("TextureManager");
            textureManager = go.AddComponent<TextureManager>();

            var loggerFactoryField = textureManager.GetType().BaseType.GetField("loggerFactory", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.FlattenHierarchy | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

            // Assert.NotNull(loggerFactoryField, "loggerFactoryField");
            var loggerFactory = new LoggerFactory();
            loggerFactoryField.SetValue(textureManager, loggerFactory);
        }

        [UnityTest]
        public IEnumerator TestLoadTexture()
        {
            var isDone = false;
            var context = new TextureRequestContext()
            {
                MaxRetry = 3,
                Url = TestUrl,
            };

            var request = new TextureRequest(this, context, callback: (result) =>
            {
                isDone = true;

                loadedTexture = result.Texture;

                Assert.NotNull(result, "Texture Data");
                Assert.NotNull(result.Texture, "Texture2D");
            });

            textureManager.Load(request);

            while (!isDone)
            {
                yield return null;
            }

            textureManager.Release(TestUrl, this);

            yield return new WaitForSeconds(1);
            Assert.IsTrue(loadedTexture == null, "ReleaseTexture");
        }
    }
}
