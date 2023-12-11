using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TPFive.Game.Reel.Camera
{
    [CustomEditor(typeof(ReelDirectorConfig))]
    public sealed class ReelDirectorConfigEditor : Editor
    {
        private SerializedProperty cameraTags;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var saveToDisk = false;
            serializedObject.Update();

            if (GUILayout.Button("Refresh All Tags"))
            {
                RefreshAllTags();
                saveToDisk = true;
            }

            serializedObject.ApplyModifiedProperties();

            if (saveToDisk)
            {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
            }
        }

        private void OnEnable()
        {
            cameraTags = serializedObject.FindProperty("cameraTags");
        }

        private void RefreshAllTags()
        {
            var tags = LoadAllTags();
            cameraTags.arraySize = tags.Count;

            for (int i = 0; i < cameraTags.arraySize; i++)
            {
                SerializedProperty element = cameraTags.GetArrayElementAtIndex(i);
                element.objectReferenceValue = tags[i];
            }
        }

        private List<ReelCameraTag> LoadAllTags()
        {
            var tags = AssetDatabase.FindAssets($"t:{typeof(ReelCameraTag).FullName}")
                .Select(x => AssetDatabase.GUIDToAssetPath(x))
                .Select(x => AssetDatabase.LoadAssetAtPath<ReelCameraTag>(x))
                .ToList();
            return tags;
        }
    }
}
