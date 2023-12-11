using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.ViewModels;
using Microsoft.Extensions.Logging;
using TPFive.Game.Login.Entry.Models;
using TPFive.Game.Login.Entry.Repository;
using AppEntrySettings = TPFive.Game.App.Entry.Settings;
using AppLifetimeScope = TPFive.Game.App.Entry.LifetimeScope;
using ObservableOptionDataList = Loxodon.Framework.Observables.ObservableList<TMPro.TMP_Dropdown.OptionData>;
using OptionDataList = System.Collections.Generic.List<TMPro.TMP_Dropdown.OptionData>;

namespace TPFive.Game.Login.Entry.Views
{
    public class LoginWindowViewModel : ViewModelBase
    {
        private readonly ILogger logger;
        private readonly IService loginService;
        private readonly AccountRepository accountRepository;
        private readonly AppEntrySettings appEntrySettings;
        private readonly MessagePipe.IPublisher<TPFive.Game.SceneFlow.ChangeScene> pubSceneLoading;
        private readonly AppLifetimeScope lifetimeScope;

        private readonly RelayCommand logInCommand;
        private readonly RelayCommand signUpCommand;
        private readonly RelayCommand showCommand;
        private readonly RelayCommand goToNextSceneCommand;
        private readonly RelayCommand clearPresetsCommand;
        private readonly InteractionRequest dismissRequest;

        private readonly CancellationTokenSource cancellationTokenSource;

        private string progress;
        private Account account;
        private Account[] accounts;
        private int currentPresetIndex = -1;
        private ObservableOptionDataList presetOptions;

        public LoginWindowViewModel(
            ILoggerFactory loggerFactory,
            IService loginService,
            AccountRepository accountRepository,
            AppEntrySettings appEntrySettings,
            MessagePipe.IPublisher<TPFive.Game.SceneFlow.ChangeScene> pubSceneLoading,
            AppLifetimeScope lifetimeScope)
        {
            this.logger = loggerFactory.CreateLogger<LoginWindowViewModel>();
            this.loginService = loginService;
            this.accountRepository = accountRepository;
            this.appEntrySettings = appEntrySettings;
            this.pubSceneLoading = pubSceneLoading;
            this.lifetimeScope = lifetimeScope;
            this.presetOptions = new ObservableOptionDataList();

            this.logInCommand = new RelayCommand(OnLogin, IsInteractable);
            this.signUpCommand = new RelayCommand(OnSignUp, IsInteractable);
            this.showCommand = new RelayCommand(Show, IsInteractable);
            this.goToNextSceneCommand = new RelayCommand(OnGoToNextScene, IsInteractable);
            this.clearPresetsCommand = new RelayCommand(ClearPresets, IsInteractable);

            this.dismissRequest = new InteractionRequest();

            this.cancellationTokenSource = new CancellationTokenSource();
        }

        public ICommand LoginCommand => logInCommand;

        public ICommand SignUpCommand => signUpCommand;

        public ICommand ShowCommand => showCommand;

        public ICommand GoToNextCommand => goToNextSceneCommand;

        public ICommand ClearPresetsCommand => clearPresetsCommand;

        public IInteractionRequest DismissRequest => dismissRequest;

        public string Username
        {
            get => account.Username;
            set => Set(ref account.Username, value);
        }

        public string Password
        {
            get => account.Password;
            set => Set(ref account.Password, value);
        }

        public string Progress
        {
            get => progress;
            set => Set(ref progress, value);
        }

        public ObservableOptionDataList PresetOptions
        {
            get => presetOptions;
            set => Set(ref presetOptions, value);
        }

        public int CurrentPresetIndex
        {
            get => currentPresetIndex;
            set
            {
                if (!Set(ref currentPresetIndex, value))
                {
                    return;
                }

                if (accounts != null && value >= 0 && value < accounts.Length)
                {
                    Username = accounts[value].Username;
                    Password = accounts[value].Password;
                }
            }
        }

        public string SignUpUrl { get; set; }

        public string CurrentScene { get; set; }

        public string NextScene { get; set; }

        private bool Interactable { get; set; } = true;

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
            }
        }

        private bool IsInteractable() => Interactable;

        private void OnLogin()
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug(nameof(OnLogin));
            }

            if (account.IsValid() &&
                !string.IsNullOrEmpty(Password))
            {
                _ = SignInAsync(cancellationTokenSource.Token);
            }
        }

        private async UniTaskVoid SignInAsync(CancellationToken token)
        {
            Interactable = false;
            SetProgress("Going to sign in...");
            try
            {
                token.ThrowIfCancellationRequested();
                var result = await loginService.LoginByPassword(Username, Password);
                if (result.Ok)
                {
                    account.UpdateLastLogonTimestamp();
                    accountRepository.UpdateOrAdd(account);
                    dismissRequest.Raise();
                    SetProgress(string.Empty);
                }
                else
                {
                    SetProgress(result.Error, LogLevel.Error);
                }
            }
            catch (OperationCanceledException)
            {
                // Not going to do anything.
            }
            catch (Exception e)
            {
                if (logger.IsEnabled(LogLevel.Error))
                {
                    logger.LogError(e, "Failed to login");
                }
            }
            finally
            {
                Interactable = true;
            }
        }

        private void OnSignUp()
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug(nameof(OnSignUp));
            }

            if (!string.IsNullOrEmpty(SignUpUrl))
            {
                UnityEngine.Application.OpenURL(SignUpUrl);
            }
        }

        private void OnGoToNextScene()
        {
            GoToNextScene();
        }

        private void GoToNextScene()
        {
            Interactable = false;
            try
            {
                if (!TryGetSceneProperty(CurrentScene, out var currentProperty))
                {
                    LogMissingSceneError(CurrentScene);
                    return;
                }

                if (!TryGetSceneProperty(NextScene, out var nextProperty))
                {
                    LogMissingSceneError(NextScene);
                    return;
                }

                pubSceneLoading.Publish(
                    new Game.SceneFlow.ChangeScene()
                    {
                        FromCategory = currentProperty.category,
                        FromTitle = currentProperty.addressableKey,
                        FromCategoryOrder = currentProperty.categoryOrder,
                        FromSubOrder = currentProperty.subOrder,
                        ToCategory = nextProperty.category,
                        ToTitle = nextProperty.addressableKey,
                        ToCategoryOrder = nextProperty.categoryOrder,
                        ToSubOrder = nextProperty.subOrder,
                        LifetimeScope = lifetimeScope,
                    });
            }
            catch (Exception e)
            {
                if (logger.IsEnabled(LogLevel.Error))
                {
                    logger.LogError(e, "Failed to go to next scene");
                }
            }
            finally
            {
                Interactable = false;
            }
        }

        private void LogMissingSceneError(string title)
        {
            if (logger.Equals(LogLevel.Error))
            {
                logger.LogError("Failed to find {0} scene property", title);
            }
        }

        private bool TryGetSceneProperty(string title, out SceneProperty property)
        {
            var index = appEntrySettings.ScenePropertyList.FindIndex(x =>
            {
                return x.title.Equals(title, StringComparison.Ordinal);
            });
            property = index != -1 ? appEntrySettings.ScenePropertyList[index] : null;
            return property != null;
        }

        private void Show()
        {
            accounts = accountRepository.GetAll();
            var options = CreateOptions(accounts);
            PresetOptions = new ObservableOptionDataList(options);

            if (accounts.Length != 0)
            {
                var lastLogonAccount = this.accountRepository.GetLastLogonAccount();
                var index = lastLogonAccount.IsValid() ? Array.IndexOf(accounts, lastLogonAccount) : 0;
                CurrentPresetIndex = index;
            }
        }

        private OptionDataList CreateOptions(Account[] accounts)
        {
            var optionDataList = new OptionDataList(accounts.Length);
            foreach (var account in accounts)
            {
                var name = account.GetDisplayName();
                optionDataList.Add(new TMPro.TMP_Dropdown.OptionData(name));
            }

            return optionDataList;
        }

        private void SetProgress(string text, LogLevel level = LogLevel.Information)
        {
            string color = null;
            switch (level)
            {
                case LogLevel.Warning:
                    color = "yellow";
                    break;
                case LogLevel.Error:
                case LogLevel.Critical:
                    color = "red";
                    break;
                default:
                    break;
            }

            if (color == null)
            {
                Progress = text;
            }
            else
            {
                Progress = string.Format("<color=\"{0}\">{1}</color>", color, text);
            }
        }

        private void ClearPresets()
        {
            accounts = Array.Empty<Account>();
            PresetOptions.Clear();
            CurrentPresetIndex = -1;
            accountRepository.RemoveAll();
        }
    }
}
