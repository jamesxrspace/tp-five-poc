using System;
using System.Text;
using Newtonsoft.Json;

namespace TPFive.Game.Record
{
    // To convert a byte array to an object and vice versa.
    public class ByteConverter
    {
        public static float[] ByteToFloat(byte[] source)
        {
            var target = new float[source.Length / sizeof(float)];
            Buffer.BlockCopy(source, 0, target, 0, source.Length);
            return target;
        }

        public static byte[] FloatToByte(float[] source)
        {
            var target = new byte[source.Length * sizeof(float)];
            Buffer.BlockCopy(source, 0, target, 0, target.Length);
            return target;
        }

        public static byte[] ToBytes(object data, JsonSerializerSettings settings = null)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data, settings));
        }

        public static T FromBytes<T>(byte[] bytes, JsonSerializerSettings settings = null)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(bytes), settings);
        }
    }
}
