using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Animations;

namespace TPFive.Game.Avatar.HumanPosePlayable
{
    public struct HumanPoseAnimationJob : IAnimationJob, IDisposable
    {
        private static readonly float[] DefaultMuscles = new float[HumanTrait.MuscleCount];
        private static readonly MuscleHandle[] MuscleHandles;

        private NativeArray<float> muscles;
        private Vector3 position;
        private Quaternion rotation;

        static HumanPoseAnimationJob()
        {
            MuscleHandles = new MuscleHandle[HumanTrait.MuscleCount];
            MuscleHandle.GetMuscleHandles(MuscleHandles);
        }

        private HumanPoseAnimationJob(float[] muscles)
            : this(Vector3.zero, Quaternion.identity, muscles)
        {
        }

        private HumanPoseAnimationJob(Vector3 position, Quaternion rotation, float[] muscles)
        {
            this.position = position;
            this.rotation = rotation;
            this.muscles = new NativeArray<float>(muscles, Allocator.Persistent);

            if (this.muscles.Length != HumanTrait.MuscleCount)
            {
                throw new InvalidOperationException(
                    "Bad array size for HumanPoseAnimationJob.muscles. Size must equal HumanTrait.MuscleCount");
            }
        }

        public static HumanPoseAnimationJob Create()
        {
            return new HumanPoseAnimationJob(DefaultMuscles);
        }

        public void SetHumanPose(Vector3 position, Quaternion rotation, float[] muscles)
        {
            if (muscles.Length != HumanTrait.MuscleCount)
            {
                throw new InvalidOperationException(
                    "Bad array size for SetHumanPose(muscles). Size must equal HumanTrait.MuscleCount");
            }

            this.position = position;
            this.rotation = rotation;

            this.muscles.CopyFrom(muscles);
        }

        public readonly void ProcessRootMotion(AnimationStream stream)
        {
            // do something before processing the animation.
        }

        public void ProcessAnimation(AnimationStream stream)
        {
            var humanStream = stream.AsHuman();

            for (int i = 0; i < MuscleHandles.Length; ++i)
            {
                humanStream.SetMuscle(MuscleHandles[i], muscles[i]);
            }

            humanStream.bodyLocalPosition = position;
            humanStream.bodyLocalRotation = rotation;
        }

        public void Dispose()
        {
            muscles.Dispose();
        }
    }
}