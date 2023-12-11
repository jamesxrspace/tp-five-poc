using System;
using Fusion;

namespace TPFive.Room
{
    public interface IRoom : IDisposable
    {
        event Action OnStarted;

        GameMode Mode { get; }

        string SpaceID { get; }

        PhotonRegion Region { get; }

        string RoomID { get; }

        string SceneKey { get; }

        void Open(GameMode mode, string spaceID, string roomID, string sceneKey, PhotonRegion region, ushort? port = null);

        void Close();
    }
}