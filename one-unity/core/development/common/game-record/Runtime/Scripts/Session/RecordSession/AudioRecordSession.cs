using System;
using UnityEngine;

namespace TPFive.Game.Record
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.Logging;

    public class AudioRecordSession : RecordSession
    {
        private IEnumerable<AudioRecordData> audioData;

        private float startTime;

        public AudioRecordSession(ILogger logger)
            : base(logger)
        {
        }

        public override void Setup(RecordData[] data)
        {
            audioData = data.OfType<AudioRecordData>();
        }

        public override void Start()
        {
            base.Start();

            foreach (var data in audioData)
            {
                if (!data.BeforeRecord())
                {
                    throw new Exception("audioData not prepared");
                }
            }

            startTime = Time.time;
        }

        public override void Stop()
        {
            base.Stop();

            var endTime = Time.time;
            var audioLength = endTime - startTime;

            foreach (var data in audioData)
            {
                data.AfterRecord();

                data.Trim(startTime: 0, endTime: audioLength);
                data.DoTrim();
            }
        }

        public override IEnumerable<RecordData> GetRecordData()
        {
            var clone = audioData.ToList();
            return clone;
        }

        public override void Dispose()
        {
            audioData = null;
        }
    }
}