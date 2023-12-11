using System;

namespace TPFive.Game.Avatar.Tracking
{
    public interface IHumanPoseSynchronizer : IDisposable
    {
        bool Enabled { get; set; }

        void SetHumanPoseProvider(Func<UnityEngine.HumanPose> humanPoseProvider);
    }
}