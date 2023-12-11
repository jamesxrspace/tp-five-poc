using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.Editor;

namespace TPFive.Extended.InputSystem.Interactions.Editors
{
    internal class MinMaxAxisInteractionEditor : InputParameterEditor<MinMaxAxisInteraction>
    {
        private GUIContent minLabel;
        private GUIContent maxLabel;
        private GUIContent invertLabel;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            
            minLabel = new GUIContent
            {
                text = "Min Value",
                tooltip = "Min value of range."
            };
            maxLabel = new GUIContent
            {
                text = "Max Value",
                tooltip = "Max value of range."
            };
            invertLabel = new GUIContent
            {
                text = "Invert",
                tooltip = "Invert behavior. If current value is in range, the output will be not."
            };
        }

        public override void OnGUI()
        {
            target.Min = EditorGUILayout.DelayedFloatField(minLabel, target.Min);
            target.Max = EditorGUILayout.DelayedFloatField(maxLabel, target.Max);
            target.Invert = EditorGUILayout.Toggle(invertLabel, target.Invert);
        }
    }
}