using Loxodon.Framework.Binding;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Views;
using Microsoft.Extensions.Logging;
using TPFive.Game.Hud;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VContainer;

namespace TPFive.Game.AvatarEdit.Entry
{
    internal class EditAvatarDetailWindow : UI.WindowBase
    {
        [FormerlySerializedAs("m_FullRawImage")]
        [SerializeField]
        private RawImage _fullRawImage;
        [FormerlySerializedAs("m_HalfRawImage")]
        [SerializeField]
        private RawImage _halfRawImage;
        [FormerlySerializedAs("m_AvatarEditing")]
        [SerializeField]
        private GameObject _avatarEditing;
        [FormerlySerializedAs("m_BtnBack")]
        [SerializeField]
        private Button _btnBack;
        [FormerlySerializedAs("m_BtnSave")]
        [SerializeField]
        private Button _btnSave;
        [FormerlySerializedAs("m_PartListView")]
        [SerializeField]
        private EditorPartListView _partListView;
        [FormerlySerializedAs("m_SubPartListView")]
        [SerializeField]
        private EditorPartListView _subPartListView;
        [FormerlySerializedAs("m_EditorPanel")]
        [SerializeField]
        private GameObject _editorPanel;
        [FormerlySerializedAs("m_PresetStylePanel")]
        [SerializeField]
        private EditorPresetStylePanelView _presetStylePanel;
        [FormerlySerializedAs("m_BtnAppearances")]
        [SerializeField]
        private Toggle _btnAppearances;
        [FormerlySerializedAs("m_BtnApparels")]
        [SerializeField]
        private Toggle _btnApparels;
        [FormerlySerializedAs("m_BtnMakeup")]
        [SerializeField]
        private Toggle _btnMakeup;

        private EditAvatarDetailWindowViewModel _viewModel;

        public UnityEvent ShowEvent { get; set; } = new UnityEvent();

        public static string GetWindowAssetPath(IUIConfiguration configuration)
        {
            return $"Prefabs/{configuration.GetRootDirName()}/EditAvatarDetailWindow.prefab";
        }

        [Inject]
        public void Construct(
            ILoggerFactory loggerFactory,
            IAvatarEditController controller,
            AvatarEditSettings settings)
        {
            _viewModel = new EditAvatarDetailWindowViewModel(
                loggerFactory,
                controller.CurrentAvatarFormatInfo.Type,
                controller.GetStyleItems(),
                AvatarEditorPage.Appearances,
                controller,
                controller.AvatarStyleAccessor,
                settings.AvatarRenderTexture,
                string.Empty,
                string.Empty,
                null);
        }

        protected override void OnCreate(IBundle bundle)
        {
            var bindingSet = this.CreateBindingSet(_viewModel);
            bindingSet.Bind(_btnBack).For(v => v.onClick).To(vm => vm.BackCmd);
            bindingSet.Bind(_btnSave).For(v => v.onClick).To(vm => vm.SaveCmd);
            bindingSet.Bind(_partListView).For(v => v.PartCells).To(vm => vm.PartCells);
            bindingSet.Bind(_subPartListView.gameObject).For(v => v.activeSelf).ToExpression(vm => vm.SubPartCells.Count > 0);
            bindingSet.Bind(_subPartListView).For(v => v.PartCells).To(vm => vm.SubPartCells);
            bindingSet.Bind(_fullRawImage).For(v => v.texture).To(vm => vm.Texture);
            bindingSet.Bind(_fullRawImage.gameObject).For(v => v.activeSelf).To(vm => vm.ShowFullImg);
            bindingSet.Bind(_halfRawImage).For(v => v.texture).To(vm => vm.Texture);
            bindingSet.Bind(_halfRawImage.gameObject).For(v => v.activeSelf).To(vm => vm.ShowHalfImg);
            bindingSet.Bind(_avatarEditing).For(v => v.activeSelf).To(vm => vm.IsEditingAvatar);
            bindingSet.Bind(_presetStylePanel).For(v => v.Interactable).ToExpression(vm => !vm.IsEditingAvatar);
            bindingSet.Bind().For(v => v.ShowEvent).To(vm => vm.WindowShowCmd);
            bindingSet.Bind(_btnAppearances).For(v => v.onValueChanged).To(vm => vm.ShowAppearancesCmd);
            bindingSet.Bind(_btnApparels).For(v => v.onValueChanged).To(vm => vm.ShowApparelsCmd);
            bindingSet.Bind(_btnMakeup).For(v => v.onValueChanged).To(vm => vm.ShowMakeupCmd);
            bindingSet.Bind(_btnAppearances).For(v => v.isOn).ToExpression(vm => vm.CurrentStylePage == AvatarEditorPage.Appearances);
            bindingSet.Bind(_btnApparels).For(v => v.isOn).ToExpression(vm => vm.CurrentStylePage == AvatarEditorPage.Apparels);
            bindingSet.Bind(_btnMakeup).For(v => v.isOn).ToExpression(vm => vm.CurrentStylePage == AvatarEditorPage.Makeup);
            bindingSet.Bind().For(v => OnDismissWindow).To(vm => vm.DismissRequest);
            bindingSet.Bind().For(v => OnShowPresetPanel).To(vm => vm.ShowPresetAvatarPanelRequest);
            bindingSet.Build();

            if (_viewModel.PresetStylePanelViewModel != null)
            {
                _presetStylePanel.SetDataContext(_viewModel.PresetStylePanelViewModel);

                if (!_presetStylePanel.Created)
                {
                    _presetStylePanel.Create();
                }
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _viewModel?.Dispose();
        }

        private void OnDismissWindow(object sender, InteractionEventArgs args)
        {
            Dismiss();
        }

        private void OnShowPresetPanel(object sender, InteractionEventArgs args)
        {
            if (args.Context is EditorPresetStylePanelViewModel viewModel)
            {
                _presetStylePanel.SetDataContext(viewModel);

                if (!_presetStylePanel.Created)
                {
                    _presetStylePanel.Create();
                }
            }
        }
    }
}