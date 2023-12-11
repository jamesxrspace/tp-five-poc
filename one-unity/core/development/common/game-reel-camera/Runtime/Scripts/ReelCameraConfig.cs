using System;
using System.Collections.Generic;
using UnityEngine;

namespace TPFive.Game.Reel.Camera
{
    [Serializable]
    public class ReelCameraConfig
    {
        [SerializeField]
        private string name;

        [SerializeField]
        private List<ReelCameraTag> cameraTags;

        [SerializeField]
        private BaseReelCamera camera;

        public string Name => name;

        public BaseReelCamera Camera => camera;

        public IReadOnlyList<ReelCameraTag> CameraTags => cameraTags;
    }
}
