namespace TPFive.Game.Account
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Cysharp.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using UnityEngine;
    using ILogger = Microsoft.Extensions.Logging.ILogger;

    public class XrAuth : IXrAuth
    {
        private static readonly string PackageId = Application.identifier;
        private readonly HttpClientApi authClient;
        private readonly IPortingLayer authLayer = null;
        private readonly ILoggerFactory loggerFactory;
        private readonly string clientId;
        private readonly string domain;
        private readonly string audDomain;
        private DeviceCodeFormat deviceCodeFormat = null;
        private bool stopStatePolling = false;
        private string redirectUri;
        private ILogger logger;

        public XrAuth(
            ILoggerFactory loggerFactory,
            string domain,
            string clientId,
            string audDomain)
        {
            this.loggerFactory = loggerFactory;
            this.domain = domain;
            this.clientId = clientId;
            this.audDomain = audDomain;

            authLayer = CreateAuthLayer();

            authClient = new HttpClientApi(domain, clientId, audDomain);
        }

        private enum DeviceAuthStatus
        {
            Expired = -1,
            Unauth = 0,
            Scanned = 1,
            Confirm = 2,
        }

        private ILogger Logger => logger ??= loggerFactory.CreateLogger<XrAuth>();

        /*
         * return valid access token
         */
        public async void RenewAccessToken(string refreshToken, IAuthCallback<Credential> listener)
        {
            Logger.LogInformation("Try to renew access token");
            var response = await authClient.HttpRenewAuth(refreshToken);
            string responseBody = await response.Content.ReadAsStringAsync();
            Credential credentials;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (string.IsNullOrEmpty(responseBody))
                {
                    Logger.LogError("responseBody is null or empty");
                    listener?.OnFailure(AuthError.EmptyResponse.Code, AuthError.EmptyResponse.Message);
                    return;
                }

                try
                {
                    credentials = Credential.Parse(responseBody);
                    if (credentials == null || !Credential.IsValid(credentials))
                    {
                        Logger.LogWarning("credentials is invalid");
                        listener?.OnFailure(AuthError.RenewAccessTokenFailed.Code, "Refresh token is invalid");
                        return;
                    }

                    credentials.ExpiredAt = Credential.GetTokenExpirationTime(credentials);
                    var expiredTime = DateTimeOffset.FromUnixTimeSeconds(credentials.ExpiredAt).LocalDateTime;
                    Logger.LogInformation($"access_token expired_at: {credentials.ExpiredAt} ({expiredTime})");
                }
                catch (Exception e)
                {
                    Logger.LogError($"Parsing credential exception: {e}");
                    return;
                }

                listener?.OnSuccess(credentials);
            }
            else
            {
                Logger.LogWarning($"refesh access token failed: status_code: ({(int)response.StatusCode}), resp: {responseBody}");
                listener?.OnFailure(AuthError.RenewAccessTokenFailed.Code, $"Failed to renew token: {response.StatusCode} {responseBody}");
            }
        }

        /*
         * Open webview to sign in
         */
        public void SignInUser(IAuthCallback<Credential> listener)
        {
            Logger.LogInformation($"SignInUser by webview");

            authLayer.Login(clientId, domain, redirectUri, new AuthCallback<string>(
                result => ProcessSuccessSignInCallback(result, listener),
                listener.GetFailureAction()));
        }

        /*
         * Sign in through id/pwd
         */
        public async void SignInUser(string username, string password, IAuthCallback<Credential> listener)
        {
            Logger.LogInformation($"SignInUser by id/pwd with {username}");

            var response = await authClient.HttpSignIn(username, password);
            string responseBody = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                ProcessSuccessSignInCallback(responseBody, listener);
            }
            else
            {
                Logger.LogWarning($"Sign in fail. code:{(int)response.StatusCode} , msg:{responseBody}");
                listener?.OnFailure(AuthError.RequestFailed.Code, $"Sign in failed: {response.StatusCode} {responseBody}");
            }
        }

        /*
         * Open webview to sign out
         */
        public void SignOutUser(IAuthCallback<string> listener)
        {
            authLayer.Logout(clientId, domain, redirectUri, listener);
        }

        public async void ApiStartDeviceAuth(IAuthCallback<DeviceCodeFormat> listener)
        {
            Logger.LogInformation("Start device auth.");
            deviceCodeFormat = null;
            var response = await authClient.StartDeviceAuth();
            var responseString = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                deviceCodeFormat = JsonConvert.DeserializeObject<DeviceCodeFormat>(responseString);
                listener?.OnSuccess(deviceCodeFormat);
            }
            else
            {
                var errorBody = JsonConvert.DeserializeObject<XrAccountErrorBody>(responseString);
                Logger.LogError($"Generate device code failed. {errorBody.Error.Code} {errorBody.Error.Message}");
                listener?.OnFailure(AuthError.CannotGetDeviceCode.Code, $"Failed to generate device code: {errorBody.Error.Code} {errorBody.Error.Message}");
            }
        }

        public void ApiDeviceCodeSignIn(
            IAuthCallback<DeviceCodeFormat> verifyUriListener,
            IAuthCallback<Credential> authStateListener)
        {
            ApiStartDeviceAuth(new AuthCallback<DeviceCodeFormat>(
                data =>
                {
                    Logger.LogInformation($"Get device code success: [{deviceCodeFormat?.VerificationUriComplete}]. Start polling state...");
                    verifyUriListener?.OnSuccess(data);
                    GetCurrentDeviceAuthState(deviceCodeFormat?.UserCode, authStateListener);
                }, verifyUriListener?.GetFailureAction()));
        }

        public async void GetCurrentDeviceAuthState(string userCode, IAuthCallback<Credential> listener)
        {
            Logger.LogInformation($"GetCurrentDeviceAuthState({userCode})");
            stopStatePolling = false;
            while (!stopStatePolling)
            {
                var response = await authClient.PollingDeviceCodeStatus(userCode);
                var responseString = await response.Content.ReadAsStringAsync();
                Logger.LogInformation($"status code({(int)response.StatusCode}), result({responseString})");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var statusResp = JsonConvert.DeserializeObject<XRAccountStatusResponse>(responseString);

                    switch (statusResp.Status)
                    {
                        case (int)DeviceAuthStatus.Unauth:
                        case (int)DeviceAuthStatus.Scanned:
                            Logger.LogInformation("Waiting for user enter activation code.");
                            break;
                        case (int)DeviceAuthStatus.Confirm:
                            Logger.LogInformation($"Get authing token success, try to get credential. authing token: {statusResp.AuthingToken}");
                            stopStatePolling = true;
                            var resp = await authClient.AuthingTokenToCredential(statusResp.AuthingToken);
                            var respString = await resp.Content.ReadAsStringAsync();
                            if (resp.StatusCode == HttpStatusCode.OK)
                            {
                                Logger.LogInformation("Get credential success.");

                                ProcessSuccessSignInCallback(responseString, listener);
                            }
                            else
                            {
                                ErrorResponse errResp = JsonConvert.DeserializeObject<ErrorResponse>(respString);
                                Logger.LogError($"authing token to credential failed. error: {errResp.Error}, err_msg: {errResp.ErrorDescription}");
                                listener?.OnFailure(AuthError.AuthTokenFailed.Code, $"Sign in device failed: {errResp.Error} {errResp.ErrorDescription}");
                            }

                            break;
                        case (int)DeviceAuthStatus.Expired:
                        default:
                            Logger.LogWarning($"Device code has expired : {responseString}");
                            listener?.OnFailure(AuthError.DeviceCodeExpired.Code, responseString);
                            stopStatePolling = true;
                            break;
                    }
                }
                else
                {
                    var errResp = JsonConvert.DeserializeObject<XrAccountErrorBody>(responseString);
                    Logger.LogError($"User code is not exist. status: {errResp.Status}, err_code: {errResp.Error.Code}, err_msg: {errResp.Error.Message}");
                    listener?.OnFailure(AuthError.AuthTokenFailed.Code, $"Sign in device failed: {errResp.Error.Code} {errResp.Error.Message}");
                    stopStatePolling = true;
                }

                Logger.LogInformation($"Wait for {deviceCodeFormat.Interval} seconds.");
                await Task.Delay(deviceCodeFormat.Interval * 1000);
            }
        }

        /*
         * POST to AccountServer and get created guest account in response
         */
        public async void CreateGuestAccount(string nickname, IAuthCallback<Guest> listener)
        {
            Logger.LogInformation("Create guest account");

            var response = await authClient.CreateGuestAccount(nickname);
            string msg = null;
            if (response.Content != null)
            {
                msg = await response.Content.ReadAsStringAsync();
            }

            if (response.IsSuccessStatusCode)
            {
                var guestResp = JsonConvert.DeserializeObject<CreateGuestResponse>(msg);
                Guest guest = guestResp?.GuestInfo;

                if (string.IsNullOrEmpty(guest?.UserId))
                {
                    listener?.OnFailure(AuthError.CreateGuestFailed.Code, AuthError.CreateGuestFailed.Message);
                    return;
                }

                Logger.LogInformation($"Create guest account success.");
                listener.OnSuccess(guest);
            }
            else
            {
                Logger.LogWarning($"Create guest account failed: status_code:{(int)response.StatusCode} , msg:{msg}");
                listener?.OnFailure(AuthError.CreateGuestFailed.Code, $"Failed to create guest: {response.StatusCode} {msg}");
            }
        }

        public UniTask<AuthResult<string>> ReadDataAsync(string key)
        {
            var tcs = new UniTaskCompletionSource<AuthResult<string>>();
            authLayer.ReadInfo(key, new AuthCallback<string>(
                data => tcs.TrySetResult(AuthResult<string>.Success(data)),
                (code, message) => tcs.TrySetResult(AuthResult<string>.Failure(code, message))));
            return tcs.Task;
        }

        public UniTask<AuthResult<bool>> SaveDataAsync(string key, string value)
        {
            var tcs = new UniTaskCompletionSource<AuthResult<bool>>();
            authLayer.SaveInfo(key, value, new AuthCallback<bool>(
                result => tcs.TrySetResult(AuthResult<bool>.Success(result)),
                (code, message) => tcs.TrySetResult(AuthResult<bool>.Failure(code, message))));
            return tcs.Task;
        }

        public UniTask<AuthResult<bool>> DeleteDataAsync(string key)
        {
            var tcs = new UniTaskCompletionSource<AuthResult<bool>>();
            authLayer.DeleteInfo(key, new AuthCallback<bool>(
                result => tcs.TrySetResult(AuthResult<bool>.Success(result)),
                (code, message) => tcs.TrySetResult(AuthResult<bool>.Failure(code, message))));
            return tcs.Task;
        }

        private void ProcessSuccessSignInCallback(string response, IAuthCallback<Credential> callback)
        {
            Credential credentials;
            if (string.IsNullOrEmpty(response))
            {
                Logger.LogWarning("Login response data is empty");
                callback?.OnFailure(AuthError.EmptyResponse.Code, "Sign in success but received EMPTY response of credentials.");
                return;
            }

            try
            {
                credentials = Credential.Parse(response);
                if (credentials == null || !Credential.IsValid(credentials))
                {
                    Logger.LogWarning("credentials is invalid");
                    callback?.OnFailure(AuthError.MalformedResponse.Code, "Sign in success but received malformed response of credentials.");
                    return;
                }

                credentials.ExpiredAt = Credential.GetTokenExpirationTime(credentials);
                var expiredTime = DateTimeOffset.FromUnixTimeSeconds(credentials.ExpiredAt).LocalDateTime;
                Logger.LogInformation($"access_token expired_at: {credentials.ExpiredAt} ({expiredTime})");
            }
            catch (Exception e)
            {
                Logger.LogError($"Parsing credential exception: {e}");
                callback?.OnFailure(AuthError.InvalidCredentials.Code, $"Filed to parse credentials: {e}");
                return;
            }

            callback?.OnSuccess(credentials);
        }

        private IPortingLayer CreateAuthLayer()
        {
            if (Application.isEditor)
            {
                Logger.LogInformation("Go Editor");
                return new EditorPortingLayer(loggerFactory);
            }
            else
            {
                switch (Application.platform)
                {
                    case RuntimePlatform.Android:
                        Logger.LogInformation("Go Android");
                        redirectUri = $"{audDomain}/authing/authn/android/{PackageId}";
                        Logger.LogInformation($"redirectUri: {redirectUri}");
                        return new AndroidPortingLayer(loggerFactory);
                    case RuntimePlatform.IPhonePlayer:
                        Logger.LogInformation("Go iOS");
                        redirectUri = $"{audDomain}/authing/authn/ios/{PackageId}";
                        Logger.LogInformation($"redirectUri: {redirectUri}");
                        return new IosPortingLayer(loggerFactory);
                    case RuntimePlatform.WindowsPlayer:
                    case RuntimePlatform.OSXPlayer:
                    case RuntimePlatform.LinuxPlayer:
                        Logger.LogInformation("Go Editor for standalone");
                        return new EditorPortingLayer(loggerFactory);
                    default:
                        Logger.LogError($"Only supports the Android or iOS platforms, not support [{Application.platform}].");
                        throw new Exception("Only supports the Android or iOS platforms. Please check the current running platform.");
                }
            }
        }
    }
}