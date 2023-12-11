using System;
using System.Linq;
using UnityEngine;

namespace TPFive.Game.Login.Entry.Models
{
    [CreateAssetMenu(fileName = "AccountPresets", menuName = "TPFive/Login/AccountPresets")]
    public class AccountPresets : ScriptableObject
    {
        [SerializeField]
        private AccountPreset[] accounts;

        public Account[] GetAccounts()
        {
            if (accounts == null || accounts.Length == 0)
            {
                return Array.Empty<Account>();
            }

            return accounts.Select(preset => preset.ToAccount())
                           .ToArray();
        }
    }
}
