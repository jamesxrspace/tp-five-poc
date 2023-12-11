using UnityEngine;

namespace TPFive.Extended.Addressable.Editor
{
    [System.Serializable]
    public class FileContent
    {
        [SerializeField]
        private string title;

        [SerializeField]
        private string id;

        [SerializeField]
        private string version;

        [SerializeField]
        private bool toBeImported;

        public string Title => title;

        public string Id
        {
            get => id;
            set => id = value;
        }

        public string Version
        {
            get => version;
            set => version = value;
        }

        public bool ToBeImported
        {
            get => toBeImported;
            set => toBeImported = value;
        }
    }
}