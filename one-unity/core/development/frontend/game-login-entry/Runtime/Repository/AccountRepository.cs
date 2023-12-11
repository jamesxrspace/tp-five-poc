using System;
using System.Collections.Generic;
using System.Linq;
using Loxodon.Framework.Prefs;
using TPFive.Game.Login.Entry.Models;
using UnityEngine.Assertions;
using VContainer;

namespace TPFive.Game.Login.Entry.Repository
{
    public class AccountRepository
    {
        private static readonly string PrefsKey = nameof(AccountRepository);

        private readonly Account[] presets;
        private readonly Preferences preferences;
        private readonly Dictionary<string, Account> accountDict;

        [Inject]
        public AccountRepository(Account[] presets, Preferences preferences)
        {
            this.presets = presets;
            this.preferences = preferences;
            this.accountDict = new Dictionary<string, Account>();

            Load();
        }

        public bool Add(Account account)
        {
            Assert.IsTrue(account.IsValid());

            return Add(account, true);
        }

        public void AddMany(IEnumerable<Account> accounts)
        {
            Assert.IsNotNull(accounts);

            AddMany(accounts, true);
        }

        public Account Get(string username)
        {
            if (Account.Validate(username) &&
                accountDict.TryGetValue(username, out var account))
            {
                return account;
            }

            return default;
        }

        public Account[] GetAll()
        {
            return accountDict.Values.ToArray();
        }

        public void Update(Account account)
        {
            Assert.IsTrue(account.IsValid());

            Update(account, true);
        }

        public void UpdateMany(IEnumerable<Account> accounts)
        {
            Assert.IsNotNull(accounts);

            UpdateMany(accounts, true);
        }

        public void UpdateOrAdd(Account account)
        {
            Assert.IsTrue(account.IsValid());

            UpdateOrAdd(account, true);
        }

        public void UpdateOrAddMany(IEnumerable<Account> accounts)
        {
            Assert.IsNotNull(accounts);

            UpdateOrAddMany(accounts, true);
        }

        public bool Remove(string username)
        {
            Assert.IsTrue(Account.Validate(username));

            return Remove(username, true);
        }

        public void RemoveMany(IEnumerable<string> usernames)
        {
            Assert.IsNotNull(usernames);

            RemoveMany(usernames, true);
        }

        public void RemoveAll()
        {
            accountDict.Clear();
            Save();
        }

        public Account GetLastLogonAccount()
        {
            return accountDict.Values.Where(x => x.LastLogonTimestamp <= DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                              .OrderByDescending(x => x.LastLogonTimestamp)
                              .FirstOrDefault();
        }

        private bool Add(Account account, bool save)
        {
            if (accountDict.TryAdd(account.Username, account))
            {
                if (save)
                {
                    Save();
                }

                return true;
            }

            return false;
        }

        private void AddMany(IEnumerable<Account> accounts, bool save)
        {
            bool dirty = false;
            foreach (var account in accounts)
            {
                if (account.IsValid())
                {
                    dirty |= accountDict.TryAdd(account.Username, account);
                }
            }

            if (dirty && save)
            {
                Save();
            }
        }

        private void Update(Account account, bool save)
        {
            if (account.IsValid() &&
                accountDict.TryGetValue(account.Username, out var value))
            {
                value.Password = account.Password;
                value.LastLogonTimestamp = account.LastLogonTimestamp;

                if (save)
                {
                    Save();
                }
            }
        }

        private void UpdateMany(IEnumerable<Account> accounts, bool save)
        {
            bool dirty = false;
            foreach (var account in accounts)
            {
                if (account.IsValid() &&
                    accountDict.TryGetValue(account.Username, out var value))
                {
                    value.Password = account.Password;
                    value.LastLogonTimestamp = account.LastLogonTimestamp;

                    dirty = true;
                }
            }

            if (dirty && save)
            {
                Save();
            }
        }

        private void UpdateOrAdd(Account account, bool save)
        {
            if (account.IsValid())
            {
                accountDict[account.Username] = account;

                if (save)
                {
                    Save();
                }
            }
        }

        private void UpdateOrAddMany(IEnumerable<Account> accounts, bool save)
        {
            bool dirty = false;
            foreach (var account in accounts)
            {
                if (account.IsValid())
                {
                    accountDict[account.Username] = account;
                    dirty = true;
                }
            }

            if (dirty && save)
            {
                Save();
            }
        }

        private bool Remove(string username, bool save)
        {
            if (accountDict.Remove(username))
            {
                if (save)
                {
                    Save();
                }

                return true;
            }

            return false;
        }

        private void RemoveMany(IEnumerable<string> usernames, bool save)
        {
            bool dirty = false;
            foreach (var username in usernames)
            {
                if (Account.Validate(username) &&
                    accountDict.Remove(username))
                {
                    dirty = true;
                }
            }

            if (dirty && save)
            {
                Save();
            }
        }

        private void Save()
        {
            preferences.SetObject(PrefsKey, new Json.ArrayWarpper<Account>
            {
                Items = accountDict.Values.ToArray(),
            });
            preferences.Save();
        }

        private void Load()
        {
            var warpper = preferences.GetObject<Json.ArrayWarpper<Account>>(PrefsKey, default);

            if (warpper.Items != null && warpper.Items.Length > 0)
            {
                AddMany(warpper.Items, false);
            }

            if (presets != null && presets.Length > 0)
            {
                AddMany(presets, false);
            }
        }
    }
}
