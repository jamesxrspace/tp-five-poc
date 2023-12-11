using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting;
using XR.Avatar;

[assembly: AlwaysLinkAssembly]

namespace TPFive.Game.Assist.Entry
{
    [RemoteController]
    public class AssistRemoteController : MonoBehaviour
    {
        // #if ENABLE_BENCHMARK_REMOTE
        //         [RuntimeInitializeOnLoadMethod]
        //         static void CreateInstance()
        //         {
        //             var go = new GameObject("AssistRemoteController").AddComponent<AssistRemoteController>();
        //             UnityEngine.Object.DontDestroyOnLoad(go);
        //         }
        // #endif
        // static AssistRemoteController instance;
        // public static AssistRemoteController Create()
        // {
        //     if (instance == null)
        //         instance = new GameObject("AssistRemoteController").AddComponent<AssistRemoteController>();
        //     return instance;
        // }

        /// <summary>
        /// This size is less than iPhone default setting (9xxx)
        /// </summary>
        public const int MaxBufferSize = 8192;
        public bool defaultEnableHeaderUI;
        public MethodDataList methodData = null;
        public EventDataList eventData = null;
        private static readonly RecorderData[] Recorders = new RecorderData[]
        {
            new RecorderData("Main Thread", ProfilerCategory.Internal, RecorderType.Time, 15),

            new RecorderData("Triangles Count", ProfilerCategory.Render, RecorderType.Count),
            new RecorderData("Vertices Count", ProfilerCategory.Render, RecorderType.Count),
            new RecorderData("SetPass Calls Count", ProfilerCategory.Render, RecorderType.Count),
            new RecorderData("Total Batches Count", ProfilerCategory.Render, RecorderType.Count),
            new RecorderData("Draw Calls Count", ProfilerCategory.Render, RecorderType.Count),

            new RecorderData("Texture Count", ProfilerCategory.Memory, RecorderType.Count),
            new RecorderData("Texture Memory", ProfilerCategory.Memory, RecorderType.Bytes),
            new RecorderData("Mesh Count", ProfilerCategory.Memory, RecorderType.Count),
            new RecorderData("Mesh Memory", ProfilerCategory.Memory, RecorderType.Bytes),
            new RecorderData("Material Count", ProfilerCategory.Memory, RecorderType.Count),
            new RecorderData("Material Memory", ProfilerCategory.Memory, RecorderType.Bytes),
            new RecorderData("AnimationClip Count", ProfilerCategory.Memory, RecorderType.Count),
            new RecorderData("AnimationClip Memory", ProfilerCategory.Memory, RecorderType.Bytes),
            new RecorderData("AudioClip Count", ProfilerCategory.Memory, RecorderType.Count),
            new RecorderData("AudioClip Memory", ProfilerCategory.Memory, RecorderType.Bytes),
            new RecorderData("Game Object Count", ProfilerCategory.Memory, RecorderType.Count),

            new RecorderData("System Used Memory", ProfilerCategory.Memory, RecorderType.Bytes),
            new RecorderData("Total Used Memory", ProfilerCategory.Memory, RecorderType.Bytes),
            new RecorderData("Total Reserved Memory", ProfilerCategory.Memory, RecorderType.Bytes),
        };

        private static readonly HashSet<System.Type> SkippingBaseTypes = new HashSet<Type>()
        {
            typeof(object),
            typeof(UnityEngine.Object),
            typeof(Component),
            typeof(Behaviour),
            typeof(MonoBehaviour),
        };

        private static HashSet<System.Type> benchmarkTypes = new HashSet<Type>()
        {
            typeof(MeshFilter),
            typeof(Renderer),
        };

        private System.Text.StringBuilder headerSB = new System.Text.StringBuilder();

        private UDPConnector connector = null;
        private System.Action delayAction = null;

        private Dictionary<RecorderData, ProfilerRecorder> recorders = new Dictionary<RecorderData, ProfilerRecorder>();
        private UIConsole console;
        private UIHeader header;

        public enum MessageDataType
        {
            Log = 0,
            /// <summary>
            /// Method is from c# script, event is from visual scripting
            /// </summary>
            ListAllMethodAndEvents,
            /// <summary>
            /// Method is from c# script
            /// </summary>
            CallMethod,
            /// <summary>
            /// Event is from visual scripting
            /// </summary>
            CallEvent,
            MemoryReport,
            Count,
        }

        public enum RecorderType
        {
            Bytes,
            Count,
            Time,
        }

        public enum ParameterType
        {
            None = 0,
            Int,
            Float,
            String,
        }

        public static byte[] ToBytes(MessageData data)
        {
            var json = JsonUtility.ToJson(data);
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            bytes = LZMAtools.CompressByteArrayToLZMAByteArray(bytes);
            if (bytes.Length > MaxBufferSize)
            {
                Debug.LogError($"data too long bytes: {bytes.Length}, json:\n{json}");
                return null;
            }

            return bytes;
        }

        public static double GetRecorderFrameAverage(ProfilerRecorder recorder)
        {
            var samplesCount = recorder.Capacity;
            if (samplesCount == 0)
            {
                return 0;
            }

            double r = 0;
            var samples = new List<ProfilerRecorderSample>(samplesCount);
            recorder.CopyTo(samples);
            for (var i = samples.Count - 1; i >= 0; --i)
            {
                r += samples[i].Value;
            }

            r /= samplesCount;

            return r;
        }

        public static MethodDataList CollectMethods(ICollection<MonoBehaviour> monos)
        {
            var methodList = new List<string>();
            var ownerNameList = new List<string>();
            var ownerInstanceList = new List<int>();
            var responseList = new List<bool>();
            var parameterTypeList = new List<int>();
            var ownerGODict = new Dictionary<int, GameObject>();
            foreach (var mono in monos)
            {
                var currentType = mono.GetType();
                if (SkippingBaseTypes.Contains(currentType))
                {
                    continue;
                }

                var instanceID = mono.gameObject.GetInstanceID();
                var ownerName = mono.name;
                do
                {
                    var infos = currentType.GetMembers(
                        System.Reflection.BindingFlags.Instance |
                        System.Reflection.BindingFlags.Public |
                        System.Reflection.BindingFlags.InvokeMethod);

                    foreach (var info in infos)
                    {
                        if (!(info is System.Reflection.MethodInfo methodInfo) || methodInfo.DeclaringType != currentType)
                        {
                            continue;
                        }

                        var parameters = methodInfo.GetParameters();
                        if (parameters.Length > 1)
                        {
                            continue;
                        }

                        var eParameterType = GetParameterType(parameters);
                        if (eParameterType != ParameterType.None)
                        {
                            methodList.Add(info.Name);
                            ownerNameList.Add($"{ownerName}.{mono.GetType().Name}");
                            ownerInstanceList.Add(instanceID);
                            ownerGODict[instanceID] = mono.gameObject;
                            responseList.Add(Attribute.IsDefined(methodInfo, typeof(RequireResponse)));
                            parameterTypeList.Add((int)eParameterType);
                        }
                    }

                    currentType = currentType.BaseType;
                }
                while (!SkippingBaseTypes.Contains(currentType));
            }

            return new MethodDataList()
            {
                methodNames = methodList.ToArray(),
                ownerNames = ownerNameList.ToArray(),
                ownerInstanceIDs = ownerInstanceList.ToArray(),
                owners = ownerGODict,
                requireResponse = responseList.ToArray(),
                parameterTypes = parameterTypeList.ToArray(),
            };
        }

        [RequireResponse]
        public void CollectDetailedMemory()
        {
            Send(new MessageData()
            {
                type = (int)MessageDataType.MemoryReport,
                strValue = CollectDetailedMemory(GetMemeoryCollection()),
            });
        }

        [RequireResponse]
        public void CollectSystemMemory()
        {
            var sb = new System.Text.StringBuilder();
            foreach (var item in recorders)
            {
                var recorderData = item.Key;
                var recorder = item.Value;
                if (recorder.Valid)
                {
                    sb.AppendLine(recorderData.GetReport(recorder));
                }
            }

            Send(new MessageData()
            {
                type = (int)MessageDataType.MemoryReport,
                strValue = sb.ToString(),
            });
        }

        public void CreateUIHeader()
        {
            header = TPFive.Game.Assist.Entry.UIHeader.Instance;

            if (header != null)
            {
                return;
            }

            var headerGO = new GameObject("UI Header");
            var camera = UnityEngine.Camera.main;
            headerGO.transform.SetParent(camera.transform, false);
            header = headerGO.AddComponent<TPFive.Game.Assist.Entry.UIHeader>();
            header.Is3D = true;
            header.cameraFor3D = camera;
            header.ShowUI = true;
            header.transform.position = camera.transform.position + (camera.transform.forward * 0.5f);
            header.transform.forward = -camera.transform.forward;
            header.transform.localPosition = new Vector3(0.405f, 0.22f, 0.5f);
        }

        public void DestroyUIHeader()
        {
            if (header != null)
            {
                Destroy(header.gameObject);
            }
        }

        public void CreateUIConsole()
        {
            if (console != null)
            {
                return;
            }

            var consoleGO = new GameObject("UI Console");
            var camera = UnityEngine.Camera.main;
            consoleGO.transform.SetParent(camera.transform, false);
            console = consoleGO.AddComponent<TPFive.Game.Assist.Entry.UIConsole>();
            console.Is3D = true;
            console.ReceiveDebugLog = true;
            console.cameraFor3D = camera;
            console.ShowUI = true;
            console.transform.position = camera.transform.position + (camera.transform.forward * 0.5f);
            console.transform.forward = -camera.transform.forward;
        }

        public void DestroyUIConsole()
        {
            if (console != null)
            {
                Destroy(console.gameObject);
            }
        }

        public void EnableConnectionVerbose()
        {
            if (connector != null)
            {
                connector.verbose = true;
            }
        }

        public void DisableConnectionVerbose()
        {
            if (connector != null)
            {
                connector.verbose = false;
            }
        }

        public EventDataList CollectCustomEvents(ICollection<ScriptMachine> sms)
        {
            var eventList = new List<string>();
            var ownerNameList = new List<string>();
            var ownerInstanceList = new List<int>();
            // var parameterTypeList = new List<int>();
            foreach (var sm in sms)
            {
                var ownerName = $"{sm.name} at GO: {sm.gameObject.name}";
                var instanceID = sm.GetInstanceID();
                foreach (var unit in sm.graph.units)
                {
                    if (!(unit is CustomEvent customEvent))
                    {
                        continue;
                    }

                    if (customEvent.argumentCount > 0)
                    {
                        continue;
                    }

                    var inputValue = customEvent.name;
                    var name = inputValue.unit.defaultValues[inputValue.key] as string;
                    if (string.IsNullOrEmpty(name))
                    {
                        continue;
                    }

                    ownerInstanceList.Add(instanceID);
                    ownerNameList.Add(ownerName);
                    eventList.Add(name);
                }
            }

            return new EventDataList()
            {
                eventNames = eventList.ToArray(),
                ownerNames = ownerNameList.ToArray(),
                ownerInstanceIDs = ownerInstanceList.ToArray(),
            };
        }

        protected void Start()
        {
            AsyncLoader.AutoInitAssetBundleSource();

            LogIps();
            RunListener();

            if (defaultEnableHeaderUI)
            {
                CreateUIHeader();
            }
        }

        protected void OnEnable()
        {
            if (recorders.Count != 0)
            {
                foreach (var recorder in recorders.Values)
                {
                    recorder.Dispose();
                }

                recorders.Clear();
            }

            foreach (var recorderData in Recorders)
            {
                recorders.Add(recorderData, ProfilerRecorder.StartNew(recorderData.category, recorderData.name, recorderData.capacity));
            }
        }

        protected void OnDisable()
        {
            foreach (var recorder in recorders.Values)
            {
                recorder.Dispose();
            }

            recorders.Clear();
        }

        protected void Update()
        {
            if (delayAction != null)
            {
                delayAction();
                delayAction = null;
            }

            if (header != null)
            {
                headerSB.Length = 0;
                for (int i = 0; i < 6; ++i)
                {
                    var fpsRecorder = recorders[Recorders[i]];
                    if (fpsRecorder.Valid)
                    {
                        headerSB.AppendLine(Recorders[i].GetReport(fpsRecorder));
                    }
                }

                header.Show(headerSB.ToString());
            }
        }

        protected void OnDestroy()
        {
            connector?.Dispose();
            connector = null;
        }

        private static ParameterType GetParameterType(System.Reflection.ParameterInfo[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
            {
                return ParameterType.None;
            }

            var parameterType = parameters[0].ParameterType;
            if (parameterType == typeof(string))
            {
                return ParameterType.String;
            }
            else if (parameterType == typeof(int))
            {
                return ParameterType.Int;
            }
            else if (parameterType == typeof(float))
            {
                return ParameterType.Float;
            }

            return ParameterType.None;
        }

        private bool OnDataIn(byte[] bytes)
        {
            bytes = LZMAtools.DecompressLZMAByteArrayToByteArray(bytes);
            string json = System.Text.Encoding.UTF8.GetString(bytes);
            var messageData = JsonUtility.FromJson<MessageData>(json);
            delayAction = () =>
            {
                ProcessDataIn(messageData);
            };
            return messageData.waitForResponse;
        }

        private void ProcessDataIn(MessageData recvData)
        {
            switch ((MessageDataType)recvData.type)
            {
                case MessageDataType.Log:
                    Debug.Log(recvData.strValue);
                    Send(new MessageData()
                    {
                        type = (int)MessageDataType.Log,
                        strValue = $"Data ({recvData.strValue}) Received!",
                    });
                    break;
                case MessageDataType.ListAllMethodAndEvents:
                    Send(new MessageData()
                    {
                        type = (int)MessageDataType.ListAllMethodAndEvents,
                        strValue = ListAllMethodAndEvents(),
                    });
                    break;
                case MessageDataType.CallMethod:
                    InvokeMethod(recvData);
                    break;
                case MessageDataType.CallEvent:
                    InvokeEvent(recvData);
                    break;
                default:
                    break;
            }
        }

        private GameObject FindObject(int instanceID)
        {
            if (methodData != null && methodData.owners != null && methodData.owners.ContainsKey(instanceID))
            {
                var go = methodData.owners[instanceID];
                if (go != null)
                {
                    return go;
                }
            }

            Debug.LogWarning($"FindObject Not find instanceID: {instanceID}");
            return gameObject;
        }

        private void InvokeMethod(MessageData data)
        {
            try
            {
                Debug.Log($"{nameof(InvokeMethod)}: {data.strValue}");
                var go = FindObject(data.instanceID);
                switch (data.parameterType)
                {
                    case (int)ParameterType.Int:
                        if (int.TryParse(data.parameter, out var intValue))
                        {
                            go.SendMessage(data.strValue, intValue);
                        }
                        else
                        {
                            Debug.LogError($"InvokeMethod Failed: input is not int ({data.parameter})");
                        }

                        break;
                    case (int)ParameterType.Float:
                        if (float.TryParse(data.parameter, out var floatValue))
                        {
                            go.SendMessage(data.strValue, floatValue);
                        }
                        else
                        {
                            Debug.LogError($"InvokeMethod Failed: input is not float ({data.parameter})");
                        }

                        break;
                    case (int)ParameterType.String:
                        go.SendMessage(data.strValue, data.parameter);
                        break;
                    default:
                        go.SendMessage(data.strValue);
                        break;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"{nameof(InvokeMethod)} with error: {e}");
            }
        }

        private void InvokeEvent(MessageData data)
        {
            if (string.IsNullOrEmpty(data.strValue))
            {
                Debug.LogWarning($"Cannot trigger empty event");
                return;
            }

            try
            {
                Debug.Log($"{nameof(InvokeEvent)}: {data.strValue}");
                var sms = CollectScriptMachine();
                foreach (var sm in sms)
                {
                    if (sm.GetInstanceID() == data.instanceID)
                    {
                        CustomEvent.Trigger(sm.gameObject, data.strValue);
                        break;
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"{nameof(InvokeEvent)} with error: {e}");
            }
        }

        private void LogIps()
        {
            Debug.Log("================== address list ==================");
            try
            {
                var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
                foreach (var address in host.AddressList)
                {
                    if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        Debug.Log(address);
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.Log($"get address failed: {e}");
            }

            Debug.Log("================== address list end ==================");
        }

        private void RunListener()
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
            connector.RunListener("<color=cyan>Listener</color>");
        }

        private void Send(MessageData sendData)
        {
            connector.Send(ToBytes(sendData), sendData.waitForResponse);
        }

        private ICollection<GameObject> GetMemeoryCollection()
        {
            var addedInstance = new HashSet<int>();
            var gos = new List<GameObject>();
            var trans = FindObjectsOfType<Transform>();
            foreach (var t in trans)
            {
                foreach (var mono in t.gameObject.GetComponentsInChildren<Component>())
                {
                    var currentType = mono.GetType();

                    foreach (var benchmarkType in benchmarkTypes)
                    {
                        if (!benchmarkType.IsAssignableFrom(currentType))
                        {
                            continue;
                        }

                        if (!addedInstance.Add(mono.gameObject.GetInstanceID()))
                        {
                            continue;
                        }

                        gos.Add(mono.gameObject);
                        break;
                    }
                }
            }

            return gos;
        }

        private string CollectDetailedMemory(ICollection<GameObject> gos)
        {
            var list = new List<GameObjectMemoryData>();
            foreach (var go in gos)
            {
                list.Add(GameObjectMemoryData.Record(go, false));
            }

            return new MemoryDataList()
            {
                data = list,
            }.ToReport();
        }

        private ICollection<MonoBehaviour> CollectControllers()
        {
            var typeSet = new HashSet<System.Type>();
            var monos = new List<MonoBehaviour>();

            var trans = FindObjectsOfType<Transform>();
            foreach (var t in trans)
            {
                foreach (var mono in t.gameObject.GetComponentsInChildren<MonoBehaviour>())
                {
                    if (mono == null)
                    {
                        continue;
                    }

                    var currentType = mono.GetType();
                    if (SkippingBaseTypes.Contains(currentType))
                    {
                        continue;
                    }

                    if (typeSet.Contains(currentType))
                    {
                        continue;
                    }

                    foreach (var attr in currentType.CustomAttributes)
                    {
                        if (attr.AttributeType == typeof(RemoteControllerAttribute))
                        {
                            typeSet.Add(currentType);
                            monos.Add(mono);
                            break;
                        }
                    }
                }
            }

            return monos;
        }

        private ICollection<ScriptMachine> CollectScriptMachine()
        {
            var sms = new HashSet<ScriptMachine>();
            var trans = FindObjectsOfType<Transform>();
            foreach (var t in trans)
            {
                foreach (var mono in t.gameObject.GetComponentsInChildren<ScriptMachine>())
                {
                    sms.Add(mono);
                }
            }

            return sms;
        }

        private string ListAllMethodAndEvents()
        {
            methodData = CollectMethods(CollectControllers());
            eventData = CollectCustomEvents(CollectScriptMachine());
            return JsonUtility.ToJson(
                new MethodAndEventList()
                {
                    methodData = methodData,
                    eventData = eventData,
                });
        }

        public struct RecorderData : IEquatable<RecorderData>
        {
            public string name;
            public RecorderType type;
            public ProfilerCategory category;
            public int capacity;

            public RecorderData(string name, ProfilerCategory category, RecorderType type, int capacity = 1)
            {
                this.name = name;
                this.type = type;
                this.category = category;
                this.capacity = capacity;
            }

            public override int GetHashCode()
            {
                return name.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (obj is RecorderData data)
                {
                    return name.Equals(data.name);
                }

                return false;
            }

            public bool Equals(RecorderData other)
            {
                return name == other.name;
            }

            public string GetReport(ProfilerRecorder recorder)
            {
                switch (type)
                {
                    case RecorderType.Bytes:
                        return $"{name}: {GameObjectMemoryData.HumanReadable(recorder.LastValueAsDouble)} ({recorder.LastValue})";
                    case RecorderType.Time:
                        return $"{name}: {GetRecorderFrameAverage(recorder) * 1e-6f:F1} ms";
                    default:
                        break;
                }

                return $"{name}: {recorder.LastValue}";
            }
        }

        [Serializable]
        public class MessageData
        {
            public int type;
            public string strValue;
            public int parameterType;
            public int instanceID;
            public string parameter;
            public bool waitForResponse;
        }

        [Serializable]
        public class MemoryDataList
        {
            public List<GameObjectMemoryData> data = new List<GameObjectMemoryData>();

            public string ToReport()
            {
                string report = string.Empty;
                foreach (var goMemory in data)
                {
                    report += goMemory.ToReport();
                }

                Debug.Log($"GameObjectMemoryReport:\n{report}");
                return report;
            }
        }

        [Serializable]
        public class MethodDataList
        {
            /// <summary>
            /// String list
            /// </summary>
            public string[] methodNames;
            /// <summary>
            /// Name of Owner.
            /// </summary>
            public string[] ownerNames;
            /// <summary>
            /// Instance ID list
            /// </summary>
            public int[] ownerInstanceIDs;
            /// <summary>
            /// Require response
            /// </summary>
            public bool[] requireResponse;
            /// <summary>
            /// Parameter Types
            /// </summary>
            public int[] parameterTypes;
            /// <summary>
            /// Dictionary holds corresponding between instanceID and gameobject.
            /// Note: Non-serialized, so only in client not server.
            /// </summary>
            public Dictionary<int, GameObject> owners;
        }

        [Serializable]
        public class EventDataList
        {
            public string[] eventNames;
            public string[] ownerNames;
            public int[] ownerInstanceIDs;
            // public int[] parameterTypes;
        }

        [Serializable]
        public class MethodAndEventList
        {
            public MethodDataList methodData;
            public EventDataList eventData;
        }
    }
}