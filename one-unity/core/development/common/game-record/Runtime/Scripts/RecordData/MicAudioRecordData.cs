using System;
using UnityEngine;

namespace TPFive.Game.Record
{
    public sealed class MicAudioRecordData : AudioRecordData
    {
        private int lengthSec;

        public MicAudioRecordData(int lengthSec, string id = default, int frequency = 44100)
            : base(id: id, frequency: frequency)
        {
            this.lengthSec = lengthSec;
        }

        public override bool BeforeRecord()
        {
            if (!PermissionCheck.GetPermissionCheck())
            {
                throw new Exception("Microphone not found");
            }

            AudioSource.clip = Microphone.Start(null, false, lengthSec, Frequency);

            return true;
        }

        public override void AfterRecord()
        {
            Microphone.End(null);
        }
    }
}
