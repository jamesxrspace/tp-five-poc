using System;
using UnityEngine;

namespace TPFive.Game.Hud
{
    [CreateAssetMenu(fileName = "UIMultiPlatformConfiguration", menuName = "TPFive/UI/UIMultiPlatformConfiguration")]
    public class UIMultiPlatformConfiguration : UIConfigurationBase
    {
        [Serializable]
        private class RuntimePlatformStableEnum : StableEnum<RuntimePlatform>
        {
            public static implicit operator RuntimePlatformStableEnum(RuntimePlatform value)
            {
                return new () { _value = value };
            }
        }

        [Serializable]
        private class PlatformDetails
        {
            [SerializeField]
            RuntimePlatformStableEnum platform;
            [SerializeReference]
            UIConfigurationBase configuration;

            public RuntimePlatform Platform => platform;
            public UIConfigurationBase Configuration => configuration;
        }

        [SerializeReference]
        UIConfigurationBase defaultConfiguration;
        [SerializeField]
        PlatformDetails[] details;

        public IUIConfiguration DefaultConfiguration => defaultConfiguration;

        public override string GetRootDirName()
        {
            var config = GetConfiguration();
            return config != null ? config.GetRootDirName() : string.Empty;
        }

        public IUIConfiguration GetConfiguration()
        {
            return TryGetConfiguration(GameApp.RuntimePlatform, out var config) ? config : defaultConfiguration;
        }

        public bool TryGetConfiguration(RuntimePlatform platform, out IUIConfiguration config)
        {
            if (details == null || details.Length == 0)
            {
                config = null;
                return false;
            }

            var index = Array.FindIndex(details, d => d.Platform == platform);
            if (index >= 0 && index < details.Length)
            {
                config = details[index].Configuration;
                return true;
            }

            config = null;
            return false;
        }
    }
}
