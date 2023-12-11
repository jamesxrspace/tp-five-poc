using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace TPFive.Game.Assist.Entry
{
    [System.Serializable]
    public struct MeshData
    {
        public bool hasData;
        public int vertexCount;
        public int triangles;
        public int vertexBufferCount;
        public int streamBytes;
        public int indexFormat;

        public UnityEngine.Rendering.IndexFormat IndexFormat
        {
            get
            {
                return (UnityEngine.Rendering.IndexFormat)indexFormat;
            }
        }
    }

    [System.Serializable]
    public struct MemoryData
    {
        public static MemoryData Empty = new MemoryData()
        {
            name = "Null Object",
        };

        public string name;
        public string type;
        public double memory;
        public MeshData meshData;

        public static double GetRuntimeMemorySizeLong(UnityEngine.Object obj)
        {
            if (obj == null)
            {
                return 0.0;
            }

            return (double)Profiler.GetRuntimeMemorySizeLong(obj);
        }

        public static MemoryData GetMemoryData(UnityEngine.Object obj)
        {
            if (obj == null)
            {
                return MemoryData.Empty;
            }

            var name = obj.name;
            var type = obj.GetType().Name;
            MeshData meshData = default;
            if (type == "Mesh")
            {
                var mesh = obj as Mesh;
                var streamBytes = 0;
                for (int i = mesh.vertexBufferCount - 1; i >= 0; --i)
                {
                    streamBytes += mesh.GetVertexBufferStride(i);
                }

                meshData = new MeshData()
                {
                    hasData = true,
                    vertexCount = mesh.vertexCount,
                    triangles = mesh.triangles.Length / 3,
                    indexFormat = (int)mesh.indexFormat,
                    vertexBufferCount = mesh.vertexBufferCount,
                    streamBytes = streamBytes,
                };
            }

            var memory = GetRuntimeMemorySizeLong(obj);
            return new MemoryData()
            {
                name = name,
                type = type,
                memory = memory,
                meshData = meshData,
            };
        }
    }

    [System.Serializable]
    public class GameObjectMemoryData
    {
        public string name;
        public MemoryData[] memories;

        private const double KB = 1024.0;
        private const double MB = 1048576.0;
        private const double MBComparer = 52428.8;

        private static List<Component> monoBehaviours = new List<Component>();

        private static System.Text.StringBuilder sb = new System.Text.StringBuilder();

        private static GameObjectMemoryData empty = new GameObjectMemoryData()
        {
            name = "Null Object",
        };

        public static string HumanReadable(double value)
        {
            if (value > MBComparer)
            {
                return $"{value / MB:0.00} MB";
            }

            return $"{value / KB:0.00} KB";
        }

        public static GameObjectMemoryData Record(GameObject go, bool includeChildren)
        {
            if (go == null)
            {
                return GameObjectMemoryData.empty;
            }

            monoBehaviours.Clear();
            if (includeChildren)
            {
                go.GetComponentsInChildren(true, monoBehaviours);
            }
            else
            {
                go.GetComponents(monoBehaviours);
            }

            var memoryDatas = new List<MemoryData>();

            foreach (var mono in monoBehaviours)
            {
                if (mono is Transform)
                {
                    continue;
                }

                // Mesh Container
                if (mono is MeshFilter mf)
                {
                    memoryDatas.Add(MemoryData.GetMemoryData(mf.mesh));
                }
                else if (mono is SkinnedMeshRenderer smr)
                {
                    memoryDatas.Add(MemoryData.GetMemoryData(smr.sharedMesh));
                }

                // Material Container
                if (mono is Renderer r)
                {
                    var mats = r.sharedMaterials;
                    foreach (var mat in mats)
                    {
                        RecordMaterial(mat, memoryDatas);
                    }
                }

                memoryDatas.Add(MemoryData.GetMemoryData(mono));
            }

            return new GameObjectMemoryData()
            {
                name = go.name,
                memories = memoryDatas.ToArray(),
            };
        }

        public string ToReport()
        {
            sb.Length = 0;
            sb.AppendLine($"== GO: {name}");
            foreach (var m in memories)
            {
                sb.AppendLine($"\t{m.name} ({m.type}): {HumanReadable(m.memory)}");
                if (m.meshData.hasData)
                {
                    sb.AppendLine($"\t\tmesh v: {m.meshData.vertexCount}, t: {m.meshData.triangles}");
                }
            }

            sb.AppendLine("==");
            return sb.ToString();
        }

        private static void RecordTexture(Material mat, string name, List<MemoryData> memoryDatas)
        {
            if (mat.HasTexture(name))
            {
                var tex = mat.GetTexture(name);
                if (tex != null)
                {
                    memoryDatas.Add(MemoryData.GetMemoryData(tex));
                }
            }
        }

        private static void RecordMaterial(Material mat, List<MemoryData> memoryDatas)
        {
            if (mat.shader != null)
            {
                if (mat.HasProperty("_MatCap"))
                {
                    // all texture names used in avatar
                    // there is no ways to collect all shader property names in unity runtime
                    RecordTexture(mat, "_MaskTex", memoryDatas);
                    RecordTexture(mat, "_MainTex", memoryDatas);
                    RecordTexture(mat, "_MatCap", memoryDatas);
                    RecordTexture(mat, "_MatCap2", memoryDatas);
                }
                else
                {
                    var tex = mat.mainTexture;
                    if (tex != null)
                    {
                        memoryDatas.Add(MemoryData.GetMemoryData(tex));
                    }
                }
            }

            memoryDatas.Add(MemoryData.GetMemoryData(mat));
        }
    }
}
