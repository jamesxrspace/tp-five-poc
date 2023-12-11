using System;
using System.Collections.Generic;

namespace TPFive.Game.Record
{
    using System.Linq;
    using Microsoft.Extensions.Logging;
    using UniRx;

    public class MotionRecordSession : RecordSession
    {
        private readonly FrameRateController frameRateController = new ();
        private IDisposable disposable;
        private IEnumerable<AvatarRecordData> avatarData;

        public MotionRecordSession(ILogger logger, float frameRate = 30f)
            : base(logger)
        {
            frameRateController.Init(frameRate);
        }

        public override void Setup(RecordData[] recordData)
        {
            frameRateController.Reset();
            avatarData = recordData.OfType<AvatarRecordData>();
        }

        public override void Start()
        {
            base.Start();
            disposable?.Dispose();

            // MusicToMotion Service update avatar motion in LateUpdate hook,
            // so we need to do record in EndOfFrame hook, which is called after LateUpdate
            disposable = Observable.EveryEndOfFrame().Subscribe(_ => RecordMotion());
            if (disposable == null)
            {
                throw new Exception("MotionSession late update subscription failed");
            }
        }

        public override void Stop()
        {
            base.Stop();
            disposable?.Dispose();
        }

        public override IEnumerable<RecordData> GetRecordData()
        {
            var clone = avatarData.ToList();
            return clone;
        }

        public override void Dispose()
        {
            avatarData = null;
            disposable?.Dispose();
        }

        private void RecordMotion()
        {
            if (!frameRateController.ShouldSkip())
            {
                foreach (var data in avatarData)
                {
                    data.ExtractMotionFromAvatarModel(frameRateController.GetDeltaTime());
                }
            }
        }
    }
}