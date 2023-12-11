namespace TPFive.Game.Login
{
    public sealed class LogoutResult
    {
        public string Error { get; set; }

        public bool Ok => string.IsNullOrEmpty(Error);
    }
}
