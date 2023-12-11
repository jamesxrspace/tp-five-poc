namespace TPFive.Game.Account
{
    public class AuthError
    {
        // Also means success
        public static readonly AuthError OK = new AuthError(0, "OK");

        // System error
        public static readonly AuthError FunctionNotSupported = new AuthError(2001, "Function not supported");
        public static readonly AuthError FunctionNotImplemented = new AuthError(2002, "Function not implemented");

        // Data cache error
        public static readonly AuthError SaveDataFailed = new AuthError(2011, "Save data failed");
        public static readonly AuthError ReadDataFailed = new AuthError(2012, "Read data failed");
        public static readonly AuthError DeleteDataFailed = new AuthError(2013, "Delete data failed");

        // State error
        public static readonly AuthError NotSignedIn = new AuthError(2021, "Not signed in");
        public static readonly AuthError SignInAlready = new AuthError(2022, "Already signed in");

        // Auth error
        public static readonly AuthError InvalidParameter = new AuthError(2101, "Invalid parameter");
        public static readonly AuthError InvalidCredentials = new AuthError(2102, "Invalid credentials");
        public static readonly AuthError InvalidAccessToken = new AuthError(2103, "Invalid access token");
        public static readonly AuthError InvalidRefreshToken = new AuthError(2104, "Invalid refresh token");
        public static readonly AuthError AccessTokenExpired = new AuthError(2105, "Access token expired");
        public static readonly AuthError RefreshTokenExpired = new AuthError(2106, "Refresh token expired");
        public static readonly AuthError UserCancelLogin = new AuthError(2111, "User cancel login");
        public static readonly AuthError AuthCodeFailed = new AuthError(2112, "Failed to get authorization code during login");
        public static readonly AuthError AuthTokenFailed = new AuthError(2113, "Failed to exchange access_token with authorization code");

        // User instance error
        public static readonly AuthError UsernameAlreadyExists = new AuthError(2201, "Username already exists");
        public static readonly AuthError EmailAlreadyExists = new AuthError(2202, "Email already exists");
        public static readonly AuthError UsernameMalformed = new AuthError(2203, "Username malformed");
        public static readonly AuthError EmailMalformed = new AuthError(2204, "Email malformed");
        public static readonly AuthError UserNotFound = new AuthError(2205, "User not found");
        public static readonly AuthError RenewAccessTokenFailed = new AuthError(2206, "Renew access token failed");

        // Http error
        public static readonly AuthError Network = new AuthError(2301, "Network error");
        public static readonly AuthError RequestTimeout = new AuthError(2302, "Request timeout");
        public static readonly AuthError RequestFailed = new AuthError(2303, "Request failed");
        public static readonly AuthError EmptyResponse = new AuthError(2304, "Empty response");
        public static readonly AuthError InvalidResponse = new AuthError(2305, "Invalid response");
        public static readonly AuthError MalformedResponse = new AuthError(2306, "Malformed response");

        // Device auth error
        public static readonly AuthError AuthorizationPending = new AuthError(2501, "Authorization pending");
        public static readonly AuthError Slowdown = new AuthError(2502, "Slowdown");
        public static readonly AuthError CannotGetDeviceCode = new AuthError(2503, "Cannot get device code");
        public static readonly AuthError DeviceCodeExpired = new AuthError(2504, "Device code expired");

        // Guest auth error
        public static readonly AuthError GuestNotExist = new AuthError(2601, "Guest not exist");
        public static readonly AuthError CreateGuestFailed = new AuthError(2602, "Create guest failed");

        public static readonly AuthError Unknown = new AuthError(2999, "Unknown error");

        public AuthError(int code, string message)
        {
            Code = code;
            Message = message;
        }

        public int Code { get; }

        public string Message { get; private set; }
    }
}