using System;
using UnityEngine;
using XR.AvatarEditing;

namespace TPFive.Game.AvatarEdit.Entry
{
    [CreateAssetMenu(fileName = "Avatar Edit Settings", menuName = "TPFive/Avatar/Create Edit Settings")]

    public sealed class AvatarEditSettings : ScriptableObject
    {
        [SerializeField]
        private string _avatarLayerName;
        [SerializeField]
        private RenderTexture _avatarRenderTexture;
        [SerializeField]
        private AvatarTypeEnum[] _types;
        [SerializeField]
        private RuntimeAnimatorController[] _controllers;

        [Header("Preset Avatar")]
        [SerializeField]
        private TextAsset _presetAvatarJsonAsset;
        [SerializeField]
        private string _presetIdleImagePathFormat;
        [SerializeField]
        private string _presetSelectedImagePathFormat;

        [Header("Detail Editor")]
        [SerializeField]
        private string _appearancesStylePath;
        [SerializeField]
        private string _apparelsStylePath;
        [SerializeField]
        private string _makeupStylePath;

        public string PresetAvatarJson => _presetAvatarJsonAsset != null ? _presetAvatarJsonAsset.text : "{}";

        public RenderTexture AvatarRenderTexture => _avatarRenderTexture;

        public string AvatarLayerName => _avatarLayerName;

        public string PresetIdleImagePathFormat => _presetIdleImagePathFormat;

        public string PresetSelectedImagePathFormat => _presetSelectedImagePathFormat;

        public string AppearancesStylePath => _appearancesStylePath;

        public string ApparelsStylePath => _apparelsStylePath;

        public string MakeupStylePath => _makeupStylePath;

        public RuntimeAnimatorController GetAnimatorController(AvatarType type)
        {
            var index = Array.IndexOf(_types, (AvatarTypeEnum)type);
            return index >= 0 && index < _controllers.Length ? _controllers[index] : null;
        }

        [Serializable]
        private class AvatarTypeEnum : StableEnum<AvatarType>
        {
            public static implicit operator AvatarTypeEnum(AvatarType value)
            {
                return new () { _value = value };
            }
        }
    }
}