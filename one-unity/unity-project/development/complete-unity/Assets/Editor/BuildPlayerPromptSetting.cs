using System.Collections.Generic;
using UnityEngine;

namespace TPFive.Editor
{
    [CreateAssetMenu(fileName = nameof(BuildPlayerPromptSetting), menuName = "TPFive/Editor/Create BuildPlayerPromptSetting")]
    public class BuildPlayerPromptSetting : ScriptableObject
    {
        [SerializeField]
        [Tooltip("Supports to search the file system for files with names that match specified patterns")]
        List<string> essentialFiles;

        public List<string> EssentialFiles => essentialFiles;

        [ContextMenu("Open Documentation")]
        private void OpenDocumentation()
        {
            Application.OpenURL("https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.filesystemglobbing.matcher?view=dotnet-plat-ext-7.0&viewFallbackFrom=netstandard-2.0");
        }
    }
}