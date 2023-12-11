using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TPFive.Creator.Bundle.Editor.UI
{
    using TPFive.Creator.Editor;

    [CustomEditor(typeof(TPFive.Creator.BundleDetailData), true)]
    public sealed partial class BundleDetailDataEditor : UnityEditor.Editor
    {
        [SerializeField]
        private VisualTreeAsset visualTreeAsset;

        private BundleDetailData Target => target as BundleDetailData;

        public override VisualElement CreateInspectorGUI()
        {
            if (visualTreeAsset == null)
            {
                return default;
            }

            VisualElement container = new();

            container.Add(visualTreeAsset.CloneTree());

            var idPropertyField = container.Q<PropertyField>("id");
            if (idPropertyField != null)
            {
                idPropertyField.SetEnabled(false);
            }

            var ugcIdPropertyField = container.Q<PropertyField>("ugcId");
            if (ugcIdPropertyField != null)
            {
                ugcIdPropertyField.SetEnabled(false);
            }

            return container;
        }
    }
}
