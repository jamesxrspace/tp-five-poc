using Fusion;

namespace TPFive.Room
{
    public interface IRoomManager
    {
        GameMode Mode { get; }

        IRoom Room { get; }
    }
}
