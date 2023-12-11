using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.Editor;

namespace TPFive.Extended.InputSystem.Processors
{
    internal class ForcePushVector2ProcessorEditor : InputParameterEditor<ForcePushVector2Processor>
    {
        public override void OnGUI()
        {
            var newValue = EditorGUILayout.Vector2Field("Force Value", new Vector2(target.X, target.Y));
            target.X = newValue.x;
            target.Y = newValue.y;
        }
    }
}