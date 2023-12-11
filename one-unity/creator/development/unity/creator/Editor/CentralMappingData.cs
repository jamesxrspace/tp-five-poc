using UnityEngine;

namespace TPFive.Creator.Editor
{
    [CreateAssetMenu(fileName = "Central Mapping Data", menuName = "TPFive/Creator/Central Mapping Data")]
    public class CentralMappingData : ScriptableObject
    {
        public MappingLayerData currentMappingData;
    }
}
