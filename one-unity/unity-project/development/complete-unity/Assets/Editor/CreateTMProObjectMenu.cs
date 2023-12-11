using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TPFive.Editor
{
    public class CreateTMProObjectMenu
    {
        private static Object m_TMP_EmojiTextUGUIScript;

        private static Object TMP_EmojiTextUGUIScript
        {
            get
            {
                if (m_TMP_EmojiTextUGUIScript == null)
                {
                    var script = "TMP_EmojiTextUGUI.cs";
                    var guids = AssetDatabase.FindAssets(Path.GetFileNameWithoutExtension(script));
                    foreach (var id in guids)
                    {
                        var path = AssetDatabase.GUIDToAssetPath(id);
                        if (path.Contains(script))
                        {
                            m_TMP_EmojiTextUGUIScript = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
                        }
                    }

                }
                return m_TMP_EmojiTextUGUIScript;
            }
        }

        private static readonly HashSet<string> ExcludedGameObjectName = new ()
        {
        };

        /// <summary>
        /// Create a TextMeshPro object that works with the Mesh Renderer, then add I2.Loc.Localize component on it.
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("GameObject/3D Object/Text - TextMeshPro (Localize)", false, 20)]
        static void CreateTextMeshProObjectPerform(MenuCommand menuCommand)
        {
            InvokeTMPro_CreateObjectMenuMethod("CreateTextMeshProObjectPerform", menuCommand);
            TryToAddLocalizeComponent(Selection.activeGameObject, false);
        }

        /// <summary>
        /// Create a TextMeshPro object that works with the CanvasRenderer, then add I2.Loc.Localize component on it.
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("GameObject/UI/Text - TextMeshPro (Localize)", false, 21)]
        private static void AddLocalizeText(MenuCommand menuCommand)
        {
            InvokeTMPro_CreateObjectMenuMethod("CreateTextMeshProGuiObjectPerform", menuCommand);
            TryToAddLocalizeComponent(Selection.activeGameObject, false);
        }

        [MenuItem("GameObject/UI/Button - TextMeshPro (Localize)", false, 22)]
        private static void AddLocalizeButton(MenuCommand menuCommand)
        {
            TMPro.EditorUtilities.TMPro_CreateObjectMenu.AddButton(menuCommand);
            TryToAddLocalizeComponent(Selection.activeGameObject, true);
        }

        [MenuItem("GameObject/UI/Dropdown - TextMeshPro (Localize)", false, 23)]
        private static void AddLocalizeDropdown(MenuCommand menuCommand)
        {
            TMPro.EditorUtilities.TMPro_CreateObjectMenu.AddDropdown(menuCommand);
            TryToAddLocalizeComponent(Selection.activeGameObject, true);
        }

        [MenuItem("GameObject/UI/Text - TextMeshPro (Emoji)", false, 41)]
        private static void AddEmojiText(MenuCommand menuCommand)
        {
            InvokeTMPro_CreateObjectMenuMethod("CreateTextMeshProGuiObjectPerform", menuCommand);
            ReplaceTMProTextWithEmojiText(Selection.activeGameObject, true);
        }

        [MenuItem("GameObject/UI/Button - TextMeshPro (Emoji)", false, 42)]
        private static void AddEmojiButton(MenuCommand menuCommand)
        {
            TMPro.EditorUtilities.TMPro_CreateObjectMenu.AddButton(menuCommand);
            ReplaceTMProTextWithEmojiText(Selection.activeGameObject, true);
        }

        [MenuItem("GameObject/UI/Dropdown - TextMeshPro (Emoji)", false, 43)]
        private static void AddEmojiDropdown(MenuCommand menuCommand)
        {
            TMPro.EditorUtilities.TMPro_CreateObjectMenu.AddDropdown(menuCommand);
            ReplaceTMProTextWithEmojiText(Selection.activeGameObject, true);
        }

        [MenuItem("GameObject/UI/Inputfield - TextMeshPro (Emoji)", false, 44)]
        private static void AddEmojiInputField(MenuCommand menuCommand)
        {
            InvokeTMPro_CreateObjectMenuMethod("AddTextMeshProInputField", menuCommand);
            ReplaceTMProTextWithEmojiText(Selection.activeGameObject, true);
        }

        [MenuItem("GameObject/UI/Text - TextMeshPro (Localize + Emoji)", false, 61)]
        private static void AddLocalizeEmojiText(MenuCommand menuCommand)
        {
            InvokeTMPro_CreateObjectMenuMethod("CreateTextMeshProGuiObjectPerform", menuCommand);
            ReplaceTMProTextWithEmojiText(Selection.activeGameObject, true);
            TryToAddLocalizeComponent(Selection.activeGameObject, true);
        }

        [MenuItem("GameObject/UI/Button - TextMeshPro (Localize + Emoji)", false, 62)]
        private static void AddLocalizeEmojiButton(MenuCommand menuCommand)
        {
            TMPro.EditorUtilities.TMPro_CreateObjectMenu.AddButton(menuCommand);
            ReplaceTMProTextWithEmojiText(Selection.activeGameObject, true);
            TryToAddLocalizeComponent(Selection.activeGameObject, true);
        }

        [MenuItem("GameObject/UI/Dropdown - TextMeshPro (Localize + Emoji)", false, 63)]
        private static void AddLocalizeEmojiDropdown(MenuCommand menuCommand)
        {
            TMPro.EditorUtilities.TMPro_CreateObjectMenu.AddDropdown(menuCommand);
            ReplaceTMProTextWithEmojiText(Selection.activeGameObject, true);
            TryToAddLocalizeComponent(Selection.activeGameObject, true);
        }

        private static void InvokeTMPro_CreateObjectMenuMethod(string methodName, MenuCommand command)
        {
            var bindingFlags = System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public;
            var method = typeof(TMPro.EditorUtilities.TMPro_CreateObjectMenu).GetMethod(methodName, bindingFlags);
            method.Invoke(null, new object[] { command });
        }

        private static void ReplaceTMProTextWithEmojiText(GameObject root, bool includeChidren)
        {
            if (includeChidren)
            {
                var textComponents = root.GetComponentsInChildren<TMP_Text>(true);
                foreach (var textComponent in textComponents)
                {
                    if (!ExcludedGameObjectName.Contains(textComponent.gameObject.name))
                    {
                        var go = textComponent.gameObject;
                        var serializedTextComponent = new SerializedObject(textComponent);
                        var scriptProperty = serializedTextComponent.FindProperty("m_Script");
                        scriptProperty.objectReferenceValue = TMP_EmojiTextUGUIScript;
                        serializedTextComponent.ApplyModifiedProperties();
                        go.name = $"{go.name}-Emoji";
                    }
                }
            }
            else
            {
                if (root.TryGetComponent<TextMeshProUGUI>(out var text))
                {
                    var serializedTextComponent = new SerializedObject(text);
                    var scriptProperty = serializedTextComponent.FindProperty("m_Script");
                    scriptProperty.objectReferenceValue = TMP_EmojiTextUGUIScript;
                    serializedTextComponent.ApplyModifiedProperties();
                    root.name = $"{root.name}-Emoji";
                }
            }
        }

        private static void TryToAddLocalizeComponent(GameObject root, bool includeChidren)
        {
            if (root.TryGetComponent<TMP_Text>(out var text))
            {
                text.gameObject.AddComponent<I2.Loc.Localize>();
            }

            if (includeChidren)
            {
                var textComponents = root.GetComponentsInChildren<TMP_Text>(true);
                foreach (var textComponent in textComponents)
                {
                    if (!ExcludedGameObjectName.Contains(textComponent.gameObject.name))
                        textComponent.gameObject.AddComponent<I2.Loc.Localize>();
                }
            }
        }
    }
}

