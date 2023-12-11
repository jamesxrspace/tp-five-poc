// It is easier for auto filling these values while generating the settings.
// Using reflection will make the process more complicated.
#pragma warning disable SA1307
#pragma warning disable SA1401

using System.Collections.Generic;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;

namespace TPFive.Creator.Entry
{
    using TPFive.Game;

    [CreateAssetMenu(fileName = "Settings - Entry", menuName = "TPFive/Creator/Settings - Entry")]
    public class Settings : ScriptableObject
    {
        public string levelBundleId;

#if ODIN_INSPECTOR
        [BoxGroup("Settings - General")]
#endif
        public string title;

#if ODIN_INSPECTOR
        [BoxGroup("Settings - General")]
#endif
        public string category;

        public int categoryOrder;

#if ODIN_INSPECTOR
        [BoxGroup("Settings - LayerScene")]
#endif
        public bool loadByScenePropertyList;

#if ODIN_INSPECTOR
        [BoxGroup("Settings - Assist")]
#endif
        public bool useAssist;

        private OverviewSettings overviewSettings;

        public string Title => title;

        public string Category => category;

        public bool LoadByScenePropertyList => loadByScenePropertyList;

        public int CategoryOrder => categoryOrder;

        public bool UseAssist => useAssist;

        public OverviewSettings OverviewSettings => overviewSettings;
    }

    [System.Serializable]
    public class OverviewSettings
    {
        public ScriptableObject config;
        public ScriptableObject hud;

        public ScriptableObject Config => config;

        public ScriptableObject Hud => hud;
    }
}

#pragma warning restore SA1401
#pragma warning restore SA1307
