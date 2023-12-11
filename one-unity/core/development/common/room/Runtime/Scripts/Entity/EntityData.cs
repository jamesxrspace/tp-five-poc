using Fusion;

namespace TPFive.Room
{
    public class EntityData<TNetEntityData>
        where TNetEntityData : struct, INetworkStruct
    {
        private readonly TNetEntityData _data;

        public EntityData(PlayerRef playerRef, ref TNetEntityData data)
        {
            PlayerRef = playerRef;
            _data = data;
        }

        public PlayerRef PlayerRef { get; private set; }

        public TNetEntityData Data => _data;
    }
}
