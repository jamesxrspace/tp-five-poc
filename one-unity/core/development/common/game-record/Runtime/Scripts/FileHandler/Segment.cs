using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TPFive.Game.Record
{
    public class Segment
    {
        // Segment header size is 4 bytes, it only contains segment size(boundary)
        private const int SegmentHeaderSize = sizeof(int);

        // Segment boundary size should be less than 2GB to avoid overflow (size limit should be discuss)
        // TBD: [TF3R-126] [Unity] define the maximum size of xrs file and each segment
        private const int SegmentBoundarySize = int.MaxValue;

        public static int PredictLength(byte[] data)
        {
            return BitConverter.GetBytes(data.Length).Length + data.Length;
        }

        public static byte[] SerializeList(List<byte[]> byteList)
        {
            using MemoryStream stream = new MemoryStream(byteList.Sum(Segment.PredictLength));
            byteList.ForEach(x =>
            {
                Segment.Write(stream, x);
            });
            return stream.ToArray();
        }

        public static List<byte[]> DeserializeList(Stream stream)
        {
            var result = new List<byte[]>();
            while (stream.Position < stream.Length)
            {
                result.Add(Segment.Read(stream));
            }

            return result;
        }

        public static List<byte[]> DeserializeList(byte[] buffer)
        {
            using MemoryStream stream = new MemoryStream(buffer);
            return DeserializeList(stream);
        }

        private static void Write(Stream stream, byte[] data)
        {
            var boundary = BitConverter.GetBytes(data.Length);
            stream.Write(boundary, 0, boundary.Length);
            stream.Write(data, 0, data.Length);
        }

        private static byte[] Read(Stream stream)
        {
            byte[] header = new byte[SegmentHeaderSize];
            stream.Read(header, 0, header.Length);
            var boundary = BitConverter.ToInt32(header, 0);
            if (boundary > SegmentBoundarySize)
            {
                throw new Exception($"Segment boundary {boundary} is larger than {SegmentBoundarySize}");
            }

            byte[] data = new byte[boundary];
            stream.Read(data, 0, boundary);
            return data;
        }
    }
}
