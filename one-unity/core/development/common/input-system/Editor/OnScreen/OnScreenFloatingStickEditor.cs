using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.InputSystem.OnScreen;

namespace TPFive.Extended.InputSystem.OnScreen.Editors
{
    [CustomEditor(typeof(OnScreenFloatingStick))]
    internal class OnScreenFloatingStickEditor : Editor
    {
        private OnScreenFloatingStick floatingStick;

        private AnimBool showDynamicOriginOptions;
        private AnimBool showIsolatedInputActions;

        private SerializedProperty script;
        private SerializedProperty useIsolatedInputActions;
        private SerializedProperty behaviour;
        private SerializedProperty controlPathInternal;
        private SerializedProperty allowKnobBeyondMovementRange;
        private SerializedProperty movementRange;
        private SerializedProperty dynamicOriginRange;
        private SerializedProperty pointerDownAction;
        private SerializedProperty pointerMoveAction;
        private SerializedProperty strengthThreshold;
        private SerializedProperty showVisualElement;
        private SerializedProperty visualWeakBackgroundRect;
        private SerializedProperty visualStrongBackgroundRect;
        private SerializedProperty visualKnobRect;

        public void OnEnable()
        {
            showDynamicOriginOptions = new AnimBool(false);
            showIsolatedInputActions = new AnimBool(false);

            script = serializedObject.FindProperty("m_Script");
            allowKnobBeyondMovementRange = serializedObject.FindProperty("allowKnobBeyondMovementRange");
            movementRange = serializedObject.FindProperty("movementRange");
            dynamicOriginRange = serializedObject.FindProperty("dynamicOriginRange");
            controlPathInternal = serializedObject.FindProperty("stickControlPath");
            behaviour = serializedObject.FindProperty("behaviour");

            useIsolatedInputActions = serializedObject.FindProperty("useIsolatedInputActions");
            pointerDownAction = serializedObject.FindProperty("pointerDownAction");
            pointerMoveAction = serializedObject.FindProperty("pointerMoveAction");

            strengthThreshold = serializedObject.FindProperty("strengthThreshold");
            showVisualElement = serializedObject.FindProperty("showVisualElement");
            visualWeakBackgroundRect = serializedObject.FindProperty("visualWeakBackgroundRect");
            visualStrongBackgroundRect = serializedObject.FindProperty("visualStrongBackgroundRect");
            visualKnobRect = serializedObject.FindProperty("visualKnobRect");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginDisabledGroup(true);
            {
                EditorGUILayout.PropertyField(script);
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space();

            GUILayout.Label("Stick setting", EditorStyles.boldLabel);
            GUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.PropertyField(allowKnobBeyondMovementRange);
                EditorGUILayout.PropertyField(movementRange);
                EditorGUILayout.PropertyField(controlPathInternal);
                EditorGUILayout.PropertyField(behaviour);

                showDynamicOriginOptions.target = (OnScreenStick.Behaviour)behaviour.intValue ==
                    OnScreenStick.Behaviour.ExactPositionWithDynamicOrigin;
                if (EditorGUILayout.BeginFadeGroup(showDynamicOriginOptions.faded))
                {
                    ++EditorGUI.indentLevel;
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(dynamicOriginRange);
                    if (EditorGUI.EndChangeCheck())
                    {
                        UpdateDynamicOriginClickableArea();
                    }
                    --EditorGUI.indentLevel;
                }
                EditorGUILayout.EndFadeGroup();

                EditorGUILayout.PropertyField(useIsolatedInputActions);
                showIsolatedInputActions.target = useIsolatedInputActions.boolValue;
                if (EditorGUILayout.BeginFadeGroup(showIsolatedInputActions.faded))
                {
                    ++EditorGUI.indentLevel;
                    EditorGUILayout.PropertyField(pointerDownAction);
                    EditorGUILayout.PropertyField(pointerMoveAction);
                    --EditorGUI.indentLevel;
                }
                EditorGUILayout.EndFadeGroup();
            }
            GUILayout.EndVertical();
            EditorGUILayout.Space();

            GUILayout.Label("Visual setting", EditorStyles.boldLabel);
            GUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.PropertyField(strengthThreshold);
                EditorGUILayout.PropertyField(showVisualElement);
                EditorGUI.BeginDisabledGroup(!showVisualElement.boolValue);
                {
                    EditorGUILayout.PropertyField(visualWeakBackgroundRect);
                    EditorGUILayout.PropertyField(visualStrongBackgroundRect);
                    EditorGUILayout.PropertyField(visualKnobRect);
                }
                EditorGUI.EndDisabledGroup();
            }
            GUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }

        private void UpdateDynamicOriginClickableArea()
        {
            var dynamicOriginTransform = floatingStick.transform.Find(OnScreenFloatingStick.DynamicOriginClickable);
            if (dynamicOriginTransform != null)
            {
                float size = dynamicOriginRange.floatValue * 2;
                var rectTransform = (RectTransform)dynamicOriginTransform;
                rectTransform.sizeDelta = new Vector2(size, size);
            }
        }
    }
}
