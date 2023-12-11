using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TPFive.Game.Avatar.Factory
{
    [CreateAssetMenu(fileName = "AvatarFactorySettings", menuName = "TPFive/Avatar/Create Avatar Factory Settings")]
    public sealed class AvatarFactorySettings : ScriptableObject
    {
        [SerializeField]
        private AvatarFactoryPreset[] presets;

        [SerializeField]
        private Category[] categories;

        [SerializeField]
        [Min(1)]
        private int maxDownloadRetries;

        [SerializeField]
        [Min(1)]
        [Tooltip("Maximum time in seconds we wait to establish the connection to the target server")]
        private int connectTimeout;

        [SerializeField]
        [Min(1)]
        [Tooltip("Maximum time in seconds we want to wait to the request to finish after the connection is established")]
        private int requetTimeout;

        private Dictionary<string, AvatarFactoryPreset> presetDict;
        private Dictionary<int, string> categoryDict;

        public int MaxDownloadRetries => maxDownloadRetries;

        public int ConnectTimeout => connectTimeout;

        public int RequetTimeout => requetTimeout;

        private Dictionary<string, AvatarFactoryPreset> PresetDict => presetDict ?? CreatePresetDict();

        private Dictionary<int, string> CategoryDict => categoryDict ?? CreateCategoryDict();

        public AvatarFactoryPreset GetFirstPreset()
        {
            if (presets == null || presets.Length == 0)
            {
                return null;
            }

            return presets[0];
        }

        public AvatarFactoryPreset GetPreset(string name)
        {
            PresetDict.TryGetValue(name, out var preset);
            return preset;
        }

        public string GetCategoryName(int gender)
        {
            CategoryDict.TryGetValue(gender, out var category);
            return category;
        }

        private Dictionary<string, AvatarFactoryPreset> CreatePresetDict()
        {
            if (presets == null || presets.Length == 0)
            {
                presetDict = new Dictionary<string, AvatarFactoryPreset>();
            }
            else
            {
                presetDict = presets.ToDictionary(x => x.PresetName, x => x, StringComparer.Ordinal);
            }

            return presetDict;
        }

        private Dictionary<int, string> CreateCategoryDict()
        {
            if (categories == null || categories.Length == 0)
            {
                categoryDict = new Dictionary<int, string>();
            }
            else
            {
                categoryDict = categories.ToDictionary(x => x.Gender, x => x.Name);
            }

            return categoryDict;
        }

        [Serializable]
        private struct Category
        {
            public int Gender;
            public string Name;
        }
    }
}