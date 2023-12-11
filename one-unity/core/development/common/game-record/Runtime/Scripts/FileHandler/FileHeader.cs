using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TPFive.Game.Record
{
    public class FileHeader
    {
        private readonly List<HeaderChunk> chunks;

        public FileHeader()
        {
            List<HeaderChunk> templateChunks = new ()
            {
                new HeaderChunk(name: "version", length: 16, value: "1.0"),
                new HeaderChunk(name: "copyright", length: 20, value: "XRSPACE CO., LTD."),
            };
            chunks = templateChunks.Aggregate(new List<HeaderChunk>(), (acc, current) =>
            {
                acc.Add(new HeaderChunk(
                    name: current.Name,
                    value: current.Value,
                    length: current.Length,
                    position: acc.Count == 0 ? 0 : acc.Last().Length + acc.Last().Position));
                return acc;
            });
        }

        public byte[] Serialize()
        {
            byte[] result = new byte[chunks.Last().Position + chunks.Last().Length];
            chunks.ForEach(chunk =>
            {
                byte[] bytes = Encoding.UTF8.GetBytes(chunk.Value);
                Array.Copy(bytes, 0, result, chunk.Position, bytes.Length);
            });
            return result;
        }

        public FileHeaderValidateResult Validate(byte[] bytes)
        {
            // TBD: [TF3R-121] [Unity] header file validation should include version control/copyright/etc
            foreach (var chunk in chunks)
            {
                if (Encoding.UTF8.GetString(bytes.Skip(chunk.Position).Take(chunk.Length).ToArray()).Replace("\0", string.Empty) != chunk.Value)
                {
                    return new FileHeaderValidateResult() { Error = $"Header {chunk.Name} not match" };
                }
            }

            return new FileHeaderValidateResult();
        }

        private class HeaderChunk
        {
            private readonly string name;
            private readonly string value;
            private readonly int length;
            private readonly int position;

            public HeaderChunk(string name, string value, int length, int position = default)
            {
                this.name = name;
                this.value = value;
                this.length = length;
                this.position = position;
                if (Encoding.UTF8.GetBytes(value).Length > length)
                {
                    throw new Exception($"Header chunk {name} value length {Encoding.UTF8.GetBytes(value).Length} is larger than maximum byte size: {length}");
                }
            }

            public string Name => name;

            public string Value => value;

            public int Length => length;

            public int Position => position;
        }
    }
}
