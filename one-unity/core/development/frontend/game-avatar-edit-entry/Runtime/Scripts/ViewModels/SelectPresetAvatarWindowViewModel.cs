using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.ViewModels;
using Microsoft.Extensions.Logging;
using GameLoggingUtility = TPFive.Game.Logging.Utility;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Game.AvatarEdit.Entry
{
    internal sealed class SelectPresetAvatarWindowViewModel : ViewModelBase
    {
        private readonly SimpleCommand _confirmCmd;
        private readonly SimpleCommand _cancelCmd;
        private readonly InteractionRequest _dismissRequest;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IList<AvatarFormatInfo> _presetInfoList;
        private readonly IAvatarEditController _controller;

        private bool _disposed = false;
        private PresetAvatarScrollViewModel _scrollViewViewModel;

        public SelectPresetAvatarWindowViewModel(
            ILoggerFactory loggerFactory,
            IAvatarEditController controller,
            IList<AvatarFormatInfo> presetInfoList,
            AvatarEditSettings editSettings)
        {
            Logger = GameLoggingUtility.CreateLogger<SelectPresetAvatarWindowViewModel>(loggerFactory);
            _loggerFactory = loggerFactory;
            _controller = controller;

            _confirmCmd = new SimpleCommand(OnConfirmBtnClicked);
            _cancelCmd = new SimpleCommand(OnCancelBtnClicked);
            _dismissRequest = new InteractionRequest();

            _scrollViewViewModel = new PresetAvatarScrollViewModel();

            _presetInfoList = presetInfoList;
            var presetAvatarCellDatas = new PresetAvatarScrollViewCellData[_presetInfoList.Count];
            for (int i = 0; i < _presetInfoList.Count; ++i)
            {
                var idlePath = string.Format(editSettings.PresetIdleImagePathFormat, _presetInfoList[i].Name);
                var selectedPath = string.Format(editSettings.PresetSelectedImagePathFormat, _presetInfoList[i].Name);
                presetAvatarCellDatas[i] = new PresetAvatarScrollViewCellData(i, idlePath, selectedPath, loggerFactory);
            }

            _scrollViewViewModel.AddItems(presetAvatarCellDatas);
        }

        ~SelectPresetAvatarWindowViewModel()
        {
            Dispose(false);
        }

        public ICommand ConfirmCmd => _confirmCmd;

        public ICommand CancelCmd => _cancelCmd;

        public IInteractionRequest DismissRequest => _dismissRequest;

        public PresetAvatarScrollViewModel ScrollViewViewModel
        {
            get => _scrollViewViewModel;
            set => Set(ref _scrollViewViewModel, value, nameof(ScrollViewViewModel));
        }

        public ILoggerFactory LoggerFactory => _loggerFactory;

        private ILogger Logger { get; set; }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _scrollViewViewModel.Dispose();
            }

            _disposed = true;

            base.Dispose(disposing);
        }

        private void OnConfirmBtnClicked()
        {
            OnNextBtnClickedAsync().Forget();
        }

        private void OnCancelBtnClicked()
        {
            _dismissRequest.Raise();
            _controller.GoToHomeEntry();
        }

        private void EnableAllCmds(bool value)
        {
            _cancelCmd.Enabled = value;
            _confirmCmd.Enabled = value;
        }

        private async UniTaskVoid OnNextBtnClickedAsync()
        {
            try
            {
                _confirmCmd.Enabled = false;
                _controller.EnableLoadingPanel(true);

                await CreateAvatar();

                _dismissRequest.Raise();
                _controller.ShowMainEditWindow().Forget();
            }
            catch (Exception e)
            {
                Logger.LogWarning(e, $"{nameof(SelectPresetAvatarWindowViewModel)} CreateAvatar failed.");
            }
            finally
            {
                _controller.EnableLoadingPanel(false);

                _confirmCmd.Enabled = true;
            }
        }

        private async UniTask<bool> CreateAvatar()
        {
            EnableAllCmds(false);

            var index = _scrollViewViewModel.CurrentIndex;
            var cellIndex = _scrollViewViewModel.Items[index].Index;
            if (cellIndex < 0 || cellIndex >= _presetInfoList.Count)
            {
                EnableAllCmds(true);
                return false;
            }

            var info = _presetInfoList[cellIndex];
            bool result = await _controller.CreatePreviewAavatar(info);

            if (!result)
            {
                EnableAllCmds(true);
            }

            return result;
        }
    }
}