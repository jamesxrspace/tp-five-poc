using System;
using System.Collections.Generic;
using TPFive.Model;
using UnityEngine;

namespace TPFive.Game.Record.Entry
{
    [Serializable]
    public class WindowUIState
    {
        [SerializeField]
        private RecordStateTypeEnum state;

        [SerializeField]
        private List<GameObjectActiveSetting> settings;

        public RecordStateTypeEnum State => state;

        public void Apply()
        {
            settings.ForEach(x => x.Apply());
        }
    }
}
