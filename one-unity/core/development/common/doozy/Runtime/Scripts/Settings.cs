namespace TPFive.Extended.Doozy
{
#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Settings", menuName = "XSPO/Hud/Doozy/Settings")]
    public class Settings : ScriptableObject
    {
#if ODIN_INSPECTOR
        [BoxGroup("Signal")]
#endif
        public List<SignalBindingData> signalBindingDataList;

    }
}
