using System;
using UnityEngine;

namespace TPFive.Game.Record.Entry
{
    [CreateAssetMenu(fileName = "RecordSetting", menuName = "TPFive/Record/RecordSetting")]
    [Serializable]
    public class RecordSetting : ScriptableObject
    {
        [SerializeField]
        private GameObject localAvatarPrefab;

        public GameObject LocalAvatarPrefab => localAvatarPrefab;
    }
}
