using System.Collections.Generic;
using Newtonsoft.Json;
using TPFive.Game.Resource;

namespace TPFive.Game.Record
{
    public class DecorationRecordData : RecordData
    {
        private const int DecorationIndex = 1;
        private JsonSerializerSettings _serializerSettings;

        public DecorationRecordData(string id = default)
            : base(id)
        {
            _serializerSettings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
            };
        }

        public DecorationRecordData(XRObject decoration)
            : this(decoration.Uid)
        {
            Decoration = decoration;
        }

        public XRObject Decoration { get; private set; }

        public override RecordDataType GetType()
        {
            return RecordDataType.Decoration;
        }

        public override void Deserialize(byte[] buffer)
        {
            var byteList = Segment.DeserializeList(buffer);
            base.Deserialize(byteList[0]);
            Decoration = ByteConverter.FromBytes<XRObject>(byteList[DecorationIndex]);
        }

        public override byte[] Serialize()
        {
            return Segment.SerializeList(new List<byte[]>()
            {
                base.Serialize(),
                ByteConverter.ToBytes(Decoration, _serializerSettings),
            });
        }

        public override string ToString()
        {
            return $"{Decoration}";
        }
    }
}