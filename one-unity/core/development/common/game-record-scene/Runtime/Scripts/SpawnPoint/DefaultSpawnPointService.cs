using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using VContainer;
using Random = UnityEngine.Random;

namespace TPFive.Game.Record.Scene.SpawnPoint
{
    public class DefaultSpawnPointService : ISpawnPointService
    {
        private readonly ILogger log;
        private readonly List<PointDesc> pointList = new List<PointDesc>();

        private PointDesc defaultPoint;

        [Inject]
        public DefaultSpawnPointService(ILoggerFactory loggerFactory)
        {
            this.log = loggerFactory.CreateLogger<DefaultSpawnPointService>();
        }

        public int PointCount => pointList.Count;

        public void AddPoint(PointDesc pointDesc, bool isDefault = false)
        {
            if (pointList.Contains(pointDesc))
            {
                log.LogWarning(
                    "{Method}: the point({PointDesc}) is already added, is set default({IsDefault}) or not will be ignored",
                    nameof(AddPoint),
                    pointDesc,
                    isDefault);
                return;
            }

            pointList.Add(pointDesc);

            if (!isDefault)
            {
                return;
            }

            defaultPoint = pointDesc;
        }

        public void RemovePoint(PointDesc pointDesc)
        {
            pointList.Remove(pointDesc);
        }

        public void Clear()
        {
            pointList.Clear();
            defaultPoint = PointDesc.None;
        }

        public PointDesc GetDefaultPoint()
        {
            return defaultPoint;
        }

        public PointDesc GetRandomPoint()
        {
            if (pointList.Count < 1)
            {
                return PointDesc.None;
            }

            int index = Random.Range(0, pointList.Count - 1);

            return pointList[index];
        }

        public PointDesc GetPoint(int index)
        {
            if (index < 0 || pointList.Count <= index)
            {
                throw new IndexOutOfRangeException($"'{index}' is out of range.");
            }

            return pointList[index];
        }
    }
}
