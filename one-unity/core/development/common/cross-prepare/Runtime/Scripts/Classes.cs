namespace TPFive.Cross.Prepare
{
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

    [System.Serializable]
    public class ScriptDefineAdjustment
    {
        public string title;
        public int adjustment;
    }

    [System.Serializable]
    public class KeyValueSubstitution
    {
#if ODIN_INSPECTOR
        [TableColumnWidth(140, false)]
        [LabelWidth(100)]
#endif
        public string key;

#if ODIN_INSPECTOR
        [TableColumnWidth(200)]
        [LabelWidth(100)]
#endif
        public string value;

        public override string ToString()
        {
            var desc = $"key: {key} value: {value}";
            return desc;
        }
    }

    [System.Serializable]
    public partial class PathInfo
    {
#if ODIN_INSPECTOR
        // [GUIColor(0.5f, 1f, 1f)]
        [GUIColor("GetColorSourcePath")]
        [LabelWidth(120)]
        [PropertyTooltip("Source path of the folder or file")]
#endif
        public string sourcePath;

#if ODIN_INSPECTOR
        // [GUIColor(0.922f, 0.494f, 0.090f)]
        [GUIColor("GetColorTargetPath")]
        [LabelWidth(120)]
        [PropertyTooltip("Target path of the folder or file")]
#endif
        public string targetPath;

#if ODIN_INSPECTOR
        [LabelWidth(120)]
        [PropertyTooltip("Operation type of the folder or file")]
        [ValueDropdown("operateTypeOptionList")]
#endif
        public int operateType;

#if ODIN_INSPECTOR
        [LabelWidth(180)]
        [PropertyTooltip("Create the parent folder of the target path if it does not exist")]
#endif
        public int createTargetDirectParent;

        public override string ToString()
        {
            var desc = $"sourcePath: {sourcePath}\ntargetPath: {targetPath}";
            return desc;
        }
    }
}
