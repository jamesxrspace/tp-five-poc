using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace TPFive.Game.Record
{
    // TBD: [TF3R-122] [Unity] record data should derive to framesbasedata & non-framebaedata
    public sealed partial class AvatarRecordData : FrameBaseRecordData
    {
        private HumanPoseHandler humanPoseHandler;
        private SkinnedMeshRenderer faceMeshRender;
        private int faceBlendShapeCount;

        // TBD: [TF3R-118] [Unity] trigger avatar object show/hide
        private GameObject recordTarget;

        public AvatarRecordData(string id = default)
            : base(id: id)
        {
        }

        public Vector3 InitialPosition
        {
            get
            {
                var firstFrame = Frames.FirstOrDefault()?.Frame;

                if (firstFrame == null)
                {
                    throw new System.Exception("Record Data's first Frame is null.");
                }

                var firstFrameFootage = firstFrame as AvatarFrameFootage;

                if (firstFrameFootage == null)
                {
                    throw new System.Exception("Record Data's first Frame is not AvatarFrameFootage.");
                }

                return firstFrameFootage.TargetPosition;
            }
        }

        public override RecordDataType GetType()
        {
            return RecordDataType.Avatar;
        }

        public void ExtractMotionFromAvatarModel(float deltaTime)
        {
            HumanPose humenPos = new ();
            humanPoseHandler.GetHumanPose(ref humenPos);
            Transform target = recordTarget.transform;

            Assert.IsTrue(Frames.Count == 0 || Frames.Last().Timestamp <= deltaTime);
            Frames.Add(new (deltaTime, new AvatarFrameFootage(GetBlendShapeWeights(faceMeshRender, faceBlendShapeCount), humenPos, target.position, target.eulerAngles)));
        }

        public bool ApplyMotionToAvatarModel(float deltaTime)
        {
            var avatarData = (AvatarFrameFootage)FindFrameByTimestamp(Frames, deltaTime);

            if (avatarData == null)
            {
                return false;
            }

            HumanPose humenPos = avatarData.HumanPose;
            humanPoseHandler.SetHumanPose(ref humenPos);
            recordTarget.transform.position = avatarData.TargetPosition;
            recordTarget.transform.eulerAngles = avatarData.TargetRotation;

            for (int i = 0; i < avatarData.Morphers.Length; i++)
            {
                faceMeshRender.SetBlendShapeWeight(i, avatarData.Morphers[i]);
            }

            return true;
        }

        public string GetAvatarFormat()
        {
            return Base64.Decode(Profile["AvatarFormat"].ToObject<string>());
        }

        public float GetLengthSec()
        {
            return Frames.LastOrDefault()?.Timestamp ?? 0f;
        }

        public void Bind(GameObject target, Animator animator, SkinnedMeshRenderer meshRender, int blendShapeCount, string avatarFormat = default)
        {
            recordTarget = target;
            humanPoseHandler = new HumanPoseHandler(animator.avatar, animator.transform);
            faceMeshRender = meshRender;
            faceBlendShapeCount = blendShapeCount;

            if (Frames.Count > 0)
            {
                RestoreInitialPosition(target);
            }

            if (avatarFormat != default)
            {
                SetProfile(new Dictionary<string, object>()
                {
                    { "AvatarFormat", Base64.Encode(avatarFormat) },
                });
            }
        }

        private void RestoreInitialPosition(GameObject target)
        {
            AvatarFrameFootage firstFrameFootage = Frames.First().Frame as AvatarFrameFootage;
            target.transform.position = firstFrameFootage.TargetPosition;
        }

        private float[] GetBlendShapeWeights(SkinnedMeshRenderer faceMorpher, int totalMorpher)
        {
            return Enumerable.Range(0, totalMorpher).Select(i => faceMorpher.GetBlendShapeWeight(i)).ToArray();
        }
    }
}
