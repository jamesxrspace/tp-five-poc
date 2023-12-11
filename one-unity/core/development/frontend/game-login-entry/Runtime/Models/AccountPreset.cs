namespace TPFive.Game.Login.Entry
{
    [System.Serializable]
    internal struct AccountPreset
    {
        public string Username;
        public string Password;

        public Models.Account ToAccount()
        {
            return new Models.Account
            {
                Username = Username,
                Password = Password,
            };
        }
    }
}
