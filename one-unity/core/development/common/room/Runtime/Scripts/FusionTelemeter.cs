using System;
using System.Diagnostics;
using Fusion;
using Microsoft.Extensions.Logging;
using VContainer;

namespace TPFive.Room
{
    public class FusionTelemeter : NetworkBehaviour, IAfterSpawned, IBeforeAllTicks
    {
        private const int KSampleIntervalInSec = 30;

        [Inject]
        private ILoggerFactory loggerFactory;

        private ILogger<FusionTelemeter> logger;

        private Stopwatch stopWatch = new Stopwatch();

        private int accuServerSyncCount;

        public float AvgServerSyncCount { get; private set; }

        private ILogger<FusionTelemeter> Logger
        {
            get
            {
                if (logger == null)
                {
                    logger = loggerFactory.CreateLogger<FusionTelemeter>();
                }

                return logger;
            }
        }

        public void AfterSpawned()
        {
            stopWatch.Start();
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            base.Despawned(runner, hasState);
            stopWatch.Reset();
        }

        public void BeforeAllTicks(bool resimulation, int tickCount)
        {
            if (!resimulation)
            {
                return;
            }

            accuServerSyncCount += 1;
        }

        protected void Update()
        {
            if (Runner.IsServer)
            {
                return;
            }

            if (stopWatch.Elapsed.Seconds < KSampleIntervalInSec)
            {
                return;
            }

            // Resolve the statistics
            AvgServerSyncCount = accuServerSyncCount / stopWatch.Elapsed.Seconds;
            Logger.LogDebug($"{nameof(AvgServerSyncCount)} of runner({Runner.GetInstanceID()}): {AvgServerSyncCount}");

            // Reset the counters
            accuServerSyncCount = 0;
            stopWatch.Restart();
        }
    }
}
