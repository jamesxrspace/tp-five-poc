using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TPFive.Editor
{
    [FilePath("Assets/Editor/Settings/BuildPlayerPromptSettingManager.asset", FilePathAttribute.Location.ProjectFolder)]
    [CreateAssetMenu(fileName = nameof(BuildPlayerPromptSettingManager), menuName = "TPFive/Editor/Create BuildPlayerPromptSettingManager")]
    public class BuildPlayerPromptSettingManager : ScriptableSingleton<BuildPlayerPromptSettingManager>
    {
        [Serializable]
        private class BuildTargetSetting
        {
            [SerializeField]
            private BuildTarget buildTarget;

            [SerializeField]
            private BuildPlayerPromptSetting setting;

            public BuildTarget BuildTarget => buildTarget;

            public BuildPlayerPromptSetting Setting => setting;
        }

        [SerializeField]
        List<BuildTargetSetting> buildTargetSettings;

        public static BuildPlayerPromptSetting FindSetting(BuildTarget buildTarget)
        {
            return instance.buildTargetSettings.Find(x => x.BuildTarget == buildTarget)?.Setting;
        }
    }
}
    