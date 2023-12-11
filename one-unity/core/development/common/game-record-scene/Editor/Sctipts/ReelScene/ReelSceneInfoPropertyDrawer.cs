using UnityEditor;
using UnityEngine;

namespace TPFive.Game.Record.Scene
{
    [CustomPropertyDrawer(typeof(ReelSceneInfo), true)]
    public class ReelSceneInfoPropertyDrawer : PropertyDrawer
    {
        private GUIStyle boldStyle;

        private SerializedProperty watchReelSetting;
        private SerializedProperty enablePrepareState;
        private SerializedProperty prepareRecordSetting;
        private SerializedProperty standByRecordSetting;
        private SerializedProperty recordingSetting;
        private SerializedProperty previewRecordSetting;
        private SerializedProperty randomTrack;
        private SerializedProperty enableMusicToMotion;
        private SerializedProperty reelCameraTargetType;
        private SerializedProperty fixedPosition;

        private bool isInitialized;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            InitializeInfNeed(property);

            // title
            position.height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.LabelField(position, "State Settings", boldStyle);
            ShiftYBySelfHeightAndSpace(ref position);

            // watchReelSetting
            position.height = EditorGUI.GetPropertyHeight(watchReelSetting);
            GUI.Box(position, GUIContent.none, GUI.skin.box);
            EditorGUI.PropertyField(position, watchReelSetting, true);
            ShiftYBySelfHeightAndSpace(ref position);

            // enablePrepareState
            position.height = EditorGUI.GetPropertyHeight(enablePrepareState);
            EditorGUI.PropertyField(position, enablePrepareState);
            ShiftYBySelfHeightAndSpace(ref position);

            // prepareRecordSetting
            EditorGUI.BeginDisabledGroup(!enablePrepareState.boolValue);
            {
                position.height = EditorGUI.GetPropertyHeight(prepareRecordSetting);
                GUI.Box(position, GUIContent.none, GUI.skin.box);
                EditorGUI.PropertyField(position, prepareRecordSetting, true);
                ShiftYBySelfHeightAndSpace(ref position);
            }
            EditorGUI.EndDisabledGroup();

            // standByRecordSetting
            position.height = EditorGUI.GetPropertyHeight(standByRecordSetting);
            GUI.Box(position, GUIContent.none, GUI.skin.box);
            EditorGUI.PropertyField(position, standByRecordSetting, true);
            ShiftYBySelfHeightAndSpace(ref position);

            // recordingSetting
            position.height = EditorGUI.GetPropertyHeight(recordingSetting);
            GUI.Box(position, GUIContent.none, GUI.skin.box);
            EditorGUI.PropertyField(position, recordingSetting, true);
            ShiftYBySelfHeightAndSpace(ref position);

            // previewRecordSetting
            position.height = EditorGUI.GetPropertyHeight(previewRecordSetting);
            GUI.Box(position, GUIContent.none, GUI.skin.box);
            EditorGUI.PropertyField(position, previewRecordSetting, true);
            ShiftYBySelfHeightAndSpace(ref position);

            position.y += EditorGUIUtility.standardVerticalSpacing;

            var boxHeight =
                EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing +
                EditorGUI.GetPropertyHeight(randomTrack) + EditorGUIUtility.standardVerticalSpacing +
                EditorGUI.GetPropertyHeight(enableMusicToMotion) + EditorGUIUtility.standardVerticalSpacing +
                EditorGUI.GetPropertyHeight(reelCameraTargetType) + EditorGUIUtility.standardVerticalSpacing +
                EditorGUIUtility.singleLineHeight;

            if (reelCameraTargetType.enumValueIndex == (int)ReelCameraTargetType.FixedPosition)
            {
                // fixedPosition
                boxHeight += EditorGUI.GetPropertyHeight(fixedPosition) + EditorGUIUtility.standardVerticalSpacing;
            }

            position.height = boxHeight;
            GUI.Box(position, GUIContent.none, GUI.skin.box);

            // title
            position.height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.LabelField(position, "Reel Track Settings", boldStyle);
            ShiftYBySelfHeightAndSpace(ref position);

            // randomTrack
            position.height = EditorGUI.GetPropertyHeight(randomTrack);
            EditorGUI.PropertyField(position, randomTrack);
            ShiftYBySelfHeightAndSpace(ref position);

            // enableMusicToMotion
            position.height = EditorGUI.GetPropertyHeight(enableMusicToMotion);
            EditorGUI.PropertyField(position, enableMusicToMotion);
            ShiftYBySelfHeightAndSpace(ref position);

            // reelCameraTargetType
            position.height = EditorGUI.GetPropertyHeight(reelCameraTargetType);
            EditorGUI.PropertyField(position, reelCameraTargetType, true);
            ShiftYBySelfHeightAndSpace(ref position);

            if (reelCameraTargetType.enumValueIndex == (int)ReelCameraTargetType.FixedPosition)
            {
                // fixedPosition
                position.height = EditorGUI.GetPropertyHeight(fixedPosition);
                EditorGUI.PropertyField(position, fixedPosition);
                ShiftYBySelfHeightAndSpace(ref position);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            InitializeInfNeed(property);
            var fixedPositionHeight = reelCameraTargetType.enumValueIndex == (int)ReelCameraTargetType.FixedPosition ?
                EditorGUI.GetPropertyHeight(fixedPosition) : 0f;

            return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing +
                   EditorGUI.GetPropertyHeight(watchReelSetting) +
                   EditorGUI.GetPropertyHeight(enablePrepareState) +
                   EditorGUI.GetPropertyHeight(prepareRecordSetting) +
                   EditorGUI.GetPropertyHeight(standByRecordSetting) +
                   EditorGUI.GetPropertyHeight(recordingSetting) +
                   EditorGUI.GetPropertyHeight(previewRecordSetting) +
                   EditorGUIUtility.standardVerticalSpacing +
                   EditorGUIUtility.singleLineHeight +
                   EditorGUI.GetPropertyHeight(randomTrack) +
                   EditorGUI.GetPropertyHeight(enableMusicToMotion) +
                   EditorGUI.GetPropertyHeight(reelCameraTargetType) +
                   fixedPositionHeight +
                   (EditorGUIUtility.standardVerticalSpacing * 26f);
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

            watchReelSetting = property.FindPropertyRelative(nameof(watchReelSetting));
            enablePrepareState = property.FindPropertyRelative(nameof(enablePrepareState));
            prepareRecordSetting = property.FindPropertyRelative(nameof(prepareRecordSetting));
            standByRecordSetting = property.FindPropertyRelative(nameof(standByRecordSetting));
            recordingSetting = property.FindPropertyRelative(nameof(recordingSetting));
            previewRecordSetting = property.FindPropertyRelative(nameof(previewRecordSetting));
            randomTrack = property.FindPropertyRelative(nameof(randomTrack));
            enableMusicToMotion = property.FindPropertyRelative(nameof(enableMusicToMotion));
            reelCameraTargetType = property.FindPropertyRelative(nameof(reelCameraTargetType));
            fixedPosition = property.FindPropertyRelative(nameof(fixedPosition));

            isInitialized = true;
        }

        private void ShiftYBySelfHeightAndSpace(ref Rect rect)
        {
            rect.y += rect.height + (EditorGUIUtility.standardVerticalSpacing * 2f);
        }
    }
}
