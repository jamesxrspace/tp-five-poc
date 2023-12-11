using System;
using UnityEngine;
using UnityEngine.Timeline;

namespace TPFive.Game.Avatar.Motion
{
    [Serializable]
    public class MotionItem : IMotionItem
    {
        [SerializeField]
        private SerializableGuid uid;

        [SerializeField]
        private TimelineAsset asset;

        public Guid Uid => uid;

        public TimelineAsset Asset => asset;
    }
}