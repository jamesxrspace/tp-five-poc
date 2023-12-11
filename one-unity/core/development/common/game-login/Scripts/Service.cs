namespace TPFive.Game.Login
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Cysharp.Threading.Tasks;
    using MessagePipe;
    using Microsoft.Extensions.Logging;
    using TPFive.Game.Account;
    using TPFive.Game.Logging;
    using TPFive.Game.Messages;
    using TPFive.OpenApi.GameServer;
    using TPFive.SCG.AsyncStartable.Abstractions;
    using TPFive.SCG.DisposePattern.Abstractions;
    using TPFive.SCG.ServiceEco.Abstractions;
    using UnityEngine.Assertions;
    using VContainer;
    using VContainer.Unity;

    using IAccountService = TPFive.Game.Account.IService;

    [AsyncStartable]
    [ServiceProviderManagement]
    [ServiceProvidedBy(typeof(IServiceProvider))]
    [RegisterToContainer]
    public sealed partial class Service : IService
    {
        private readonly IAccountService accountService;
        private readonly Options options;
        private readonly IPublisher<LoginSuccess> loginPublisher;
        private readonly IPublisher<LogoutComplete> logoutPublisher;
        private readonly ILoginApi loginApi;

        [Inject]
        public Service(
            ILoggerFactory loggerFactory,
            IAccountService accountService,
            Options options,
            IPublisher<LoginSuccess> loginPublisher,
            IPublisher<LogoutComplete> logoutPublisher,
            ILoginApi loginApi)
        {
            Logger = Utility.CreateLogger<Service>(loggerFactory);
            this.accountService = accountService;
            this.options = options;
            this.loginPublisher = loginPublisher;
            this.logoutPublisher = logoutPublisher;
            this.loginApi = loginApi;

            var nullServiceProvider = new NullServiceProvider((s, args) =>
            {
                Logger.LogDebug(s, args);
            });

            _serviceProviderTable.Add(0, nullServiceProvider);
        }

        public IServiceProvider NullServiceProvider => _serviceProviderTable[0] as IServiceProvider;

        private string AppId => options.AppId;

        private ILogger Logger { get; set; }

        public UniTask<LoginResult> LoginByWebView()
        {
            return Login(
                callback =>
                {
                    accountService.SignInUserByWebview(callback);
                }, LoginSuccess.AuthenticationMethod.WebView);
        }

        public UniTask<LoginResult> LoginByPassword(string username, string password)
        {
            return Login(
                callback =>
                {
                    accountService.SignInUserByUsername(username, password, callback);
                }, LoginSuccess.AuthenticationMethod.Password);
        }

        public UniTask<LoginResult> LoginByDeviceCode(IProgress<DeviceCodeInfo> progress)
        {
            void Callback(DeviceCodeFormat format)
            {
                var deviceCode = new DeviceCodeInfo
                {
                    UserCode = format.UserCode,
                    Url = format.VerificationUri,
                    CompleteUrl = format.VerificationUriComplete,
                    ExpiresIn = format.ExpiresIn,
                };
                progress?.Report(deviceCode);
            }

            return Login(
                loginCallback =>
                {
                    accountService.SignInDevice(Callback, loginCallback);
                }, LoginSuccess.AuthenticationMethod.DeviceCode);
        }

        public UniTask<LogoutResult> Logout()
        {
            var promise = new UniTaskCompletionSource<LogoutResult>();
            var logoutCallback = new AuthCallback<string>(
                success =>
                {
                    promise.TrySetResult(new LogoutResult());
                    logoutPublisher.Publish(default);
                },
                (errorCode, errorMessage) =>
                {
                    promise.TrySetResult(new LogoutResult()
                    {
                        Error = ErrorString(errorCode, errorMessage),
                    });
                });

            accountService.SignOutUser(logoutCallback);
            return promise.Task;
        }

        public string GetAccessToken()
        {
            return accountService.GetAccessToken();
        }

        public UniTask<bool> RefreshAccessToken()
        {
            var promise = new UniTaskCompletionSource<bool>();
            accountService.TryGetValidToken(new AuthCallback<string>(
                result =>
                {
                    promise.TrySetResult(true);
                }, (code, msg) =>
                {
                    promise.TrySetResult(false);
                }));

            return promise.Task;
        }

        public void SetAccessToken(string token)
        {
            accountService.SetAccessToken(token);
        }

        private string ErrorString(int errorCode, string message)
        {
            return string.Format("({0}): {1}", errorCode.ToString(), message);
        }

        private async UniTask SetupBegin(CancellationToken cancellationToken = default)
        {
            await UniTask.CompletedTask;
        }

        private async UniTask SetupEnd(bool success, CancellationToken cancellationToken = default)
        {
            if (Logger.IsDebugEnabled())
            {
                Logger.LogEditorDebug("{Method}", nameof(SetupEnd));
            }

            await UniTask.CompletedTask;
        }

        private async Task HandleStartAsyncOperationCanceledException(
            OperationCanceledException e,
            CancellationToken cancellationToken = default)
        {
            if (Logger.IsWarningEnabled())
            {
                Logger.LogWarning("{Exception}", e);
            }

            await Task.CompletedTask;
        }

        private async Task HandleStartAsyncException(
            Exception e,
            CancellationToken cancellationToken = default)
        {
            if (Logger.IsErrorEnabled())
            {
                Logger.LogError("{Exception}", e);
            }

            await Task.CompletedTask;
        }

        private UniTask<LoginResult> LoginAuthServer(Action<AuthCallback<string>> authLoginWithCallback)
        {
            var promise = new UniTaskCompletionSource<LoginResult>();
            var callback = new AuthCallback<string>(
                _ => promise.TrySetResult(new LoginResult()),
                (errorCode, errorMessage) => promise.TrySetResult(
                    new LoginResult()
                    {
                        Error = ErrorString(errorCode, errorMessage),
                    }));

            authLoginWithCallback(callback);

            return promise.Task;
        }

        private async UniTask<LoginResult> LoginGameServer()
        {
            var response = await loginApi.PostLoginAsync();

            if (!response.IsSuccess)
            {
                if (Logger.IsErrorEnabled())
                {
                    Logger.LogError(
                        "{Method}(): Failed. http_code: {httpCode} , err_code: {errCode} , err_msg: {msg}",
                        nameof(LoginGameServer),
                        response.HttpStatusCode,
                        response.ErrorCode,
                        response.Message);
                }

                return new LoginResult()
                {
                    Error = ErrorString(response.ErrorCode, response.Message),
                };
            }

            if (Logger.IsDebugEnabled())
            {
                Logger.LogDebug("{Method}(): Success.", nameof(LoginGameServer));
            }

            return new LoginResult();
        }

        private async UniTask<LoginResult> Login(Action<AuthCallback<string>> authLoginWithCallback, LoginSuccess.AuthenticationMethod authLoginMethodName)
        {
            var result = await LoginAuthServer(authLoginWithCallback);
            if (!result.Ok)
            {
                return result;
            }

            result = await LoginGameServer();
            if (result.Ok)
            {
                loginPublisher.Publish(new LoginSuccess { Method = authLoginMethodName });
            }

            return result;
        }
    }
}