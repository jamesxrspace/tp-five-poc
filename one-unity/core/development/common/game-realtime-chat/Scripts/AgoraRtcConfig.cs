using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TPFive.Game.RealtimeChat
{
    [Serializable]
    public sealed class AgoraRtcConfig
    {
        [SerializeField]
        private string appId;

        public string AppId => appId;
    }
}
