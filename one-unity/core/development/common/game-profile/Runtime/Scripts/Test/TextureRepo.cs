using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TPFive.Game.Profile.Test
{
    public class TextureRepo : MonoBehaviour
    {
        [SerializeField]
        private Texture2D srcTexture;
        [SerializeField]
        private TextMeshProUGUI text;
        private IList<Texture2D> textures = new List<Texture2D>();

        public void Add()
        {
            Texture2D clone = new Texture2D(srcTexture.width, srcTexture.height, srcTexture.format, srcTexture.mipmapCount, true);
            Graphics.CopyTexture(srcTexture, clone);
            textures.Add(clone);
            text.text = $"{textures.Count}";
        }

        public void Remove()
        {
            if (textures.Count == 0)
            {
                return;
            }

            textures.RemoveAt(textures.Count - 1);
            Resources.UnloadUnusedAssets();
            GC.Collect();
            text.text = $"{textures.Count}";
        }

        protected void Start()
        {
            text.text = $"{textures.Count}";
        }
    }
}