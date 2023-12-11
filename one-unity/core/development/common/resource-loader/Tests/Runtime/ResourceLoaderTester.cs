using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TPFive.Game.Resource;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using VContainer;

namespace TPFive.Extended.ResourceLoader.Tests
{
    /// <summary>
    /// This script aim to provide a shortcut to setup a ugui to load texture and see texture result "manually".
    /// Refer to another script called "TextureLoaderUnitTest.cs" to perform unit test.
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(VerticalLayoutGroup))]
    public class ResourceLoaderTester : MonoBehaviour
    {
        [SerializeField]
        private int maxRetry = 3;
        [SerializeField]
        private int timeoutSecond = 10;
        [SerializeField]
        private string[] testUrls = new string[]
        {
            "https://d10cttm21ldbr4.cloudfront.net/space/internal/APTG_Auditorium_B.jpg",
            "https://d10cttm21ldbr4.cloudfront.net/space/internal/APTG_Auditorium_C.jpg",
            "https://d10cttm21ldbr4.cloudfront.net/space/internal/APTG_Auditorium_D.jpg",
            "https://d10cttm21ldbr4.cloudfront.net/space/internal/APTG_Auditorium_E.jpg",
        };

        [SerializeField]
        private List<string> loadedUrls = new List<string>();

        private IService resourceLoaderService;

        [Inject]
        public void Construct(IService resourceLoaderService)
        {
            this.resourceLoaderService = resourceLoaderService;
            Assert.IsNotNull(this.resourceLoaderService);
        }

        protected void Start()
        {
            var canvas = this.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            loadedUrls.Clear();
        }

        protected void OnGUI()
        {
            if (GUILayout.Button("Load one texture"))
            {
                var url = testUrls[0];

                // create a new ui view
                var rawImage = CreateImageView(url);

                // load texture and show result
                LoadTexture(url, rawImage);
            }

            if (GUILayout.Button("Load multiple texture"))
            {
                foreach (var url in testUrls)
                {
                    // create a new ui view
                    var rawImage = CreateImageView(url);

                    // load texture and show result
                    LoadTexture(url, rawImage).Forget();
                }
            }

            for (int i = loadedUrls.Count - 1; i >= 0; i--)
            {
                string url = loadedUrls[i];
                if (GUILayout.Button($"Release {url}"))
                {
                    resourceLoaderService.ReleaseTexture(url, this);
                    loadedUrls.RemoveAt(i);
                }
            }
        }

        private RawImage CreateImageView(string url)
        {
            var go = new GameObject(url);
            go.transform.SetParent(transform);
            var image = go.AddComponent<RawImage>();
            return image;
        }

        private async UniTaskVoid LoadTexture(string url, RawImage rawImage)
        {
            var resourceRequestContext = new TextureRequestContext()
            {
                Url = url,
                MaxRetry = maxRetry,
                Timeout = TimeSpan.FromSeconds(timeoutSecond),
            };

            var cts = new CancellationTokenSource();
            var data = await resourceLoaderService.LoadTexture(this, resourceRequestContext, cts.Token);

            if (data != null)
            {
                rawImage.texture = data.Texture;
                loadedUrls.Add(url);
            }
            else
            {
                Debug.LogWarning($"TextureRequest.callback : texture is null");
            }
        }
    }
}
