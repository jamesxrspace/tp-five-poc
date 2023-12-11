using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace TPFive.Game.Record
{
    public class FrameRateController
    {
        private float lastFrame;
        private float startTime;
        private float spf = 1 / 30f; // second per frame, default 30fps

        public void Init(float fps)
        {
            spf = 1 / fps;
            lastFrame = Time.time;
            startTime = lastFrame;
        }

        public void Reset()
        {
            lastFrame = Time.time;
            startTime = lastFrame;
        }

        public float GetDeltaTime()
        {
            return (float)Math.Round(Time.time - startTime, 3);
        }

        public bool ShouldSkip()
        {
            Assert.IsTrue(startTime != 0);

            if (lastFrame + spf <= Time.time)
            {
                lastFrame += spf;

                // TBD: [TF3R-120] [Unity] frame/motion/timestamp still have slightly difference between record and playback
                return false;
            }

            return true;
        }
    }
}
