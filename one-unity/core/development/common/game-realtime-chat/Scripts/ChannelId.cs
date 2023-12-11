using System;
using System.Collections.Generic;

namespace TPFive.Game.RealtimeChat
{
    public class ChannelId : IEquatable<ChannelId>
    {
        private const string ValidChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!#$%&()+-:;<=.>?@[]^_{}|~,";
        private static readonly HashSet<char> ValidCharHashSet = new HashSet<char>(ValidChars.ToCharArray());

        private readonly string _channelName;
        private readonly bool _isSpaceChannel;
        private readonly string _spaceId;
        private readonly string _roomId;

        public ChannelId(string channelName)
        {
            if (string.IsNullOrEmpty(channelName))
            {
                throw new ArgumentNullException(nameof(channelName));
            }

            if (!IsValidName(channelName))
            {
                throw new ArgumentException(
                    $"{GetType().Name}: Argument contains one, or more, invalid characters, or the length of the name exceeds 64 bytes.",
                    nameof(channelName));
            }

            _isSpaceChannel = false;
            _channelName = channelName;
        }

        public ChannelId(string spaceId, string roomId)
        {
            if (string.IsNullOrEmpty(spaceId))
            {
                throw new ArgumentNullException(nameof(spaceId));
            }

            if (string.IsNullOrEmpty(roomId))
            {
                throw new ArgumentNullException(nameof(roomId));
            }

            string channelName = roomId;
            if (!IsValidName(channelName))
            {
                throw new ArgumentException(
                    $"{GetType().Name}: Argument contains one, or more, invalid characters, or the length of the name exceeds 64 bytes.",
                    nameof(channelName));
            }

            _isSpaceChannel = true;
            _channelName = channelName;
            _spaceId = spaceId;
            _roomId = roomId;
        }

        public string ChannelName => _channelName;

        public bool IsSpaceChannel => _isSpaceChannel;

        public string SpaceId => _spaceId;

        public string RoomId => _roomId;

        public bool IsEmpty => string.IsNullOrEmpty(_channelName);

        public static bool operator ==(ChannelId left, ChannelId right)
        {
            return EqualityComparer<ChannelId>.Default.Equals(left, right);
        }

        public static bool operator !=(ChannelId left, ChannelId right)
        {
            return !(left == right);
        }

        public bool IsValid()
        {
            if (IsEmpty || !IsValidName(_channelName))
            {
                return false;
            }

            return true;
        }

        public bool IsValidName(string name)
        {
            if (name.Length > 64)
            {
                return false;
            }

            foreach (char c in name.ToCharArray())
            {
                if (!ValidCharHashSet.Contains(c))
                {
                    return false;
                }
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ChannelId);
        }

        public bool Equals(ChannelId other)
        {
            return other != null && string.Equals(_channelName, other._channelName, StringComparison.Ordinal);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_channelName);
        }

        public override string ToString()
        {
            return !IsValid() ? string.Empty : _channelName;
        }
    }
}
