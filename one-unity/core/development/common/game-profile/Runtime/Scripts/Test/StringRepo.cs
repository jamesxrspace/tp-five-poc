using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TPFive.Game.Profile.Test
{
    public class StringRepo : MonoBehaviour
    {
        private const int StringLength = 1048576;
        [SerializeField]
        private TextMeshProUGUI text;
        private IList<string> strings = new List<string>();

        public void Add()
        {
            strings.Add(new string('M', StringLength));
            text.text = $"{strings.Count}";
        }

        public void Remove()
        {
            if (strings.Count == 0)
            {
                return;
            }

            strings.RemoveAt(strings.Count - 1);
            Resources.UnloadUnusedAssets();
            GC.Collect();
            text.text = $"{strings.Count}";
        }

        protected void Start()
        {
            text.text = $"{strings.Count}";
        }
    }
}