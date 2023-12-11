using System.Collections.Generic;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;

namespace TPFive.Game.App.Entry
{
    using System;
    using TPFive.Game;

    /// <summary>
    /// The first setting of app entry.
    /// </summary>
    [CreateAssetMenu(fileName = "Settings - Entry", menuName = "TPFive/App/Settings - Entry")]
    public class Settings : ScriptableObject
    {
        /// <summary>
        /// The title of the app scene.
        /// </summary>
#if ODIN_INSPECTOR
        [BoxGroup("Settings - General")]
#endif
        public string title;

        /// <summary>
        /// The category of the app scene.
        /// </summary>
#if ODIN_INSPECTOR
        [BoxGroup("Settings - General")]
#endif
        public string category;

        // TODO: This might be removed.

        /// <summary>
        /// Flag indicates if loading scene property list or not.
        /// </summary>
#if ODIN_INSPECTOR
        [BoxGroup("Settings - LayerScene")]
#endif
        public bool loadByScenePropertyList;

        /// <summary>
        /// Some settings for overview.
        /// </summary>
#if ODIN_INSPECTOR
        [BoxGroup("Settings - Overview")]
#endif
        public OverviewSettings overviewSettings;

        /// <summary>
        /// This list contains scenes that should be loaded after setup done.
        /// </summary>
#if ODIN_INSPECTOR
        [BoxGroup("Settings - LayerScene")]
#endif
        // Change to public directly as access from editor side using reflection adds unnecessary complexity.
        // Besides, this data is config use, so it's fine to be public.
        public PlatformGroupSceneList[] sceneLists;
        public ProfileSysSetting profileSystemSetting;

        public List<SceneProperty> ScenePropertyList => GetSceneProperties(GameApp.PlatformGroup);

        public List<SceneProperty> GetSceneProperties(PlatformGroup platformGroup)
        {
            foreach (var sceneList in sceneLists)
            {
                if (sceneList.PlatformGroup == platformGroup)
                {
                    return sceneList.SceneList.SceneProperties;
                }
            }

            return null;
        }

        public bool TryGetSceneProperty(string title, out SceneProperty property)
        {
            var index = ScenePropertyList.FindIndex(x =>
            {
                return x.title.Equals(title, StringComparison.Ordinal);
            });
            property = index != -1 ? ScenePropertyList[index] : null;
            return property != null;
        }

        [Serializable]
        public class PlatformGroupSceneList
        {
            [SerializeField]
            private PlatformGroup platformGroup;
            [SerializeField]
            private ScenePropertyList sceneList;

            public PlatformGroup PlatformGroup => platformGroup;

            public ScenePropertyList SceneList => sceneList;
        }
    }

    /// <summary>
    /// Overview settings.
    /// </summary>
    [System.Serializable]
    public class OverviewSettings
    {
        public ScriptableObject config;
        public ScriptableObject hud;
    }
}
