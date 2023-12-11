using Fusion;
using UnityEngine;

namespace TPFive.Room
{
    [CreateAssetMenu(fileName = "RoomSetting", menuName = "RoomSetting", order = 1)]
    public class RoomSetting : ScriptableObject
    {
        private const int MinRetryStartGameInterval = 5; // in seconds

        [SerializeField]
        private int serverTargetFrameRate;
        [SerializeField]
        private NetworkPrefabRef netSceneCtrlPrefab;
        [SerializeField]
        private NetworkPrefabRef fusionTelemeterPrefab;
        [SerializeField]
        private bool customAuthentication;
        [SerializeField]
        private NetworkPrefabRef playerDataBrokerPrefab;
        [SerializeField]
        private NetworkPrefabRef playerPrefab;
        [Min(MinRetryStartGameInterval)]
        [SerializeField]
        private int retryStartGameInterval;

        public int ServerTargetFrameRate { get => serverTargetFrameRate; set => serverTargetFrameRate = value; }

        public NetworkPrefabRef NetSceneCtrlPrefab { get => netSceneCtrlPrefab; set => netSceneCtrlPrefab = value; }

        public NetworkPrefabRef FusionTelemeterPrefab { get => fusionTelemeterPrefab; set => fusionTelemeterPrefab = value; }

        public bool CustomAuthentication { get => customAuthentication; set => customAuthentication = value; }

        public NetworkPrefabRef PlayerDataBrokerPrefab => playerDataBrokerPrefab;

        public NetworkPrefabRef PlayerPrefab => playerPrefab;

        public int RetryStartGameInterval => Mathf.Max(MinRetryStartGameInterval, retryStartGameInterval);
    }
}