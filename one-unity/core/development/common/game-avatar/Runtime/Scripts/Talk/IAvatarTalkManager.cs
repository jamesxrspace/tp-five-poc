using System;

namespace TPFive.Game.Avatar.Talk
{
    public interface IAvatarTalkManager : IDisposable
    {
        void StartTalk();

        void StopTalk();
    }
}