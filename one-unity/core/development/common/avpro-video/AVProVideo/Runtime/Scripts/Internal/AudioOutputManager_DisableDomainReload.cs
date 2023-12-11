using UnityEngine;

namespace RenderHeads.Media.AVProVideo
{
    public partial class AudioOutputManager
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnSubsystemRegistration()
        {
            _instance = null;
        }
    }
}

