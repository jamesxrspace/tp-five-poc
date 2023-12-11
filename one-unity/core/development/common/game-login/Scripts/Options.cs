using System;
using UnityEngine;

namespace TPFive.Game.Login
{
    [Serializable]
    public sealed class Options
    {
        [SerializeField]
        private string appId;

        public string AppId => appId;
    }
}
