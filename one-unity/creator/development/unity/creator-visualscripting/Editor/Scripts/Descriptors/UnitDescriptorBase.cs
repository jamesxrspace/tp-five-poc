using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace TPFive.Creator.VisualScripting.Editor
{
    public abstract class UnitDescriptorBase<T> : UnitDescriptor<T> where T : Unit
    {
        protected UnitDescriptorBase(T target) : base(target)
        {
        }

        protected override string DefinedSummary()
        {
            return string.Empty;
        }

        protected override EditorTexture DefinedIcon()
        {
            var path = Path.Combine(
                "Packages",
                "io.xrspace.TPFive.creator.visualscripting",
                "Editor", "Data Assets", "Icon.png");
            var icon = AssetDatabase.LoadAssetAtPath<Texture>(path);

            return EditorTexture.Single(icon);
        }
    }
}
