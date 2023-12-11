using UnityEngine;

namespace TPFive.Creator
{
    [System.Serializable]
    public class Settings
    {
        [SerializeField]
        private ScriptableObject _hud;

        public ScriptableObject Hud => _hud;
    }
}