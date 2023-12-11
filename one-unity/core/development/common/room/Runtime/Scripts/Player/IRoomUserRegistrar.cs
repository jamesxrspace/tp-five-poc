using System;

namespace TPFive.Room
{
    public interface IRoomUserRegistrar
    {
        void Register(IPlayer player);

        void Unregister(IPlayer player);
    }
}
