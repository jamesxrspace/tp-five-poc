using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TPFive.Game.Record
{
    public sealed partial class AvatarRecordData
    {
        public override TimeBaseFrame DecodeOneFrame(byte[] buffer)
        {
            List<byte[]> dataList = Segment.DeserializeList(buffer);

            var timestamp = ByteConverter.ByteToFloat(dataList[0])[0];
            var morphers = ByteConverter.ByteToFloat(dataList[1]);
            var bodyPosition = ByteConverter.ByteToFloat(dataList[2]);
            var bodyRotation = ByteConverter.ByteToFloat(dataList[3]);
            var muscles = ByteConverter.ByteToFloat(dataList[4]);
            var targetPosition = ByteConverter.ByteToFloat(dataList[5]);
            var targetRotation = dataList.Count > 6 ? ByteConverter.ByteToFloat(dataList[6]) : new float[] { 0, 0, 0 };
            return new TimeBaseFrame(
                timestamp,
                new AvatarFrameFootage(
                    morphers,
                    new HumanPose()
                    {
                        bodyPosition = new Vector3(bodyPosition[0], bodyPosition[1], bodyPosition[2]),
                        bodyRotation = new Quaternion(bodyRotation[0], bodyRotation[1], bodyRotation[2], bodyRotation[3]),
                        muscles = muscles,
                    },
                    new Vector3(targetPosition[0], targetPosition[1], targetPosition[2]),
                    new Vector3(targetRotation[0], targetRotation[1], targetRotation[2])));
        }

        public override byte[] EncodeOneFrame(TimeBaseFrame data)
        {
            var footage = data.Frame as AvatarFrameFootage;
            HumanPose humanPose = footage.HumanPose;

            return Segment.SerializeList(new List<byte[]>()
            {
                ByteConverter.FloatToByte(new float[] { data.Timestamp }),
                ByteConverter.FloatToByte(footage.Morphers),
                ByteConverter.FloatToByte(Enumerable.Range(0, 3).Select(i => humanPose.bodyPosition[i]).ToArray()),
                ByteConverter.FloatToByte(Enumerable.Range(0, 4).Select(i => humanPose.bodyRotation[i]).ToArray()),
                ByteConverter.FloatToByte(humanPose.muscles),
                ByteConverter.FloatToByte(Enumerable.Range(0, 3).Select(i => footage.TargetPosition[i]).ToArray()),
                ByteConverter.FloatToByte(Enumerable.Range(0, 3).Select(i => footage.TargetRotation[i]).ToArray()),
            });
        }

        private class AvatarFrameFootage
        {
            private readonly float[] morphers;
            private readonly HumanPose humanPose;
            private readonly Vector3 targetPosition;
            private readonly Vector3 targetRotation;

            public AvatarFrameFootage(
                float[] morphers,
                HumanPose humanPose,
                Vector3 position,
                Vector3 rotation)
            {
                this.morphers = morphers;
                this.humanPose = humanPose;
                this.targetPosition = position;
                this.targetRotation = rotation;
            }

            public HumanPose HumanPose => humanPose;

            public float[] Morphers => morphers;

            public Vector3 TargetPosition => targetPosition;

            public Vector3 TargetRotation => targetRotation;
        }
    }
}
