using UnityEditor;

namespace TPFive.Game.UI.Editor
{
    [CustomEditor(typeof(UIViewAnimation))]
    public sealed class UIViewAnimationEditor : UnityEditor.Editor
    {
        private SerializedProperty enterAnimation;
        private SerializedProperty exitAnimation;

        private SerializedProperty enterAnimType;
        private SerializedProperty exitAnimType;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Draw 'enterAnimation' property
            DrawPropertyWithLabel(enterAnimation);

            // Force set the animation type is 'In' for the enter animation.
            enterAnimType.intValue = (int)Anim.AnimationType.In;

            // Draw 'exitAnimation' property
            DrawPropertyWithLabel(exitAnimation);

            // Force set the animation type is 'Out' for the exit animation.
            exitAnimType.intValue = (int)Anim.AnimationType.Out;

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawPropertyWithLabel(SerializedProperty property)
        {
            if (property == null)
            {
                return;
            }

            EditorGUILayout.LabelField(property.displayName, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(property);
            EditorGUILayout.Space();
        }

        private void OnEnable()
        {
            enterAnimation = serializedObject.FindProperty("enterAnimation");
            exitAnimation = serializedObject.FindProperty("exitAnimation");

            enterAnimType = enterAnimation.FindPropertyRelative("type");
            exitAnimType = exitAnimation.FindPropertyRelative("type");
        }
    }
}
