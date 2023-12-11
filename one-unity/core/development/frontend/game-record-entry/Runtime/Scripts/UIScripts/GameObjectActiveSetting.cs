using System;
using UnityEngine;

namespace TPFive.Game.Record.Entry
{
    [Serializable]
    public class GameObjectActiveSetting
    {
        [SerializeField]
        private ActiveType activeType;

        [SerializeField]
        private GameObject gameObject;

        public enum ActiveType
        {
            /// <summary>
            /// Set the ui active
            /// </summary>
            Active,

            /// <summary>
            /// Set the ui deactive
            /// </summary>
            Deactive,

            /// <summary>
            /// Set the ui active when in editor
            /// </summary>
            Editor,
        }

        public void Apply()
        {
            var active = activeType switch
            {
                ActiveType.Active => true,
                ActiveType.Deactive => false,
                ActiveType.Editor => Application.isEditor,
                _ => throw new ArgumentOutOfRangeException(),
            };

            gameObject.SetActive(active);
        }
    }
}
