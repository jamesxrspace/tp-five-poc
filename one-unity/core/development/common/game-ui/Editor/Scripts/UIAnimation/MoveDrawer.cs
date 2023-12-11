using System;
using UnityEditor;
using UnityEngine;

namespace TPFive.Game.UI.Editor
{
    [CustomPropertyDrawer(typeof(Move), true)]
    public class MoveDrawer : PropertyDrawer
    {
        private readonly Color32 backgroundColor = EditorGUIUtility.isProSkin ? new (61, 101, 34, 255) : new (175, 223, 142, 255);

        private SerializedProperty enabled;
        private SerializedProperty animationType;
        private SerializedProperty startDelay;
        private SerializedProperty duration;
        private SerializedProperty moveDirection;
        private SerializedProperty customPosition;
        private SerializedProperty easeType;
        private SerializedProperty ease;
        private SerializedProperty animationCurve;

        private bool initialized = false;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!initialized)
            {
                Init(property);
            }

            // Being Property
            EditorGUI.BeginProperty(position, label, property);

            Rect drawRect = position;

            // Draw Background
            Rect blockRect = drawRect;
            blockRect.height = GetBlockHeight();
            EditorGUI.DrawRect(blockRect, backgroundColor);

            // Cache label width
            var generalLabelWidth = EditorGUIUtility.labelWidth;

            // Move Block
            GUI.backgroundColor = backgroundColor;
            drawRect.height = EditorGUIUtility.singleLineHeight;
            enabled.boolValue = EditorGUI.Toggle(drawRect, "Move", enabled.boolValue);
            if (enabled.boolValue)
            {
                DrawMoveBlock(drawRect, (Anim.AnimationType)animationType.intValue);
            }

            // Revert label width
            EditorGUIUtility.labelWidth = generalLabelWidth;

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            FindProperties(property);
            return GetBlockHeight();
        }

        public override bool CanCacheInspectorGUI(SerializedProperty property)
        {
            return false;
        }

        private void Init(SerializedProperty property)
        {
            initialized = true;

            FindProperties(property);
        }

        private void FindProperties(SerializedProperty property)
        {
            enabled = property.FindPropertyRelative("enabled");
            animationType = property.FindPropertyRelative("animationType");
            startDelay = property.FindPropertyRelative("startDelay");
            duration = property.FindPropertyRelative("duration");
            moveDirection = property.FindPropertyRelative("direction");
            customPosition = property.FindPropertyRelative("customPosition");
            easeType = property.FindPropertyRelative("easeType");
            ease = property.FindPropertyRelative("ease");
            animationCurve = property.FindPropertyRelative("animationCurve");
        }

        private float GetBlockHeight()
        {
            var height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            if (!enabled.boolValue)
            {
                return height;
            }

            var factor = ((Move.MoveDirection)moveDirection.intValue == Move.MoveDirection.CustomPosition &&
                (Anim.AnimationType)animationType.intValue != Anim.AnimationType.State) ? 4f : 3f;
            return height * factor;
        }

        private void DrawMoveBlock(Rect drawRect, Anim.AnimationType animType)
        {
            // Line 1
            drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            drawRect.width /= 4f;
            startDelay.floatValue = FloatField(drawRect, "start delay", startDelay.floatValue);

            drawRect.x += drawRect.width;
            duration.floatValue = FloatField(drawRect, "duration", duration.floatValue);

            drawRect.x += drawRect.width;
            drawRect.width *= 2f;
            switch (animType)
            {
                case Anim.AnimationType.In:
                    moveDirection.intValue = (int)(Move.MoveDirection)EnumPopup(
                        drawRect,
                        "move from",
                        (Move.MoveDirection)moveDirection.intValue);
                    break;
                case Anim.AnimationType.Out:
                    moveDirection.intValue = (int)(Move.MoveDirection)EnumPopup(
                        drawRect,
                        "move to",
                        (Move.MoveDirection)moveDirection.intValue);
                    break;
                case Anim.AnimationType.State:
                    customPosition.vector3Value = Vector3Field(drawRect, "move by", customPosition.vector3Value);
                    break;
            }

            // Line 2
            drawRect.x -= drawRect.width;
            drawRect.width *= 2f;
            if (animType != Anim.AnimationType.State &&
                (Move.MoveDirection)moveDirection.intValue == Move.MoveDirection.CustomPosition)
            {
                drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                customPosition.vector3Value = Vector3Field(drawRect, "position", customPosition.vector3Value);
            }

            // Line 3
            drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            drawRect.width /= 3f;
            easeType.intValue = (int)(UIAnimator.EaseType)EditorGUI.EnumPopup(
                drawRect,
                (UIAnimator.EaseType)easeType.intValue);

            drawRect.x += drawRect.width;
            drawRect.width *= 2f;
            if ((UIAnimator.EaseType)easeType.intValue == UIAnimator.EaseType.Ease)
            {
                ease.intValue = (int)(DG.Tweening.Ease)EditorGUI.EnumPopup(
                    drawRect,
                    (DG.Tweening.Ease)ease.intValue);
            }
            else
            {
                animationCurve.animationCurveValue = EditorGUI.CurveField(
                    drawRect,
                    animationCurve.animationCurveValue);
            }
        }

        private float FloatField(Rect drawRect, string label, float value)
        {
            var textDimensions = GUI.skin.label.CalcSize(new GUIContent(label));
            EditorGUIUtility.labelWidth = textDimensions.x + 1f;
            return EditorGUI.FloatField(drawRect, label, value);
        }

        private Enum EnumPopup(Rect drawRect, string label, Enum selected)
        {
            var textDimensions = GUI.skin.label.CalcSize(new GUIContent(label));
            EditorGUIUtility.labelWidth = textDimensions.x + 1f;
            return EditorGUI.EnumPopup(drawRect, label, selected);
        }

        private Vector3 Vector3Field(Rect drawRect, string label, Vector3 value)
        {
            var textDimensions = GUI.skin.label.CalcSize(new GUIContent(label));
            EditorGUIUtility.labelWidth = textDimensions.x + 2f;
            return EditorGUI.Vector3Field(drawRect, label, value);
        }
    }
}
