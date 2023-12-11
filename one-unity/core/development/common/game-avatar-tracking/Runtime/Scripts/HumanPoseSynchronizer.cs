using UniRx;
using UnityEngine;

namespace TPFive.Game.Avatar.Tracking
{
    public sealed class HumanPoseSynchronizer : IHumanPoseSynchronizer
    {
        private readonly HumanPoseHandler humanPoseHandler;

        private System.Func<UnityEngine.HumanPose> humanPoseProvider;
        private bool disposed;
        private bool enabled;
        private System.IDisposable updateSubscription;

        public HumanPoseSynchronizer(HumanPoseHandler humanPoseHandler)
        {
            this.humanPoseHandler = humanPoseHandler ?? throw new System.ArgumentNullException(nameof(humanPoseHandler));
        }

        ~HumanPoseSynchronizer()
        {
            Dispose(false);
        }

        public bool Enabled
        {
            get => enabled;
            set
            {
                if (enabled == value)
                {
                    return;
                }

                enabled = value;

                OnStateChanged();
            }
        }

        public void SetHumanPoseProvider(System.Func<UnityEngine.HumanPose> humanPoseProvider)
        {
            this.humanPoseProvider = humanPoseProvider ?? throw new System.ArgumentNullException(nameof(humanPoseProvider));
        }

        void System.IDisposable.Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        private void LateUpdate(long count)
        {
            var nullableHumanPose = humanPoseProvider?.Invoke();
            if (nullableHumanPose == null)
            {
                return;
            }

            var humanPose = nullableHumanPose.Value;
            humanPoseHandler.SetHumanPose(ref humanPose);
        }

        private void OnStateChanged()
        {
            if (Enabled)
            {
                updateSubscription = Observable.EveryLateUpdate().Subscribe(LateUpdate);
            }
            else
            {
                updateSubscription?.Dispose();
            }
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                updateSubscription?.Dispose();
                humanPoseHandler?.Dispose();
            }

            disposed = true;
        }
    }
}