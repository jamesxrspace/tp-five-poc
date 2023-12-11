using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TPFive.Game.Assist.Entry
{
    using static AssistRemoteController;

    public class AssistWindow : EditorWindow
    {
        private static bool editorStateChanged = false;

        private string reportText;
        private bool drawServiceField = true;
        private bool drawMethodField = true;
        private bool drawEventField = true;
        private bool drawReportField = true;
        private bool showLast5Host = false;
        private Vector2 methodAndEventScroll;
        private Vector2 reportScroll;
        private string[] parameterTexts = null;
        private MethodDataList methods = null;
        private EventDataList events = null;
        private List<string> last5Host = new List<string>();
        private string ip = "127.0.0.1";
        private int port = UDPConnector.port;
        private UDPConnector connector = null;
        private bool connected;
        private GUIStyle titleStyle = null;
        private GUIStyle highlightStyle = null;

        private GUIStyle TitleStyle
        {
            get
            {
                if (titleStyle == null || titleStyle.fontStyle != FontStyle.Bold)
                {
                    titleStyle = new GUIStyle(GUI.skin.label)
                    {
                        fontStyle = FontStyle.Bold,
                        alignment = TextAnchor.MiddleCenter,
                    };
                    titleStyle.fontSize += 5;
                }

                return titleStyle;
            }
        }

        private GUIStyle HighlightStyle
        {
            get
            {
                if (highlightStyle == null)
                {
                    highlightStyle = new GUIStyle(EditorStyles.toolbarButton)
                    {
                        alignment = TextAnchor.MiddleLeft,
                    };
                }

                return highlightStyle;
            }
        }

        protected void OnEnable()
        {
            var json = EditorPrefs.GetString($"{nameof(AssistWindow)}.data", string.Empty);
            if (!string.IsNullOrEmpty(json))
            {
                JsonUtility.FromJsonOverwrite(json, this);
            }
        }

        protected void OnDisable()
        {
            EditorPrefs.SetString($"{nameof(AssistWindow)}.data", JsonUtility.ToJson(this));
        }

        protected void Update()
        {
            if (editorStateChanged)
            {
                editorStateChanged = false;
                methods = null;
                events = null;
                if (connected)
                {
                    connector?.Dispose();
                    connector = null;
                }

                connected = false;
            }
        }

        protected void OnGUI()
        {
            DrawToolbar();
            DrawService();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void OnAfterAssembliesLoaded()
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        private static void OnPlayModeChanged(PlayModeStateChange state)
        {
            editorStateChanged = true;
        }

        [MenuItem("TPFive/Assist/Open Assist Window")]
        private static void Open()
        {
            GetWindow<AssistWindow>().titleContent = new GUIContent("Assist Window");
        }

        private void DrawToolbar()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            if (ToggleButton(drawServiceField, "Service", EditorStyles.toolbarButton))
            {
                drawServiceField = !drawServiceField;
            }

            if (ToggleButton(drawMethodField, "Method", EditorStyles.toolbarButton))
            {
                drawMethodField = !drawMethodField;
            }

            if (ToggleButton(drawEventField, "Event", EditorStyles.toolbarButton))
            {
                drawEventField = !drawEventField;
            }

            if (ToggleButton(showLast5Host, "Last 5 Hosts", EditorStyles.toolbarButton))
            {
                showLast5Host = !showLast5Host;
            }

            GUILayout.EndHorizontal();
        }

        private void DrawServiceField()
        {
            if (!drawServiceField)
            {
                return;
            }

            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Service", TitleStyle);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Device IP:", GUILayout.Width(100f));
            ip = GUILayout.TextField(ip, GUILayout.MinWidth(100f));
            GUILayout.Label("Port:", GUILayout.Width(40f));
            port = EditorGUILayout.IntField(port, GUILayout.MaxWidth(70f));
            if (GUILayout.Button("Default port", GUILayout.Width(100f)))
            {
                port = UDPConnector.port;
            }

            EditorGUI.BeginDisabledGroup(connector == null);
            var verbose = connector != null ? connector.verbose : false;
            verbose = GUILayout.Toggle(verbose, "Verbose", GUI.skin.button, GUILayout.Width(75f));
            if (connector != null)
            {
                connector.verbose = verbose;
            }

            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();

            if (showLast5Host)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Label("Last 5 Hosts:");
                foreach (var address in last5Host)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(address);
                    if (GUILayout.Button("Select"))
                    {
                        ip = address;
                    }

                    GUILayout.EndHorizontal();
                }

                GUILayout.EndVertical();
            }

            if (connector == null)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Start service") || connected)
                {
                    connected = true;
                    var idx = last5Host.IndexOf(ip);
                    if (idx != -1)
                    {
                        last5Host.RemoveAt(idx);
                    }

                    last5Host.Add(ip);
                    if (last5Host.Count > 5)
                    {
                        last5Host.RemoveAt(0);
                    }

                    RunService(ip, port);
                }

                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button($"Stop Service: {ip}"))
                {
                    connected = false;
                    connector.Dispose();
                    connector = null;
                }

                if (GUILayout.Button("Send Test Log", GUILayout.Width(150f)))
                {
                    Send(new MessageData()
                    {
                        type = (int)MessageDataType.Log,
                        strValue = "Test Value",
                        waitForResponse = true,
                    });
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
        }

        private void DrawMethodField()
        {
            GUILayout.Label("Pragrammer Methods", TitleStyle);

            if (!drawMethodField)
            {
                return;
            }

            if (methods == null || methods.methodNames == null)
            {
                return;
            }

            if (parameterTexts == null || parameterTexts.Length != methods.methodNames.Length)
            {
                var tempArray = new string[methods.methodNames.Length];
                for (int i = methods.parameterTypes.Length - 1; i >= 0; --i)
                {
                    switch (methods.parameterTypes[i])
                    {
                        case (int)ParameterType.Float:
                        case (int)ParameterType.Int:
                            tempArray[i] = "0";
                            break;
                    }
                }

                if (parameterTexts != null && tempArray.Length >= parameterTexts.Length)
                {
                    parameterTexts.CopyTo(tempArray, 0);
                }

                parameterTexts = tempArray;
            }

            for (int i = methods.methodNames.Length - 1; i >= 0; --i)
            {
                GUILayout.BeginHorizontal(HighlightStyle);
                string method = methods.methodNames[i];
                GUILayout.Label($"{methods.ownerNames[i]}.{method}", HighlightStyle);
                var parameterType = methods.parameterTypes[i];
                if (parameterType != (int)ParameterType.None)
                {
                    var newText = EditorGUILayout.TextField(parameterTexts[i]);
                    if (parameterType == (int)ParameterType.Int)
                    {
                        if (int.TryParse(newText, out var val))
                        {
                            parameterTexts[i] = val.ToString();
                        }
                    }
                    else if (parameterType == (int)ParameterType.Float)
                    {
                        if (float.TryParse(newText, out var val))
                        {
                            parameterTexts[i] = val.ToString();
                        }
                    }
                    else
                    {
                        parameterTexts[i] = newText;
                    }
                }

                if (GUILayout.Button("Invoke", GUILayout.Width(200f)))
                {
                    Send(new MessageData()
                    {
                        type = (int)MessageDataType.CallMethod,
                        strValue = methods.methodNames[i],
                        parameterType = parameterType,
                        instanceID = methods.ownerInstanceIDs[i],
                        parameter = parameterTexts[i],
                        waitForResponse = methods.requireResponse[i],
                    });
                }

                GUILayout.EndHorizontal();
            }
        }

        private void DrawEventField()
        {
            GUILayout.Label("Visual Scripting Custom Events", TitleStyle);

            if (!drawEventField)
            {
                return;
            }

            if (events == null || events.eventNames == null)
            {
                return;
            }

            for (int i = events.eventNames.Length - 1; i >= 0; --i)
            {
                GUILayout.BeginHorizontal(HighlightStyle);
                var eventName = events.eventNames[i];
                GUILayout.Label($"{events.ownerNames[i]}.{eventName}", HighlightStyle);
                if (GUILayout.Button("Invoke", GUILayout.Width(200f)))
                {
                    Send(new MessageData()
                    {
                        type = (int)MessageDataType.CallEvent,
                        strValue = eventName,
                        instanceID = events.ownerInstanceIDs[i],
                    });
                }

                GUILayout.EndHorizontal();
            }
        }

        private void DrawReportField()
        {
            GUILayout.Label("Memory Report", TitleStyle);

            if (!drawReportField)
            {
                return;
            }

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Copy Report"))
            {
                GUIUtility.systemCopyBuffer = reportText;
            }

            if (GUILayout.Button("Clear Report"))
            {
                reportText = string.Empty;
            }

            GUILayout.EndHorizontal();
            if (reportText == null)
            {
                reportText = string.Empty;
            }

            reportText = EditorGUILayout.TextArea(reportText, GUILayout.MinHeight(300f));
        }

        private void DrawService()
        {
            DrawServiceField();

            methodAndEventScroll = GUILayout.BeginScrollView(methodAndEventScroll, EditorStyles.helpBox);
            GUILayout.Label("Commands", TitleStyle);
            bool disabled = connector == null;
            EditorGUI.BeginDisabledGroup(disabled);

            if (GUILayout.Button("Sync Commands"))
            {
                Send(new MessageData()
                {
                    type = (int)MessageDataType.ListAllMethodAndEvents,
                    waitForResponse = true,
                });
            }

            DrawMethodField();

            DrawEventField();

            EditorGUI.EndDisabledGroup();
            GUILayout.EndScrollView();

            reportScroll = GUILayout.BeginScrollView(reportScroll, EditorStyles.helpBox);

            DrawReportField();

            GUILayout.EndScrollView();
        }

        private void RunService(string ip, int port)
        {
            if (connector != null)
            {
                connector.Dispose();
                connector = null;
            }

            connector = new UDPConnector()
            {
                onDataIn = OnDataIn,
            };
            connector.RunService(ip, port, "<color=yellow>Sender</color>");
        }

        private void Send(MessageData sendData)
        {
            connector.Send(ToBytes(sendData), sendData.waitForResponse);
        }

        private bool OnDataIn(byte[] bytes)
        {
            bytes = LZMAtools.DecompressLZMAByteArrayToByteArray(bytes);
            string json = System.Text.Encoding.UTF8.GetString(bytes);
            var recv = JsonUtility.FromJson<MessageData>(json);
            switch ((MessageDataType)recv.type)
            {
                case MessageDataType.Log:
                    Debug.Log($"{(MessageDataType)recv.type} returns: {recv.strValue}");
                    break;
                case MessageDataType.ListAllMethodAndEvents:
                    try
                    {
                        var methodAndEventData = JsonUtility.FromJson<MethodAndEventList>(recv.strValue);
                        methods = methodAndEventData.methodData;
                        events = methodAndEventData.eventData;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log(recv.strValue);
                        Debug.LogError($"Parse String Array Failed: {e}");
                    }

                    break;
                case MessageDataType.MemoryReport:
                    Debug.Log($"Memory report received:\n{recv.strValue}");
                    reportText = recv.strValue;
                    break;
            }

            DelayRepaint();
            return recv.waitForResponse;
        }

        private void DelayRepaint()
        {
            EditorApplication.delayCall += Repaint;
        }

        private bool ToggleButton(bool was, string name, GUIStyle style, params GUILayoutOption[] options)
        {
            var later = GUILayout.Toggle(was, name, style, options);
            return later != was;
        }

        private bool ToggleButton(bool was, string name, params GUILayoutOption[] options)
        {
            var later = GUILayout.Toggle(was, name, options);
            return later != was;
        }
    }
}
