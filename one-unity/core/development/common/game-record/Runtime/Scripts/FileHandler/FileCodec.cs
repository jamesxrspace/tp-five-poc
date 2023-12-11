using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TPFive.Game.Record
{
    public class FileCodec
    {
        public static void Save(string filePath, IEnumerable<RecordData> recordData)
        {
            using FileStream stream = File.Create(filePath);

            List<byte[]> byteList = new List<byte[]>
            {
                new FileHeader().Serialize(),
            };

            foreach (var segment in recordData)
            {
                byteList.Add(RecordDataSerializer.Serialize(segment));
            }

            stream.Write(Segment.SerializeList(byteList));
        }

        public static List<RecordData> Load(string filePath)
        {
            using FileStream stream = File.OpenRead(filePath);
            return Load(stream);
        }

        public static List<RecordData> Load(Stream stream)
        {
            List<byte[]> byteList = Segment.DeserializeList(stream);

            FileHeaderValidateResult headerValidation = new FileHeader().Validate(byteList[0]);

            // TBD: [TF3R-121] [Unity] header file validation should include version control/copyright/etc
            if (!headerValidation.Ok)
            {
                throw new Exception(headerValidation.Error);
            }

            List<RecordData> result = new ();
            foreach (var segment in byteList.Skip(1))
            {
                result.Add(RecordDataSerializer.Deserialize(segment));
            }

            // TBD: TF3R-116 [Unity] Sign Reel data to prevent user tampering data
            return result;
        }
    }
}
