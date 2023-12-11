using UnityEditor;
using UnityEngine;

namespace TPFive.Game.UI.Editor
{
    [CustomPropertyDrawer(typeof(Scale), true)]
    public class ScaleDrawer : PropertyDrawer
    {
        private readonly Color32 backgroundColor = EditorGUIUtility.isProSkin ? new (118, 15, 19, 255) : new (243, 119, 124, 255);

        private SerializedProperty enabled;
        private SerializedProperty animationType;
        private SerializedProperty startDelay;
        private SerializedProperty duration;
        private SerializedProperty scale;
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
            enabled.boolValue = EditorGUI.Toggle(drawRect, "Scale", enabled.boolValue);
            if (enabled.boolValue)
            {
                DrawScaleBlock(drawRect, (Anim.AnimationType)animationType.intValue);
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
            scale = property.FindPropertyRelative("value");
            easeType = property.FindPropertyRelative("easeType");
            ease = property.FindPropertyRelative("ease");
            animationCurve = property.FindPropertyRelative("animationCurve");
        }

        private float GetBlockHeight()
        {
            var height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            return enabled.boolValue ? (height * 3f) : height;
        }

        private void DrawScaleBlock(Rect drawRect, Anim.AnimationType animType)
        {
            // Line 1
            drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            drawRect.width /= 4f;
            startDelay.floatValue = FloatField(drawRect, "start delay", startDelay.floatValue);

            drawRect.x += drawRect.width;
            duration.floatValue = FloatField(drawRect, "duration", duration.floatValue);

            drawRect.x += drawRect.width;
            drawRect.width *= 2f;
            var scaleLabel = animType switch
            {
                Anim.AnimationType.In => "scale from",
                Anim.AnimationType.Out => "scale to",
                Anim.AnimationType.State => "scale by",
                _ => "invalid type",
            };
            scale.vector3Value = Vector3Field(drawRect, scaleLabel, scale.vector3Value);

            // Line 2
            drawRect.x -= drawRect.width;
            drawRect.width *= 2f;
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

        private Vector3 Vector3Field(Rect drawRect, string label, Vector3 value)
        {
            var textDimensions = GUI.skin.label.CalcSize(new GUIContent(label));
            EditorGUIUtility.labelWidth = textDimensions.x + 2f;
            return EditorGUI.Vector3Field(drawRect, label, value);
        }
    }
}
