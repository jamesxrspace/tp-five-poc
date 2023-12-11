namespace TPFive.Game.Account
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Newtonsoft.Json;
    using UnityEngine;

    public class FileKeyStore : IKeyStore
    {
        private static readonly string TAG = "[XRAccount][FileKeyStore]";
        private static readonly string LocalDataPath = $"{Application.persistentDataPath}/XRData.json";
        private readonly Preferences preferences = new Preferences();

        public Credential GetCredentials()
        {
            return preferences?.Credential;
        }

        public Guest GetGuest()
        {
            return preferences?.Guest;
        }

        public void SetCredentials(Credential credentials)
        {
            preferences.Credential = credentials;
        }

        public void SetGuest(Guest guest)
        {
            preferences.Guest = guest;
        }

        public string ReadInfo(string key)
        {
            Dictionary<string, string> info;

            if (!File.Exists(LocalDataPath))
            {
                Debug.LogWarning($"{TAG} ReadInfo: file[{LocalDataPath}] not found");
                return null;
            }

            string json = File.ReadAllText(LocalDataPath);
            info = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            if (!info.ContainsKey(key))
            {
                Debug.LogWarning($"{TAG} ReadInfo: key[{key}] not exists");
                return null;
            }

            return info[key];
        }

        public bool SaveInfo(string key, string value)
        {
            Dictionary<string, string> info;

            if (File.Exists(LocalDataPath))
            {
                string json = File.ReadAllText(LocalDataPath);
                info = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }
            else
            {
                info = new Dictionary<string, string>();
            }

            info[key] = value;
            string updateInfo = JsonConvert.SerializeObject(info, Formatting.None);

            try
            {
                File.WriteAllText(LocalDataPath, updateInfo);
                Debug.Log($"{TAG} SaveInfo: save data done");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"{TAG} SaveInfo: Exception - {e}");
                return false;
            }
        }

        public bool DeleteInfo(string key)
        {
            Dictionary<string, string> info;
            if (!File.Exists(LocalDataPath))
            {
                Debug.Log($"{TAG} DeleteInfo: file[{LocalDataPath}] not exist. No need to delete.");
                return true;
            }

            string json = File.ReadAllText(LocalDataPath);
            info = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            if (!info.ContainsKey(key))
            {
                Debug.Log($"{TAG} There's no value of {key}. No need to delete.");
                return true;
            }

            bool result = info.Remove(key);
            if (!result)
            {
                Debug.LogError($"{TAG} Delete info {key} failed.");
                return false;
            }

            try
            {
                string updateInfo = JsonConvert.SerializeObject(info, Formatting.None);
                File.WriteAllText(LocalDataPath, updateInfo);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"{TAG} DeleteInfo: Exception - {e}");
                return false;
            }
        }
    }
}