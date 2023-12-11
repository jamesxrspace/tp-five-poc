using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Timeline;

namespace TPFive.Game.Avatar.Motion
{
    [CreateAssetMenu(fileName = "AvatarMotionCategory", menuName = "PersistentData/Create AvatarMotionCategory")]
    public class AvatarMotionCategory : ScriptableObject
    {
        [SerializeField]
        private string categoryName;

        [SerializeField]
        private MotionItem[] motions;

        public string CategoryName => categoryName;

        public MotionItem[] Motions => motions ?? Array.Empty<MotionItem>();

        public Dictionary<Guid, TimelineAsset> GetMotionDict()
        {
            return Motions.ToDictionary(i => i.Uid, i => i.Asset);
        }
    }
}