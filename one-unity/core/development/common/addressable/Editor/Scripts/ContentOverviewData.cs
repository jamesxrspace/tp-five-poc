using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace TPFive.Extended.Addressable.Editor
{
    [CreateAssetMenu(fileName = "Content Overview Data", menuName = "TPFive/Extended/Addressable/Content Overview Data")]
    public class ContentOverviewData : ScriptableObject
    {
        [SerializeField]
        private List<FileContent> unitypackageList;

        public List<FileContent> UnitypackageList => unitypackageList;
    }
}
