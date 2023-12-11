using System.Collections.Generic;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;

namespace TPFive.Game.Assist.Entry
{
    [CreateAssetMenu(fileName = "Settings - Entry", menuName = "XSPO/Assist/Settings - Entry")]
    public class Settings : ScriptableObject
    {
#if ODIN_INSPECTOR
        [BoxGroup("Settings - General")]
#endif
        public string title;

#if ODIN_INSPECTOR
        [BoxGroup("Settings - General")]
#endif
        public string category;

#if ODIN_INSPECTOR
        [BoxGroup("Settings - LayerScene")]
#endif
        public bool loadByScenePropertyList;
    }
}
