using UnityEditor;
using UnityEngine;

namespace TPFive.Game.Record.Scene
{
    [CustomPropertyDrawer(typeof(ReelStateSetting), true)]
    public class ReelStateSettingPropertyDrawer : PropertyDrawer
    {
        private GUIStyle boldStyle;

        private SerializedProperty state;
        private SerializedProperty alignState;
        private SerializedProperty alignStateHasValue;
        private SerializedProperty enableSingleDefaultCamera;
        private SerializedProperty singleDefaultCameraSetting;
        private SerializedProperty multiDefaultCameraSettings;

        private bool isInitialized;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            InitializeInfNeed(property);

            EditorGUI.BeginProperty(position, label, property);
            {
                // Title
                position.height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.LabelField(position, $"State - {(ReelState)state.intValue}", boldStyle);
                ShiftYBySelfHeightAndSpace(ref position);
                ShiftByIndent(ref position, 1);

                // Align State
                position.height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(position, alignState);
                ShiftYBySelfHeightAndSpace(ref position);

                EditorGUI.BeginDisabledGroup(alignStateHasValue.boolValue);
                {
                    // Enable Single Default Camera
                    position.height = EditorGUI.GetPropertyHeight(enableSingleDefaultCamera);
                    EditorGUI.PropertyField(position, enableSingleDefaultCamera);
                    ShiftYBySelfHeightAndSpace(ref position);

                    EditorGUI.BeginDisabledGroup(!enableSingleDefaultCamera.boolValue);
                    {
                        // Single Default Camera Setting
                        position.height = EditorGUI.GetPropertyHeight(singleDefaultCameraSetting);
                        EditorGUI.PropertyField(position, singleDefaultCameraSetting, true);
                        ShiftYBySelfHeightAndSpace(ref position);
                    }
                    EditorGUI.EndDisabledGroup();

                    EditorGUI.BeginDisabledGroup(enableSingleDefaultCamera.boolValue);
                    {
                        // Multi Default Camera Settings
                        position.height = EditorGUI.GetPropertyHeight(multiDefaultCameraSettings);
                        EditorGUI.PropertyField(position, multiDefaultCameraSettings, true);
                        ShiftYBySelfHeightAndSpace(ref position);
                    }
                    EditorGUI.EndDisabledGroup();
                }
                EditorGUI.EndDisabledGroup();
                ShiftByIndent(ref position, -1);
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            InitializeInfNeed(property);

            return EditorGUI.GetPropertyHeight(state) +
                   EditorGUI.GetPropertyHeight(alignState) +
                   EditorGUI.GetPropertyHeight(enableSingleDefaultCamera) +
                   EditorGUI.GetPropertyHeight(singleDefaultCameraSetting) +
                   EditorGUI.GetPropertyHeight(multiDefaultCameraSettings) +
                   (EditorGUIUtility.standardVerticalSpacing * 4f);
        }

        private void InitializeInfNeed(SerializedProperty property)
        {
            if (isInitialized)
            {
                return;
            }

            boldStyle = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Bold,
            };

            state = property.FindPropertyRelative("state");
            alignState = property.FindPropertyRelative("alignState");
            alignStateHasValue = alignState.FindPropertyRelative("hasValue");
            enableSingleDefaultCamera = property.FindPropertyRelative("enableSingleDefaultCamera");
            singleDefaultCameraSetting = property.FindPropertyRelative("singleDefaultCameraSetting");
            multiDefaultCameraSettings = property.FindPropertyRelative("multiDefaultCameraSettings");

            isInitialized = true;
        }

        private void ShiftYBySelfHeightAndSpace(ref Rect rect)
        {
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
        }

        private void ShiftByIndent(ref Rect rect, int indent)
        {
            float delta = indent * 20f;
            rect.x += delta;
            rect.width -= delta;
        }
    }
}
