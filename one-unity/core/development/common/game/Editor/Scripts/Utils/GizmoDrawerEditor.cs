using System.Diagnostics.CodeAnalysis;
using UnityEditor;

namespace TPFive.Game.Utils
{
    /// <summary>
    /// The custom editor for <see cref="GizmoDrawer"/>.
    /// </summary>
    [CustomEditor(typeof(GizmoDrawer))]
    public class GizmoDrawerEditor : UnityEditor.Editor
    {
        private SerializedProperty showGizmoProperty;
        private SerializedProperty drawTypeProperty;
        private SerializedProperty centerOffestProperty;
        private SerializedProperty colorProperty;
        private SerializedProperty sizeProperty;
        private SerializedProperty radiusProperty;
        private SerializedProperty showForwardArrowProperty;
        private SerializedProperty forwardArrowLengthProperty;

        /// <inheritdoc/>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            showGizmoProperty.boolValue = EditorGUILayout.ToggleLeft(
                showGizmoProperty.displayName,
                showGizmoProperty.boolValue);
            EditorGUI.BeginDisabledGroup(!showGizmoProperty.boolValue);
            {
                EditorGUILayout.PropertyField(drawTypeProperty);
                EditorGUILayout.PropertyField(centerOffestProperty);
                EditorGUILayout.PropertyField(colorProperty);

                switch ((GizmoDrawer.Type)drawTypeProperty.enumValueIndex)
                {
                    case GizmoDrawer.Type.Cube:
                    case GizmoDrawer.Type.WireCube:
                        EditorGUILayout.PropertyField(sizeProperty);
                        break;
                    case GizmoDrawer.Type.Sphere:
                    case GizmoDrawer.Type.WireSphere:
                        EditorGUILayout.PropertyField(radiusProperty);
                        break;
                }

                EditorGUILayout.Space();

                showForwardArrowProperty.boolValue = EditorGUILayout.ToggleLeft(
                    showForwardArrowProperty.displayName,
                    showForwardArrowProperty.boolValue);
                EditorGUI.BeginDisabledGroup(!showForwardArrowProperty.boolValue);
                {
                    EditorGUILayout.PropertyField(forwardArrowLengthProperty);
                }

                EditorGUI.EndDisabledGroup();
            }

            EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:Elements should be documented", Justification = "Reviewed")]
        protected void OnEnable()
        {
            // Find property
            showGizmoProperty = serializedObject.FindProperty("showGizmo");
            drawTypeProperty = serializedObject.FindProperty("drawType");
            centerOffestProperty = serializedObject.FindProperty("centerOffest");
            colorProperty = serializedObject.FindProperty("color");
            sizeProperty = serializedObject.FindProperty("size");
            radiusProperty = serializedObject.FindProperty("radius");
            showForwardArrowProperty = serializedObject.FindProperty("showForwardArrow");
            forwardArrowLengthProperty = serializedObject.FindProperty("forwardArrowLength");
        }
    }
}