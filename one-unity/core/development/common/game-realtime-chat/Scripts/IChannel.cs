using System;
using System.Collections.Generic;

namespace TPFive.Game.RealtimeChat
{
    public delegate void OnJoinChannelHandler(string channelId, uint uid, string userName);

    public delegate void OnLeaveChannelHandler(string channelId);

    public delegate void UserJoinedEventHandler(IParticipant user);

    public delegate void UserLeftEventHandler(IParticipant user);

    public enum ChannelState
    {
        /// <summary>
        /// The channel is in the process of joining
        /// </summary>
        Joining,

        /// <summary>
        /// The channel is joined
        /// </summary>
        Joined,

        /// <summary>
        /// The channel is in the process of leaving
        /// </summary>
        Leaving,

        /// <summary>
        /// The channel is left
        /// </summary>
        Left,
    }

    public interface IChannel : IDisposable
    {
        public event OnJoinChannelHandler OnChannelJoined;

        public event OnLeaveChannelHandler OnChannelLeft;

        public event UserJoinedEventHandler OnUserJoinedChannel;

        public event UserLeftEventHandler OnUserLeftChannel;

        ChannelId Id { get; }

        ChannelState State { get; }

        IEnumerable<IParticipant> Participants { get; }

        void Join();

        void Leave();

        bool MuteLocalAudio(bool isMute);
    }
}
