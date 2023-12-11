using System;
using UnityEngine;
using UnityEngine.UI;

namespace TPFive.Game.Record.Entry
{
    [Serializable]
    public class UnityUIState
    {
        [SerializeField]
        private UINameTag id;

        [SerializeField]
        private Selectable uiElement;

        public UINameTag Id
        {
            get => id;
            set => id = value;
        }

        public Selectable UIElement
        {
            get => uiElement;
            set => uiElement = value;
        }
    }
}
