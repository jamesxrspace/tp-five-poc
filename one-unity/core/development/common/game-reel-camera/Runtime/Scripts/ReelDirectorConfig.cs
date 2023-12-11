using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;

namespace TPFive.Game.Reel.Camera
{
    [CreateAssetMenu(fileName = "ReelDirectorConfig", menuName = "TPFive/Record/Reel Director Config")]
    [Serializable]
    public class ReelDirectorConfig : ScriptableObject
    {
        [SerializeField]
        private List<ReelCameraConfig> randomCameraDataList;

        [SerializeField]
        private List<ReelCameraConfig> presetTrackDataList;

        [SerializeField]
        private List<ReelCameraTag> cameraTags;

        [SerializeField]
        private CinemachineBlendDefinition blendDefinition;

        public List<ReelCameraConfig> RandomCameraDataList => randomCameraDataList;

        public List<ReelCameraConfig> PresetTrackDataList => presetTrackDataList;

        public IReadOnlyList<ReelCameraTag> Tags => cameraTags;

        public CinemachineBlendDefinition BlendDefinition => blendDefinition;

        public ReelCameraTag GetReelCameraTag(string tagName)
        {
            return Tags.FirstOrDefault(tag => tag.CompareTagName(tagName));
        }
    }
}
