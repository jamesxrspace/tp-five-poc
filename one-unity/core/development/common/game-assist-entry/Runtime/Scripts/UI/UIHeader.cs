using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TPFive.Game.Assist.Entry
{
    public class UIHeader : UITextPanel
    {
        private static UIHeader instance;

        public static UIHeader Instance
        {
            get
            {
                return instance;
            }
        }

        protected override Vector2 PanelSize => new Vector2(200f, 100f);

        public void Show(string line)
        {
            if (showUI && !uiCreated)
            {
                CreateUI();
            }

            uiText.text = line;
        }

        protected void OnEnable()
        {
            instance = this;
        }

        protected void OnDisable()
        {
            if (instance == this)
            {
                instance = null;
            }
        }
    }
}
