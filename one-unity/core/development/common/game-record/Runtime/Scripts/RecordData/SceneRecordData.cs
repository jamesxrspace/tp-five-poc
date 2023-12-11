using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TPFive.Game.Resource;

namespace TPFive.Game.Record
{
    public sealed class SceneRecordData : RecordData
    {
        private const int SceneDataIndex = 1;
        private const int DecorationsIndex = 2;
        private JsonSerializerSettings _serializerSettings;

        public SceneRecordData(string id = default)
            : base(id)
        {
            _serializerSettings = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, };
        }

        public SceneRecordData(XRObject scene)
            : this(scene.Uid)
        {
            Scene = scene;
        }

        public SceneRecordData(XRObject scene, Dictionary<string, XRObject> decorations)
            : this(scene.Uid)
        {
            Scene = scene;
            Decorations = decorations;
        }

        public XRObject Scene { get; private set; } = new ();

        public Dictionary<string, XRObject> Decorations { get; private set; } = new ();

        public override RecordDataType GetType()
        {
            return RecordDataType.Scene;
        }

        public override void Deserialize(byte[] buffer)
        {
            var byteList = Segment.DeserializeList(buffer);
            base.Deserialize(byteList[0]);

            Scene = ByteConverter.FromBytes<XRObject>(byteList[SceneDataIndex]);
            Decorations = ByteConverter.FromBytes<Dictionary<string, XRObject>>(byteList[DecorationsIndex]);
        }

        public override byte[] Serialize()
        {
            return Segment.SerializeList(new List<byte[]>()
            {
                base.Serialize(),
                ByteConverter.ToBytes(Scene, _serializerSettings),
                ByteConverter.ToBytes(Decorations, _serializerSettings),
            });
        }

        public override string ToString()
        {
            // Use linq to print decorations context.
            var decorationList = Decorations.Select(item => item.Value).ToList();
            var printDecorationList = "Decoration List : \n" + string.Join('\n', decorationList);
            return $"Id={Id}, Tags={string.Join(",", Tags)}, Scene={Scene}, Decorations={printDecorationList}";
        }

        public void AddDecoration(XRObject decoration)
        {
            Decorations.Add(decoration.Uid, decoration);
        }

        public ReelSceneDesc ToReelSceneDesc()
        {
            return new ReelSceneDesc()
            {
                Name = Scene.ObjectName,
                BundleID = Scene.BundleId,
                BuildInAddressableKey = Scene.BuildInKey,
                Template = Enum.Parse<ReelSceneDesc.TemplateType>(Scene.ObjectType),
            };
        }

        public void FromReelSceneDesc(ReelSceneDesc desc)
        {
            Scene.ObjectName = desc.Name;
            Scene.BundleId = desc.BundleID;
            Scene.BuildInKey = desc.BuildInAddressableKey;
            Scene.ObjectType = desc.Template.ToString();
        }
    }
}