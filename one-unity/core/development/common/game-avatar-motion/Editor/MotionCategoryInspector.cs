using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace TPFive.Game.Avatar.Motion.Editor
{
    [CustomEditor(typeof(AvatarMotionCategory))]
    public class MotionCategoryInspector : UnityEditor.Editor
    {
        private readonly List<string> errors = new ();

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical();

            if (DrawDefaultInspector())
            {
                ValidateItems();
            }

            if (errors is { Count: > 0 })
            {
                foreach (var error in errors)
                {
                    EditorGUILayout.HelpBox(error, MessageType.Error);
                }
            }

            EditorGUILayout.EndVertical();
        }

        private void ValidateItems()
        {
            var category = (AvatarMotionCategory)target;

            errors.Clear();

            errors.AddRange(
                category.Motions.Where(x => x.Uid != Guid.Empty)
                    .GroupBy(x => x.Uid)
                    .Where(g => g.Count() > 1)
                    .Select(x => $"Found Duplicated Guid: {x.Key}"));

            if (category.Motions.Any(x => x.Uid == Guid.Empty))
            {
                errors.Add("Found Empty Guid");
            }
        }
    }
}