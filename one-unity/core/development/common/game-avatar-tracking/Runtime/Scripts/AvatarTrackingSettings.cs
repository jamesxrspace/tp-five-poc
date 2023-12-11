using System;
using UnityEngine;

namespace TPFive.Game.Avatar.Tracking
{
    [Serializable]
    public class AvatarTrackingSettings
    {
        [SerializeField]
        private AvatarMask upperBodyMask;

        [SerializeField]
        private float layerFadeDuration;

        public AvatarMask UpperBodyMask => upperBodyMask;

        public float LayerFadeDuration => layerFadeDuration;
    }
}