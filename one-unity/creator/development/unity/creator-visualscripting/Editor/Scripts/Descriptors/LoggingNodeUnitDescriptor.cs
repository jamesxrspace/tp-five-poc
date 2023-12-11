using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace TPFive.Creator.VisualScripting.Editor
{
    [Descriptor(typeof(LoggingNode))]
    public class LoggingNodeUnitDescriptor : UnitDescriptorBase<LoggingNode>
    {
        public LoggingNodeUnitDescriptor(LoggingNode target) : base(target)
        {
        }

        protected override EditorTexture DefinedIcon()
        {
            var path = Path.Combine(
                "Packages",
                "io.xrspace.TPFive.creator.visualscripting",
                "Editor", "Data Assets", "Icon 02.png");
            var icon = AssetDatabase.LoadAssetAtPath<Texture>(path);

            return EditorTexture.Single(icon);
        }
    }
}
