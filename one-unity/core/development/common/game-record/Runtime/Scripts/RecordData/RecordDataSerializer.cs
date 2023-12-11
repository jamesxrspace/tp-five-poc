using System;
using System.Text;
using Newtonsoft.Json;

namespace TPFive.Game.Record
{
    public class RecordDataSerializer
    {
        public static byte[] Serialize(RecordData data)
        {
            string jsonString = JsonConvert.SerializeObject(new TypedRecordData(
                dataType: (int)data.GetType(),
                serializedData: data.Serialize()));
            return Encoding.UTF8.GetBytes(jsonString);
        }

        public static RecordData Deserialize(byte[] bytes)
        {
            var typedData = JsonConvert.DeserializeObject<TypedRecordData>(Encoding.UTF8.GetString(bytes));
            RecordData recordData = (RecordDataType)typedData.DataType switch
            {
                RecordDataType.Avatar => new AvatarRecordData(),
                RecordDataType.Audio => new AudioRecordData(),
                RecordDataType.Decoration => new DecorationRecordData(),
                RecordDataType.Scene => new SceneRecordData(),
                _ => throw new NotImplementedException(),
            };

            recordData.Deserialize(typedData.SerializedData);

            return recordData;
        }

        private class TypedRecordData
        {
            private readonly int dataType;
            private readonly byte[] serializedData;

            public TypedRecordData(int dataType, byte[] serializedData)
            {
                this.dataType = dataType;
                this.serializedData = serializedData;
            }

            public int DataType => dataType;

            public byte[] SerializedData => serializedData;
        }
    }
}