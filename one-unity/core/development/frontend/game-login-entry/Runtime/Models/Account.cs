using System;
using TPFive.Game.Login.Entry.Extensions;

namespace TPFive.Game.Login.Entry.Models
{
    [Serializable]
    public struct Account : IEquatable<Account>
    {
        public string Username;
        public string Password;
        public long LastLogonTimestamp;

        public static bool operator ==(Account left, Account right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Account left, Account right)
        {
            return !(left == right);
        }

        public static bool Validate(string username) => !string.IsNullOrEmpty(username);

        public readonly bool IsValid() => Validate(Username);

        public readonly string GetDisplayName()
        {
            if (string.IsNullOrEmpty(Username))
            {
                return string.Empty;
            }

            var items = Username.Split('@', 2, StringSplitOptions.None);
            return items.Length > 0 ? items[0].ToTitleCase() : string.Empty;
        }

        public void UpdateLastLogonTimestamp(long? timestamp = null)
        {
            LastLogonTimestamp = timestamp ?? DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        public override readonly bool Equals(object obj)
        {
            return obj is Account account && Equals(account);
        }

        public readonly bool Equals(Account other)
        {
            return Username == other.Username;
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(Username);
        }
    }
}
