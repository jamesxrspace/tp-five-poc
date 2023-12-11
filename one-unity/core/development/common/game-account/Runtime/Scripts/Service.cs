namespace TPFive.Game.Account
{
    using System;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using TPFive.SCG.ServiceEco.Abstractions;
    using VContainer;
    using ILogger = Microsoft.Extensions.Logging.ILogger;
    using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;

    [RegisterToContainer]
    public class Service : IService
    {
        private static readonly string TagCredentials = "XrCredentials";
        private static readonly string TagGuestInfo = "GuestInfo";

        private readonly ILogger log;

        [Inject]
        private Service(
            ILoggerFactory loggerFactory,
            ApplicationConfiguration.IReadOnlyAppInfo appInfo)
        {
            log = loggerFactory.CreateLogger<Service>();

            log.LogDebug(
                "Domain: {Domain}, ClientId: {ClientId}, GameServerUrl: {GameServerUrl}",
                appInfo.AuthingDomain,
                appInfo.AuthingClientId,
                appInfo.GameServer.BaseUri);

            Auth = new XrAuth(
                loggerFactory,
                appInfo.AuthingDomain,
                appInfo.AuthingClientId,
                appInfo.GameServer.BaseUri);
            ReadLocalCredentials();
            ReadLocalGuestInfo();
        }

        private IXrAuth Auth { get; set; }

        private Credential CachedCredential { get; set; }

        private Guest CachedGuest { get; set; }

        private string AccessToken { get; set; }

        public void SetAccessToken(string token)
        {
            AccessToken = token;
        }

        public string GetAccessToken()
        {
            return GameApp.IsFlutter ? AccessToken : CachedCredential?.AccessToken;
        }

        public bool IsSignedIn()
        {
            log.LogDebug("{Method}: IsSignedIn() local credentials exists? {Exist}", nameof(IsSignedIn), CachedCredential != null);
            return IsLocalCredentialsValid();
        }

        public async void SignOutUser(IAuthCallback<string> listener)
        {
            var deleteResult = await Auth.DeleteDataAsync(TagCredentials);
            log.LogDebug("{Method}: delete credentials success? {DeleteResult}", nameof(SignOutUser), deleteResult.IsSuccess);
            Auth.SignOutUser(listener);
        }

        public void TryGetValidToken(IAuthCallback<string> listener)
        {
            ReadLocalCredentials();
            if (!IsSignedIn())
            {
                log.LogDebug("{Method}: Not sign in yet.", nameof(TryGetValidToken));
                listener?.OnFailure(AuthError.UserNotFound.Code, "Not sign in yet.");
                return;
            }

            if (CachedCredential?.ExpiredAt > DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            {
                log.LogDebug("{Method}: access token not expired.", nameof(TryGetValidToken));
                listener?.OnSuccess(CachedCredential?.AccessToken);
                return;
            }

            if (string.IsNullOrEmpty(CachedCredential?.RefreshToken))
            {
                log.LogWarning("{Method}: refresh token is null or empty", nameof(TryGetValidToken));
                listener?.OnFailure(AuthError.InvalidRefreshToken.Code, "Cannot renew access_token with empty refresh_token");
                return;
            }

            log.LogDebug("{Method}: Try to refresh expired access token.", nameof(TryGetValidToken));
            Auth.RenewAccessToken(CachedCredential?.RefreshToken, new AuthCallback<Credential>(
                credentials =>
                {
                    log.LogDebug("{Method}: renew access token success", nameof(TryGetValidToken));
                    ProcessRetrievingCredentials(credentials, listener);
                }, listener.GetFailureAction()));
        }

        public void SignInUserByUsername(string username, string password, IAuthCallback<string> listener)
        {
            log.LogDebug("{Method}: SignInUser()", nameof(SignInUserByUsername));
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                listener?.OnFailure(AuthError.InvalidParameter.Code, "ID/Password not valid");
            }
            else
            {
                Auth.SignInUser(username, password, new AuthCallback<Credential>(
                    credentials =>
                    {
                        log.LogDebug("{Method}: (id/pwd) sign in success", nameof(SignInUserByUsername));
                        ProcessRetrievingCredentials(credentials, listener);
                    }, listener.GetFailureAction()));
            }
        }

        public void SignInUserByWebview(IAuthCallback<string> listener)
        {
            log.LogDebug("{Method}: SignInUserByWebview()", nameof(SignInUserByWebview));
            Auth.SignInUser(new AuthCallback<Credential>(
                credentials =>
                {
                    string credentialString = JsonConvert.SerializeObject(credentials);
                    log.LogDebug("{Method}: (webview) sign in success", nameof(SignInUserByWebview));
                    ProcessRetrievingCredentials(credentials, listener);
                }, listener.GetFailureAction()));
        }

        public void RenewToken(IAuthCallback<string> listener)
        {
            if (CachedCredential == null || string.IsNullOrEmpty(CachedCredential.RefreshToken))
            {
                listener?.OnFailure(AuthError.InvalidRefreshToken.Code, "Refresh token is empty");
                return;
            }

            Auth.RenewAccessToken(CachedCredential.RefreshToken, new AuthCallback<Credential>(
                credentials => ProcessRetrievingCredentials(credentials, listener),
                listener.GetFailureAction()));
        }

        public void SignInDevice(Action<DeviceCodeFormat> action, IAuthCallback<string> stateListener)
        {
            var listener = new AuthCallback<DeviceCodeFormat>(
                format => action.Invoke(format),
                stateListener.GetFailureAction());

            Auth.ApiDeviceCodeSignIn(listener, new AuthCallback<Credential>(
                credentials => ProcessRetrievingCredentials(credentials, stateListener),
                stateListener.GetFailureAction()));
        }

        public void StopDeviceAuth()
        {
            log.LogDebug("{Method}: StopDeviceAuth(): Function not support", nameof(StopDeviceAuth));
        }

        public void CreateGuestAccount(string nickname, IAuthCallback<string> listener)
        {
            Auth.CreateGuestAccount(nickname, new AuthCallback<Guest>(
                async guest =>
                {
                    log.LogDebug("{Method}: Create guest success: {GuestString}", nameof(CreateGuestAccount), guest.Nickname);
                    var saveResult = await Auth.SaveDataAsync(TagGuestInfo, guest.ToJsonString());
                    log.LogDebug("{Method}: save guest info success? {SaveResult}", nameof(CreateGuestAccount), saveResult.IsSuccess);
                    if (saveResult.IsSuccess)
                    {
                        this.CachedGuest = guest;
                        listener?.OnSuccess(guest.Nickname);
                    }
                    else
                    {
                        listener?.OnFailure(AuthError.SaveDataFailed.Code, "Failed to save guest data");
                    }
                }, listener.GetFailureAction()));
        }

        /*
         * Check whether the cache guest's signin data exists
         */
        public bool IsGuestExist()
        {
            return !string.IsNullOrEmpty(CachedGuest?.UserId);
        }

        /*
         * Check sign in as a guest
         */
        public bool IsGuestLoggedIn()
        {
            if (string.IsNullOrEmpty(CachedCredential?.AccessToken))
            {
                log.LogWarning("{Method}: access_token is empty. No one is logged in.", nameof(IsGuestLoggedIn));
                return false;
            }

            var role = Credential.GetTokenPayload(CachedCredential.AccessToken);
            if (role.TryGetValue("role", out var value))
            {
                var roleStr = value.ToString();
                return string.Equals(roleStr, "guest", StringComparison.Ordinal);
            }

            return false;
        }

        public void SignInGuest(IAuthCallback<string> listener)
        {
            if (CachedGuest == null
                || string.IsNullOrEmpty(CachedGuest.UserId)
                || string.IsNullOrEmpty(CachedGuest.Email)
                || string.IsNullOrEmpty(CachedGuest.Password))
            {
                log.LogDebug("{Method}: Invalid cached guest. Sign in guest failed.", nameof(SignInGuest));
                listener?.OnFailure(AuthError.GuestNotExist.Code, "No guest data to sign in.");
                return;
            }

            Auth.SignInUser(CachedGuest.Email, CachedGuest.Password, new AuthCallback<Credential>(
                credentials =>
                {
                    log.LogDebug("{Method}: sign in guest success", nameof(SignInGuest));
                    ProcessRetrievingCredentials(credentials, listener);
                }, listener.GetFailureAction()));
        }

        private async void ProcessRetrievingCredentials(Credential credentials, IAuthCallback<string> callback)
        {
            string credentialString = JsonConvert.SerializeObject(credentials);
            var saveResult = await Auth.SaveDataAsync(TagCredentials, credentialString);
            log.LogDebug("{Method}: save credentials success? {SaveResult}", nameof(TryGetValidToken), saveResult.IsSuccess);
            if (!saveResult.IsSuccess)
            {
                callback?.OnFailure(AuthError.SaveDataFailed.Code, "Failed to save credentials");
                return;
            }

            this.CachedCredential = credentials;
            callback?.OnSuccess(credentials?.AccessToken);
        }

        private async void ReadLocalCredentials()
        {
            var readResult = await Auth.ReadDataAsync(TagCredentials);
            if (!readResult.IsSuccess)
            {
                log.LogDebug("{Method}: local credentials not exists.", nameof(ReadLocalCredentials));
                this.CachedCredential = null;
                return;
            }

            log.LogDebug("{Method}: Read local credentials", nameof(ReadLocalCredentials));
            this.CachedCredential = Credential.Parse(readResult.Payload);
        }

        private async void ReadLocalGuestInfo()
        {
            var readResult = await Auth.ReadDataAsync(TagGuestInfo);
            if (!readResult.IsSuccess)
            {
                log.LogDebug("{Method}: local guest info not exists.", nameof(ReadLocalGuestInfo));
                this.CachedGuest = null;
                return;
            }

            log.LogDebug("{Method}: Read local guest info", nameof(ReadLocalGuestInfo));
            this.CachedGuest = Guest.Parse(readResult.Payload);
        }

        private bool IsLocalCredentialsValid()
        {
            // If access token is null, return not valid.
            if (string.IsNullOrEmpty(this.CachedCredential?.AccessToken) || this.CachedCredential?.ExpiredAt == 0)
            {
                log.LogDebug("{Method}: No access_token. Local credentials is not valid.", nameof(IsLocalCredentialsValid));
                return false;
            }

            // If access token not expired, return valid.
            if (this.CachedCredential.ExpiredAt > DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            {
                log.LogDebug(
                    "{Method}: expired_at: {ExpiredAt} ({LocalDateTime})",
                    nameof(IsLocalCredentialsValid),
                    CachedCredential.ExpiredAt,
                    DateTimeOffset.FromUnixTimeSeconds(CachedCredential.ExpiredAt).LocalDateTime);
                log.LogDebug("{Method}: The access token is still valid", nameof(IsLocalCredentialsValid));
                return true;
            }

            // FIXME 要檢查refresh_token的效期
            if (!string.IsNullOrEmpty(this.CachedCredential.RefreshToken))
            {
                log.LogDebug("{Method}: Although access token is expired but can be refresh.(valid)", nameof(IsLocalCredentialsValid));
                return true;
            }

            log.LogDebug("{Method}: Credential is not valid.", nameof(IsLocalCredentialsValid));
            return false;
        }
    }
}