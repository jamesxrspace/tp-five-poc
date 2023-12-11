using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TPFive.Game.Assist.Entry
{
    using Camera = UnityEngine.Camera;
    using Text = UnityEngine.UI.Text;

    public class UIConsole : UITextPanel
    {
        public bool ReceiveDebugLog = false;

        /// <summary>
        /// Magic number for unity text string max size. (if > 15xxx will not render this text)
        /// </summary>
        private const int MaxTextSize = 15000;
        private static UIConsole instance;

        private List<string> logs = new List<string>();
        private string fullText;
        private System.Text.StringBuilder sb = new System.Text.StringBuilder();

        public void Log(string line)
        {
            logs.Add(line);
            sb.Length = 0;
            for (int i = logs.Count - 1; i >= 0; --i)
            {
                sb.AppendLine(logs[i]);
            }

            if (showUI && !uiCreated)
            {
                CreateUI();
            }

            if (sb.Length > MaxTextSize)
            {
                uiText.text = sb.ToString(sb.Length - MaxTextSize, MaxTextSize);
            }
            else
            {
                uiText.text = sb.ToString();
            }
        }

        public void Log(string condition, string stackTrace, LogType type)
        {
            sb.Length = 0;
            switch (type)
            {
                case LogType.Log:
                    break;
                case LogType.Warning:
                    sb.Append("<color=yellow>");
                    break;
                default:
                    sb.Append("<color=red>");
                    break;
            }

            sb.AppendLine(condition);
            sb.AppendLine(stackTrace);

            if (type != LogType.Log)
            {
                sb.Append("</color>");
            }

            Log(sb.ToString());
        }

        protected void OnEnable()
        {
            instance = this;
        }

        protected void OnDisable()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

        [RuntimeInitializeOnLoadMethod]
        private static void SetLog()
        {
            Application.logMessageReceived += OnLogMessageReceived;
        }

        private static void OnLogMessageReceived(string condition, string stackTrace, LogType type)
        {
            if (instance != null && instance.ReceiveDebugLog)
            {
                instance.Log(condition, stackTrace, type);
            }
        }
    }
}