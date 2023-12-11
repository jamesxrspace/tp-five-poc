using Fusion;
using TPFive.Game.User;

namespace TPFive.Room
{
    public struct NetPlayerData : INetworkStruct
    {
        // The length of user id (xrid) is 36.
        public NetworkString<_64> UserId;
        public NetworkString<_64> Nickname;

        public NetPlayerData(User user)
        {
            UserId = user.Uid;
            Nickname = user.Nickame;
        }
    }
}
