namespace TPFive.Game.Account
{
    using System;
    using Newtonsoft.Json;

    public interface IKeyStore
    {
        Credential GetCredentials();

        Guest GetGuest();

        void SetCredentials(Credential credentials);

        void SetGuest(Guest guest);

        string ReadInfo(string key);

        bool SaveInfo(string key, string value);

        bool DeleteInfo(string key);
    }
}