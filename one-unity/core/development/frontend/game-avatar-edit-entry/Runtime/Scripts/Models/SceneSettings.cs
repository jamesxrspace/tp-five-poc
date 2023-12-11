using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace TPFive.Game.AvatarEdit.Entry
{
    [Serializable]
    public sealed class SceneSettings
    {
        [SerializeField]
        private Transform _modelRoot;
        [SerializeField]
        private GameObject _waitingPanel;

        public Transform ModelRoot => _modelRoot;

        public GameObject WaitingPanel => _waitingPanel;
    }
}