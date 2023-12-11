using UnityEditor;
using UnityEngine;

namespace TPFive.Game.UI.Editor
{
    [CustomPropertyDrawer(typeof(Anim), true)]
    public class AnimDrawer : PropertyDrawer
    {
        private readonly Color32 backgroundColor = new (231, 231, 231, 255);

        private SerializedProperty animationType;

        private SerializedProperty move;
        private SerializedProperty moveAnimationType;

        private SerializedProperty rotate;
        private SerializedProperty rotateAnimationType;

        private SerializedProperty scale;
        private SerializedProperty scaleAnimationType;

        private SerializedProperty fade;
        private SerializedProperty fadeAnimationType;

        private MoveDrawer moveDrawer;
        private RotateDrawer rotateDrawer;
        private ScaleDrawer scaleDrawer;
        private FadeDrawer fadeDrawer;

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

            // Split Line
            drawRect = DrawLine(drawRect, Color.black);
            drawRect.height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            // Draw Background
            Rect rect = position;
            rect.height = EditorGUIUtility.singleLineHeight + (EditorGUIUtility.standardVerticalSpacing * 2f);
            EditorGUI.DrawRect(rect, backgroundColor);

            // Animation Type
            animationType.intValue = (int)(Anim.AnimationType)EditorGUI.EnumPopup(
                drawRect,
                "Animation Type",
                (Anim.AnimationType)animationType.intValue);
            moveAnimationType.intValue = animationType.intValue;
            rotateAnimationType.intValue = animationType.intValue;
            scaleAnimationType.intValue = animationType.intValue;
            fadeAnimationType.intValue = animationType.intValue;
            drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            // Split Line
            drawRect = DrawLine(drawRect, Color.black);

            // Move
            moveDrawer.OnGUI(drawRect, move, GUIContent.none);
            drawRect.y += EditorGUI.GetPropertyHeight(move);

            // Split Line
            drawRect = DrawLine(drawRect, Color.black);

            // Rotate
            rotateDrawer.OnGUI(drawRect, rotate, GUIContent.none);
            drawRect.y += EditorGUI.GetPropertyHeight(rotate);

            // Split Line
            drawRect = DrawLine(drawRect, Color.black);

            // Scale
            scaleDrawer.OnGUI(drawRect, scale, GUIContent.none);
            drawRect.y += EditorGUI.GetPropertyHeight(scale);

            // Split Line
            drawRect = DrawLine(drawRect, Color.black);

            // Fade
            fadeDrawer.OnGUI(drawRect, fade, GUIContent.none);
            drawRect.y += EditorGUI.GetPropertyHeight(fade);

            // Split Line
            DrawLine(drawRect, Color.black);

            // End Property
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight + (4f * EditorGUIUtility.standardVerticalSpacing);

            FindProperties(property);

            height += EditorGUI.GetPropertyHeight(move)
                + EditorGUI.GetPropertyHeight(rotate)
                + EditorGUI.GetPropertyHeight(scale)
                + EditorGUI.GetPropertyHeight(fade);
            return height;
        }

        public override bool CanCacheInspectorGUI(SerializedProperty property)
        {
            return false;
        }

        private void Init(SerializedProperty property)
        {
            initialized = true;

            FindProperties(property);

            moveDrawer = new MoveDrawer();
            rotateDrawer = new RotateDrawer();
            scaleDrawer = new ScaleDrawer();
            fadeDrawer = new FadeDrawer();
        }

        private void FindProperties(SerializedProperty property)
        {
            animationType = property.FindPropertyRelative("type");

            move = property.FindPropertyRelative("move");
            moveAnimationType = property.FindPropertyRelative("move.animationType");

            rotate = property.FindPropertyRelative("rotate");
            rotateAnimationType = property.FindPropertyRelative("rotate.animationType");

            scale = property.FindPropertyRelative("scale");
            scaleAnimationType = property.FindPropertyRelative("scale.animationType");

            fade = property.FindPropertyRelative("fade");
            fadeAnimationType = property.FindPropertyRelative("fade.animationType");
        }

        private Rect DrawLine(Rect drawRect, Color color, float thickness = 1f)
        {
            Rect line = drawRect;
            line.height = thickness;
            EditorGUI.DrawRect(line, color);

            drawRect.y += EditorGUIUtility.standardVerticalSpacing;
            return drawRect;
        }
    }
}
