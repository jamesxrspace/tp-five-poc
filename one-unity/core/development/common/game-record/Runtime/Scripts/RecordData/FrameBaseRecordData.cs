using System.Collections.Generic;
using System.Linq;

namespace TPFive.Game.Record
{
    public abstract class FrameBaseRecordData : RecordData
    {
        private readonly List<TimeBaseFrame> frames;

        public FrameBaseRecordData(string id)
            : base(id: id)
        {
            frames = new List<TimeBaseFrame>();
        }

        protected List<TimeBaseFrame> Frames => frames;

        public abstract TimeBaseFrame DecodeOneFrame(byte[] buffer);

        public abstract byte[] EncodeOneFrame(TimeBaseFrame data);

        public override byte[] Serialize()
        {
            return Segment.SerializeList(new List<byte[]>()
            {
                base.Serialize(),
                Segment.SerializeList(
                    Frames.Select(frame => EncodeOneFrame(frame)).ToList()),
            });
        }

        public override void Deserialize(byte[] buffer)
        {
            var byteList = Segment.DeserializeList(buffer);
            base.Deserialize(byteList[0]);
            ResetFrames(Segment.DeserializeList(byteList[1]).Select(frame => DecodeOneFrame(frame)).ToList());
        }

        protected static object FindFrameByTimestamp(List<TimeBaseFrame> frames, float deltaTimestamp)
        {
            // TBD : [TF3R-61] [Unity][Record service] Fine tune avatar motion playback at the exact point in time
            var comp = Comparer<TimeBaseFrame>.Create((x, y) => x.Timestamp.CompareTo(y.Timestamp));
            int index = frames.BinarySearch(new TimeBaseFrame(deltaTimestamp, null), comp);

            index = index < 0 ? ~index - 1 : index;
            if (index < 0 || index >= frames.Count)
            {
                return null;
            }

            return frames[index].Frame;
        }

        protected void ResetFrames(List<TimeBaseFrame> frames)
        {
            this.frames.Clear();
            this.frames.AddRange(frames);
        }
    }
}
