using System.Collections.Specialized;
using System.Linq;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Views;
using Microsoft.Extensions.Logging;
using TPFive.Game.Login.Entry.Repository;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using AppEntrySettings = TPFive.Game.App.Entry.Settings;
using AppLifetimeScope = TPFive.Game.App.Entry.LifetimeScope;
using ObservableOptionDataList = Loxodon.Framework.Observables.ObservableList<TMPro.TMP_Dropdown.OptionData>;

namespace TPFive.Game.Login.Entry.Views
{
    public class LoginWindow : TPFive.Game.UI.WindowBase
    {
        [SerializeField]
        private TMPro.TMP_Dropdown presetDropdown;
        [SerializeField]
        private TMPro.TMP_InputField usernameInputField;
        [SerializeField]
        private TMPro.TMP_InputField passwordInputField;
        [SerializeField]
        private TMPro.TextMeshProUGUI progressText;
        [SerializeField]
        private Button loginButton;
        [SerializeField]
        private Button signUpButton;
        [SerializeField]
        private Toggle passwordModeToggle;
        [SerializeField]
        private Button clearAllButton;

        private LoginWindowViewModel viewModel;
        private ObservableOptionDataList options;

        public ObservableOptionDataList Options
        {
            get => this.options;
            set
            {
                if (this.options == value)
                {
                    return;
                }

                if (this.options != null)
                {
                    this.options.CollectionChanged -= OnOptionDataChanged;
                }

                this.options = value;
                this.OnOptionDataListChanged();

                if (this.options != null)
                {
                    this.options.CollectionChanged += OnOptionDataChanged;
                }
            }
        }

        [Inject]
        public void Construct(
            ILoggerFactory loggerFactory,
            IService loginService,
            AccountRepository accountRepository,
            AppEntrySettings appEntrySettings,
            MessagePipe.IPublisher<TPFive.Game.SceneFlow.ChangeScene> pubSceneLoading,
            AppLifetimeScope lifetimeScope,
            Settings settings)
        {
            viewModel = new LoginWindowViewModel(
                loggerFactory,
                loginService,
                accountRepository,
                appEntrySettings,
                pubSceneLoading,
                lifetimeScope)
            {
                SignUpUrl = settings.SignUpUrl,
                CurrentScene = settings.CurrentScene,
                NextScene = settings.NextScene,
            };
        }

        protected override void OnCreate(IBundle bundle)
        {
            var bindingSet = this.CreateBindingSet(viewModel);
            bindingSet.Bind().For(v => v.Options).To(vm => vm.PresetOptions);
            bindingSet.Bind(presetDropdown).For(v => v.value, v => v.onValueChanged).To(vm => vm.CurrentPresetIndex).TwoWay();
            bindingSet.Bind(progressText).For(v => v.text).To(vm => vm.Progress).TwoWay();
            bindingSet.Bind(usernameInputField).For(v => v.text, v => v.onEndEdit).To(vm => vm.Username).TwoWay();
            bindingSet.Bind(passwordInputField).For(v => v.text, v => v.onEndEdit).To(vm => vm.Password).TwoWay();
            bindingSet.Bind(loginButton).For(v => v.onClick).To(vm => vm.LoginCommand).OneWay();
            bindingSet.Bind(signUpButton).For(v => v.onClick).To(vm => vm.SignUpCommand).OneWay();
            bindingSet.Bind().For(v => v.OnDismissRequested).To(vm => vm.DismissRequest);
            bindingSet.Bind(clearAllButton).For(v => v.onClick).To(vm => vm.ClearPresetsCommand).OneWay();
            bindingSet.Build();

            passwordModeToggle.onValueChanged.AddListener(_ =>
            {
                if (passwordInputField.contentType == TMPro.TMP_InputField.ContentType.Standard)
                {
                    passwordInputField.contentType = TMPro.TMP_InputField.ContentType.Password;
                }
                else
                {
                    passwordInputField.contentType = TMPro.TMP_InputField.ContentType.Standard;
                }

                passwordInputField.ForceLabelUpdate();
            });
        }

        protected override void OnShow()
        {
            base.OnShow();
            viewModel.ShowCommand.Execute(null);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            this.viewModel?.Dispose();
        }

        private void OnDismissRequested(object sender, InteractionEventArgs args)
        {
            this.Dismiss().OnFinish(() => viewModel.GoToNextCommand.Execute(null));
        }

        private void OnOptionDataListChanged()
        {
            presetDropdown.options = options.ToList();
            presetDropdown.RefreshShownValue();
        }

        private void OnOptionDataChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        for (int i = 0; i < e.NewItems.Count; ++i)
                        {
                            var item = (TMPro.TMP_Dropdown.OptionData)e.NewItems[i];
                            presetDropdown.options.Insert(e.NewStartingIndex + i, item);
                        }

                        presetDropdown.RefreshShownValue();
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        presetDropdown.options.RemoveRange(e.OldStartingIndex, e.OldItems.Count);
                        presetDropdown.RefreshShownValue();
                    }

                    break;
                case NotifyCollectionChangedAction.Replace:
                    {
                        var item = (TMPro.TMP_Dropdown.OptionData)e.NewItems[0];
                        presetDropdown.options[e.OldStartingIndex] = item;
                        presetDropdown.RefreshShownValue();
                    }

                    break;
                case NotifyCollectionChangedAction.Reset:
                    presetDropdown.ClearOptions();
                    break;
                case NotifyCollectionChangedAction.Move:
                    {
                        var item = (TMPro.TMP_Dropdown.OptionData)e.NewItems[0];
                        presetDropdown.options.RemoveAt(e.OldStartingIndex);
                        presetDropdown.options.Insert(e.NewStartingIndex, item);
                        presetDropdown.RefreshShownValue();
                    }

                    break;
            }
        }
    }
}
