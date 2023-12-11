using UnityEngine;
#pragma warning disable SA1402

namespace TPFive.Game
{
    [System.Serializable]
    public class KeyIntValue
    {
        [SerializeField]
        private string key;
        [SerializeField]
        private int value;

        public string Key => key;

        public int Value => value;
    }

    [System.Serializable]
    public class KeyFloatValue
    {
        [SerializeField]
        private string key;
        [SerializeField]
        private float value;

        public string Key => key;

        public float Value => value;
    }

    [System.Serializable]
    public class KeyStringValue
    {
        [SerializeField]
        private string key;
        [SerializeField]
        private string value;

        public string Key => key;

        public string Value => value;
    }

    [System.Serializable]
    public class KeyScriptableObjectValue
    {
        [SerializeField]
        private string key;
        [SerializeField]
        private ScriptableObject value;

        public string Key => key;

        public ScriptableObject Value => value;
    }
}
#pragma warning restore SA1402