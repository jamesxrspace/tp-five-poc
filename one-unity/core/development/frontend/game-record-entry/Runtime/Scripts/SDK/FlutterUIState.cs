using System;
using UnityEngine;

namespace TPFive.Game.Record.Entry
{
    [Serializable]
    public class FlutterUIState
    {
        [SerializeField]
        private UINameTag id;

        [SerializeField]
        private UIState state;

        public UINameTag Id
        {
            get => id;
            set => id = value;
        }

        public UIState State
        {
            get => state;
            set => state = value;
        }
    }
}
