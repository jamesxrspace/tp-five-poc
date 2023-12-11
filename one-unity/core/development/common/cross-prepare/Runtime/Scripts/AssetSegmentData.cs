namespace TPFive.Cross.Prepare
{
    using System.Collections.Generic;
    using System.Linq;
#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
#if UNITY_EDITOR
    using UnityEditor;
#endif
#if DOTNET_PROJECT
#else
    using UnityEngine;
#endif

    //
#if DOTNET_PROJECT
#else
    [CreateAssetMenu(fileName = "Asset Segment Data", menuName = "DuIytu/Prepare/Asset Segment Data")]
#endif
    public class AssetSegmentData
#if DOTNET_PROJECT
#else
        : ScriptableObject
#endif
    {
        public string title;

#if ODIN_INSPECTOR
        [Header("Specific Settings")]
        [TableList]
#endif
        public List<KeyValueSubstitution> keyValueSubstitutionList;

        public List<PathInfo> pathInfoList;

        public List<ScriptDefineAdjustment> scriptDefineAdjustmentList;

#if DOTNET_PROJECT
        public List<KrTtbu.Cross.UnityReferenceInAsset> extensionList;
#else
        public List<ScriptableObject> extensionList;
#endif

        public override string ToString()
        {
            var keyValueSubstitutionListDesc = keyValueSubstitutionList
                .Aggregate("", (acc, next) => $"{acc}{next}\n");
            var pathInfoListDesc = pathInfoList
                .Aggregate("", (acc, next) => $"{acc}{next}\n");
            var desc = $"keyValueSubstitutionList:\n{keyValueSubstitutionListDesc}\npathInfoList:\n{pathInfoListDesc}";
            return desc;
        }
    }
}
