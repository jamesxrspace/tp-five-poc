using System;
using System.Collections.Generic;

namespace TPFive.Game.Record
{
    using System.Linq;
    using Microsoft.Extensions.Logging;
    using UniRx;
    using UnityEngine.Assertions;

    public class MotionPlaySession : PlaySession
    {
        private readonly FrameRateController frameRateController = new ();
        private IEnumerable<AvatarRecordData> avatarData;
        private HashSet<string> frozenItems = new HashSet<string>();
        private IDisposable disposable;
        private float duration;

        public MotionPlaySession(ILogger logger, float frameRate = 30f)
            : base(logger)
        {
            frameRateController.Init(frameRate);
        }

        public override void Setup(RecordData[] data)
        {
            Assert.IsTrue(Machine.State == State.StandBy);
            avatarData = data.OfType<AvatarRecordData>().ToArray();
            frameRateController.Reset();
            frozenItems = new HashSet<string>();

            // Get Duration
            var maxDuration = avatarData.Select(x => x.GetLengthSec()).Max();
            duration = maxDuration;
        }

        public override void Start()
        {
            base.Start();

            if (disposable == null)
            {
                disposable = Observable.EveryLateUpdate().Subscribe(_ => UpdateMotion());
            }
        }

        public override void Stop()
        {
            base.Stop();
            frameRateController.Reset();
        }

        public override float GetDuration()
        {
            return duration;
        }

        public override void Dispose()
        {
            disposable?.Dispose();
            avatarData = null;
            frozenItems?.Clear();
        }

        private void UpdateMotion()
        {
            // Stop does not unsubscribe UpdateMotion immediately, there is still chance that
            // UpdateMotion been invoked before the unsubscribe happen.
            if (!Machine.IsInState(State.Playing))
            {
                return;
            }

            foreach (var item in avatarData.Where(item => !frozenItems.Contains(item.Id)))
            {
                if (frameRateController.GetDeltaTime() > item.GetLengthSec())
                {
                    frozenItems.Add(item.Id);
                }

                _ = item.ApplyMotionToAvatarModel(frameRateController.GetDeltaTime());
            }
        }
    }
}