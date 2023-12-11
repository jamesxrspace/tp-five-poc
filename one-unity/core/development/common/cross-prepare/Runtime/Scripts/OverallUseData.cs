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
    [CreateAssetMenu(fileName = "Overall Use Data", menuName = "XSPO/Prepare/Overall Use Data")]
#endif
    public class OverallUseData
#if DOTNET_PROJECT
#else
        : ScriptableObject
#endif
    {
#if ODIN_INSPECTOR
        [Header("Common Settings")]
        [TableList]
#endif
        public List<KeyValueSubstitution> keyValueSubstitutionList;

        public override string ToString()
        {
            var desc = keyValueSubstitutionList.Aggregate("", (acc, next) => $"{acc}{next}\n");
            return desc;
        }
    }
}
