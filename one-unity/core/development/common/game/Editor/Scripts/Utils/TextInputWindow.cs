using UnityEditor;
using UnityEngine;

namespace TPFive.Game.Editor
{
    /// <summary>
    /// A simple text input window that allows users to input text and interact with it.
    /// </summary>
    public sealed class TextInputWindow : EditorWindow
    {
        private string inputText = string.Empty;
        private string message = "Please input text below!!!";

        private System.Action<string> onInput;

        /// <summary>
        /// Shows the text input window in the Unity Editor's menu under "Window/Text Input Window".
        /// </summary>
        [MenuItem("Window/Text Input Window")]
        public static void ShowWindow()
        {
            Show("InputWindow", "Please input text below!!!", (text) => { Debug.Log(text); });
        }

        /// <summary>
        /// Shows the text input window with a custom title, message, and input handling action.
        /// </summary>
        /// <param name="title">Title of the window.</param>
        /// <param name="message">Message displayed above the input field.</param>
        /// <param name="onInput">Action to be performed when input is confirmed.</param>
        public static void Show(string title, string message, System.Action<string> onInput)
        {
            // Get the existing open window or create a new one if none exists:
            TextInputWindow window = (TextInputWindow)GetWindow(typeof(TextInputWindow), false, title, true);
            window.onInput = onInput;
            if (!string.IsNullOrEmpty(message))
            {
                window.message = message;
            }

            window.Show();
        }

        /// <summary>
        /// Shows the text input window with a custom title, message, input text, and input handling action.
        /// </summary>
        /// <param name="title">Title of the window.</param>
        /// <param name="message">Message displayed above the input field.</param>
        /// <param name="inputText">Initial input text displayed in the input field.</param>
        /// <param name="onInput">Action to be performed when input is confirmed.</param>
        public static void Show(string title, string message, string inputText, System.Action<string> onInput)
        {
            // Get the existing open window or create a new one if none exists:
            TextInputWindow window = (TextInputWindow)GetWindow(typeof(TextInputWindow), false, title, true);
            window.onInput = onInput;
            if (!string.IsNullOrEmpty(message))
            {
                window.message = message;
            }

            if (!string.IsNullOrEmpty(inputText))
            {
                window.inputText = inputText;
            }

            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label(message, EditorStyles.boldLabel);
            GUI.SetNextControlName("InputText");
            inputText = EditorGUILayout.TextField(inputText);
            if (GUILayout.Button("Confirm and Close", GUILayout.MaxWidth(150), GUILayout.MaxHeight(20)))
            {
                try
                {
                    onInput?.Invoke(inputText);
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }

                Close();
            }

            if (GUILayout.Button("Confirm", GUILayout.MaxWidth(150), GUILayout.MaxHeight(20)))
            {
                try
                {
                    onInput?.Invoke(inputText);
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }

            if ((Event.current.control || Event.current.command) && Event.current.keyCode == KeyCode.V)
            {
                inputText = EditorGUIUtility.systemCopyBuffer;
                Repaint();
            }

            if (GUI.GetNameOfFocusedControl() == string.Empty)
            {
                GUI.FocusControl("InputText");
            }
        }
    }
}
