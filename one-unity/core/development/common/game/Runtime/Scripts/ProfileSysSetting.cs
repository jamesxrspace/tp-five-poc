using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TPFive.Game
{
    [CreateAssetMenu(fileName = "ProfileSetting", menuName = "TPFive/Profile/Setting")]
    public class ProfileSysSetting : ScriptableObject
    {
        [SerializeField]
        private AssetReferenceGameObject statsMonitorPrefab;

        public AssetReferenceGameObject StatsMonitorPrefab => statsMonitorPrefab;
    }
}
