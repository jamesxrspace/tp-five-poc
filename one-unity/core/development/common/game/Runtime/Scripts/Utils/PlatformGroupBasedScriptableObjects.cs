using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TPFive.Game
{
    public abstract class PlatformGroupBasedSetting<T>
        : ScriptableObject
        where T : Object
    {
        [SerializeField]
        private PlatformGroup[] platformGroups;
        [SerializeField]
        private AssetInfo[] assets;

        public T GetAsset(PlatformGroup platformGroup)
        {
            if (platformGroups == null || assets == null)
            {
                return default;
            }

            var index = Array.IndexOf(platformGroups, platformGroup);
            return index >= 0 && index < assets.Length
                ? assets[index].GetAsset(false) as T
                : default;
        }

        public T GetAsset(PlatformGroup platformGroup, bool isEditor)
        {
            if (platformGroups == null || assets == null)
            {
                return default;
            }

            var index = Array.IndexOf(platformGroups, platformGroup);
            return index >= 0 && index < assets.Length
                ? assets[index].GetAsset(isEditor) as T
                : default;
        }

        [Serializable]
        private class AssetInfo
        {
            [SerializeField]
            private T runtimeAsset;
            [SerializeField]
            private T editorAsset;

            public Object GetAsset(bool isEditor) => isEditor ? editorAsset : runtimeAsset;
        }
    }
}
