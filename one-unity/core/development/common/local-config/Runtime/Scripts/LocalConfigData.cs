using System.Collections.Generic;
using UnityEngine;

namespace TPFive.Extended.LocalConfig
{
    // [TPFive.DevFoundation.AddToString.Abstractions.AddToString]
    [CreateAssetMenu(fileName = "Local Config Data", menuName = "XSPO/Config/Local Config Data")]
    public class LocalConfigData : ScriptableObject
    {
        [SerializeField]
        private List<ItemString> itemStringList;

        public List<ItemString> ItemStringList => itemStringList;
    }

    [System.Serializable]
    public class ItemString
    {
        [SerializeField]
        private string key;

        [SerializeField]
        private string value;

        public string Key => key;

        public string Value => value;
    }
}
