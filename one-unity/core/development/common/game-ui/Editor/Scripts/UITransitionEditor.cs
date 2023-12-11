using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine.UI;

namespace TPFive.Game.UI.Editor
{
    [CustomEditor(typeof(UITransition), true)]
    [CanEditMultipleObjects]
    public class UITransitionEditor : UnityEditor.Editor
    {
        private readonly AnimBool showColorTint = new ();
        private readonly AnimBool showSpriteTrasition = new ();

        private SerializedProperty script;
        private SerializedProperty interactableProperty;
        private SerializedProperty ignoredSelectEventProperty;
        private SerializedProperty targetGraphicProperty;
        private SerializedProperty transitionProperty;
        private SerializedProperty colorBlockProperty;
        private SerializedProperty spriteStateProperty;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginDisabledGroup(true);
            {
                EditorGUILayout.PropertyField(script);
            }

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.PropertyField(interactableProperty);

            var trans = GetTransition(transitionProperty);
            var graphic = targetGraphicProperty.objectReferenceValue as Graphic;

            showColorTint.target = trans.HasFlag(UITransition.TransitionType.ColorTint);
            showSpriteTrasition.target = trans.HasFlag(UITransition.TransitionType.SpriteSwap);

            EditorGUILayout.PropertyField(transitionProperty);

            if (trans > 0)
            {
                EditorGUILayout.PropertyField(targetGraphicProperty);
                EditorGUILayout.PropertyField(ignoredSelectEventProperty);
            }

            ++EditorGUI.indentLevel;
            {
                switch (trans)
                {
                    case UITransition.TransitionType.ColorTint:
                        if (graphic == null)
                        {
                            EditorGUILayout.HelpBox("You must have a Graphic target in order to use a color transition.", MessageType.Warning);
                        }

                        break;

                    case UITransition.TransitionType.SpriteSwap:
                        if (graphic as Image == null)
                        {
                            EditorGUILayout.HelpBox("You must have a Image target in order to use a sprite swap transition.", MessageType.Warning);
                        }

                        break;
                }

                if (EditorGUILayout.BeginFadeGroup(showColorTint.faded))
                {
                    --EditorGUI.indentLevel;
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Color Tint", EditorStyles.boldLabel);
                    ++EditorGUI.indentLevel;
                    EditorGUILayout.PropertyField(colorBlockProperty);
                }

                EditorGUILayout.EndFadeGroup();

                if (EditorGUILayout.BeginFadeGroup(showSpriteTrasition.faded))
                {
                    --EditorGUI.indentLevel;
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Sprite Swap", EditorStyles.boldLabel);
                    ++EditorGUI.indentLevel;
                    EditorGUILayout.PropertyField(spriteStateProperty);
                }

                EditorGUILayout.EndFadeGroup();
            }

            --EditorGUI.indentLevel;

            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();
        }

        protected void OnEnable()
        {
            script = serializedObject.FindProperty("m_Script");
            interactableProperty = serializedObject.FindProperty("interactable");
            ignoredSelectEventProperty = serializedObject.FindProperty("ignoredSelectEvent");
            targetGraphicProperty = serializedObject.FindProperty("targetGraphic");
            transitionProperty = serializedObject.FindProperty("transition");
            colorBlockProperty = serializedObject.FindProperty("colors");
            spriteStateProperty = serializedObject.FindProperty("spriteState");

            var trans = GetTransition(transitionProperty);
            showColorTint.value = trans == UITransition.TransitionType.ColorTint;
            showSpriteTrasition.value = trans == UITransition.TransitionType.SpriteSwap;

            showColorTint.valueChanged.AddListener(Repaint);
            showSpriteTrasition.valueChanged.AddListener(Repaint);
        }

        protected void OnDisable()
        {
            showColorTint.valueChanged.RemoveListener(Repaint);
            showSpriteTrasition.valueChanged.RemoveListener(Repaint);
        }

        private static UITransition.TransitionType GetTransition(SerializedProperty transition)
        {
            return (UITransition.TransitionType)transition.enumValueFlag;
        }
    }
}
