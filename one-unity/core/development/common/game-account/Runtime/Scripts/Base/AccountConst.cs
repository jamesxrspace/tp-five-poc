namespace TPFive.Game.Account
{
    public static class AccountConst
    {
        public const string AuthingTokenEndpoint = "oidc/token";
        public const string GuestCreateEndpoint = "authing/guest";
        public const string DeviceAuthEndpoint = "/authing/device/code";
        public const string DeviceAuthStatusEndpoint = "/authing/device/code/status";
        public const string DeviceAuthingIdTokenToAccessTokenEndpoint = "/oauth/oidc/token";
        public const string IdTokenToCredentialsGrantType = "http://authing.cn/oidc/grant_type/authing_token";
        public const string IdTokenToCredentialsRedirectUri = "https://xraccount.xrspace.net.cn/authing/authn";
    }
}