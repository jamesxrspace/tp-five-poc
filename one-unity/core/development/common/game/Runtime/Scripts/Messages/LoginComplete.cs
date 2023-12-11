namespace TPFive.Game.Messages
{
    public struct LoginSuccess
    {
        public AuthenticationMethod Method;

        public enum AuthenticationMethod
        {
            WebView,
            Password,
            DeviceCode,
        }
    }
}
