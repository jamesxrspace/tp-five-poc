using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TPFive.Creator.Editor
{
    /// <summary>
    /// Define what layer can be used.
    /// </summary>
    [CreateAssetMenu(fileName = "Mapping Layer Data", menuName = "TPFive/Creator/Mapping Layer Data")]
    public class MappingLayerData : ScriptableObject
    {
        public List<Mapping> mappingList;
    }

    [System.Serializable]
    public class Mapping
    {
        public int key;
        public string name;
    }
}
