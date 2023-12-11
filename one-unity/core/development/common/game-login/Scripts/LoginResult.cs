namespace TPFive.Game.Login
{
    public class LoginResult
    {
        public string Error { get; set; }

        public bool Ok => string.IsNullOrEmpty(Error);
    }
}
