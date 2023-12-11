namespace TPFive.Cross.Prepare
{
    using System.Collections;
    using System.Collections.Generic;
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

    public partial class PathInfo
    {
        private static Color GetColorSourcePath()
        {
            return Color.cyan;
        }

        private static Color GetColorTargetPath()
        {
            return Color.yellow;
        }

#if ODIN_INSPECTOR
        private static IEnumerable operateTypeOptionList = new ValueDropdownList<int>()
        {
            {"No operation", 0},
            {"Symlink", 1},
            {"Copy when none", 2},
            {"Copy", 3}
        };
#endif
    }
}
