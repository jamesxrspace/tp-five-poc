namespace TPFive.Game.Record.Scene.SpawnPoint
{
    public interface ISpawnPointService
    {
        public int PointCount { get; }

        public void AddPoint(PointDesc pointDesc, bool isDefault = false);

        public void RemovePoint(PointDesc pointDesc);

        public void Clear();

        public PointDesc GetDefaultPoint();

        public PointDesc GetRandomPoint();

        public PointDesc GetPoint(int index);
    }
}
