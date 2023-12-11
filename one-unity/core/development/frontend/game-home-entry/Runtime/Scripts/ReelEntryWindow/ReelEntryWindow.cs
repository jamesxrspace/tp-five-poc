using System.Collections.Generic;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Views;
using MessagePipe;
using TMPro;
using TPFive.Game.App.Entry;
using TPFive.Game.Record;
using TPFive.Game.UI;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using LifetimeScope = TPFive.Home.Entry.LifetimeScope;

namespace TPFive.Game.Home.Entry
{
    public class ReelEntryWindow : WindowBase
    {
        [SerializeField]
        private Button browseReelButton;

        [SerializeField]
        private Button closeButton;

        private ReelEntryViewModel viewModel;

        [SerializeField]
        private ReelEntryTemplateSetting setting;

        [SerializeField]
        private Button templateButtonPrefab;

        [SerializeField]
        private Transform templateParent;

        private List<ReelSceneDesc> sceneTemplateList = new List<ReelSceneDesc>();

        public List<ReelSceneDesc> SceneTemplateList
        {
            get => sceneTemplateList;
            set
            {
                sceneTemplateList = value;
                RefreshButton(value);
            }
        }

        [Inject]
        public void Construct(
            Config.IService configService,
            IPublisher<SceneFlow.ChangeScene> pubSceneLoading,
            Settings appEntrySettings,
            LifetimeScope lifetimeScope)
        {
            viewModel = new ReelEntryViewModel(
                configService,
                pubSceneLoading,
                appEntrySettings,
                setting,
                lifetimeScope);
        }

        protected override void OnCreate(IBundle bundle)
        {
            var bindingSet = this.CreateBindingSet(viewModel);
            bindingSet.Bind(browseReelButton).For(v => v.onClick).To(vm => vm.BrowseReelsCommand);
            bindingSet.Bind().For(v => v.SceneTemplateList).To(vm => vm.SceneTemplateList).OneWay();
            bindingSet.Bind(closeButton).For(v => v.onClick).To(vm => vm.CloseCommand);
            bindingSet.Bind().For(v => v.OnCloseRequest).To(vm => vm.CloseRequest);
            bindingSet.Build();
        }

        protected override void OnDestroy()
        {
            viewModel?.Dispose();

            base.OnDestroy();
        }

        // Init the testing template button, the official information will come from flutter.
        private void RefreshButton(List<ReelSceneDesc> templates)
        {
            for (var i = 0; i < templateParent.childCount; ++i)
            {
                Destroy(templateParent.GetChild(i).gameObject);
            }

            foreach (var template in templates)
            {
                var button = Instantiate(templateButtonPrefab, templateParent);
                button.GetComponentInChildren<TextMeshProUGUI>().text = template.Name;
                button.onClick.AddListener(() =>
                {
                    viewModel.CreateReelCommand.Execute(template);
                });
            }
        }

        private void OnCloseRequest(object sender, InteractionEventArgs args)
        {
            this.Dismiss();
        }
    }
}
