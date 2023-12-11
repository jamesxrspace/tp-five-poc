using System;
using System.Collections.Generic;
using System.Threading;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Observables;
using Loxodon.Framework.ViewModels;
using Microsoft.Extensions.Logging;
using UnityEngine;
using XR.AvatarEditing;
using XR.AvatarEditing.Core;
using XR.AvatarEditing.Model;

namespace TPFive.Game.AvatarEdit.Entry
{
    internal class EditorPartCellViewModel : ViewModelBase
    {
        private readonly InteractionRequest<CameraMovement> _changeAvatarImageRequest;
        private readonly InteractionRequest<EditorPresetStylePanelViewModel> _showPresetPanelRequest;
        private readonly SimpleCommand<bool> _onSelectedCmd;

        private readonly Dictionary<string, ViewModelBase> _presetStylePanelVMs;
        private readonly IAvatarEditController _editorController;
        private readonly IAvatarStyleAccessor _styleAccessor;
        private readonly AvatarType _avatarType;
        private readonly Dictionary<string, EditAvatarDetailWindowViewModel.PartUIDesc> _uiDescMap;
        private readonly Dictionary<string, EditAvatarDetailWindowViewModel.PresetIcon> _presetIconHeader;
        private readonly ObservableList<EditorPartCellViewModel> _subPartCells;
        private readonly string _path;
        private readonly string _initTabPath;
        private readonly AvatarStyleItem _partItem;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly ILoggerFactory _loggerFactory;

        private bool _selected;
        private string[] _icon;
        private string _initAssetId;
        private bool _haveNewItem;
        private bool _disposed = false;

        public EditorPartCellViewModel(
            ILoggerFactory loggerFactory,
            AvatarType avatarType,
            Dictionary<string, EditAvatarDetailWindowViewModel.PartUIDesc> partUIDescMap,
            Dictionary<string, EditAvatarDetailWindowViewModel.PresetIcon> presetIconHeader,
            AvatarStyleItem partItem,
            Dictionary<string, ViewModelBase> presetStylePanelVMs,
            string path,
            string initTabPath,
            string initAssetId,
            IAvatarEditController controller,
            IAvatarStyleAccessor styleAccessor,
            IInteractionRequest changeAvatarImageRequest,
            IInteractionRequest showPresetAvatarPanelRequest)
        {
            _loggerFactory = loggerFactory;
            _avatarType = avatarType;
            _uiDescMap = partUIDescMap;
            _presetIconHeader = presetIconHeader;
            _partItem = partItem;
            _icon = partUIDescMap[partItem.Path].Icons;
            _path = path;
            _initTabPath = initTabPath;
            _initAssetId = initAssetId;
            _presetStylePanelVMs = presetStylePanelVMs;
            _editorController = controller;
            _styleAccessor = styleAccessor;
            _changeAvatarImageRequest = changeAvatarImageRequest as InteractionRequest<CameraMovement>;
            _showPresetPanelRequest = showPresetAvatarPanelRequest as InteractionRequest<EditorPresetStylePanelViewModel>;
            _subPartCells = new ObservableList<EditorPartCellViewModel>();
            _onSelectedCmd = new SimpleCommand<bool>(OnPartCellSelected);
            _cancellationTokenSource = new CancellationTokenSource();

            GetSubPartCells();
        }

        public string[] Icon
        {
            get { return _icon; }
            set { Set(ref _icon, value, nameof(Icon)); }
        }

        public bool IsSelected
        {
            get { return _selected; }
            set { Set(ref _selected, value, nameof(IsSelected)); }
        }

        public bool HaveNewItem
        {
            get { return _haveNewItem; }
            set { Set(ref _haveNewItem, value, nameof(HaveNewItem)); }
        }

        public string Path => _path;

        public ICommand OnSelectedCmd => _onSelectedCmd;

        public Action<ObservableList<EditorPartCellViewModel>> OnSelected { get; set; }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            base.Dispose(disposing);

            if (disposing)
            {
                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource.Dispose();
                }

                if (IsSelected)
                {
                    DeSelectPartCell();
                }

                ClearSubPartCell();
            }

            _disposed = true;
        }

        private void GetSubPartCells()
        {
            if (_partItem.Items == null)
            {
                return;
            }

            foreach (var item in _partItem.Items)
            {
                if (_uiDescMap.ContainsKey(item.Path))
                {
                    var subPartCell = new EditorPartCellViewModel(
                        _loggerFactory,
                        _avatarType,
                        _uiDescMap,
                        _presetIconHeader,
                        item,
                        _presetStylePanelVMs,
                        _path + "/" + item.name,
                        _initTabPath,
                        _initAssetId,
                        _editorController,
                        _styleAccessor,
                        _changeAvatarImageRequest,
                        _showPresetPanelRequest);

                    _subPartCells.Add(subPartCell);
                }
            }
        }

        private void ClearSubPartCell()
        {
            foreach (var subPartCell in _subPartCells)
            {
                subPartCell.Dispose();
            }

            _subPartCells.Clear();
        }

        private void OnPartCellSelected(bool isOn)
        {
            if (isOn)
            {
                SelectPartCell();
            }
            else
            {
                DeSelectPartCell();
            }
        }

        private void SelectPartCell()
        {
            if (!IsSelected)
            {
                IsSelected = true;
            }

            // invoke the sub cells to UI View for showing the sub part
            OnSelected?.Invoke(_subPartCells);

            // if have sub part, select first sub part or specific sub part
            if (_subPartCells.Count > 0)
            {
                foreach (var subPartCell in _subPartCells)
                {
                    // _initTabPath is like "tattoo/face_tattoo" or "eyes/eyebrow"
                    // partCell.Path is like "tattoo" or "eyes"
                    if (_initTabPath.StartsWith(subPartCell.Path))
                    {
                        subPartCell.OnSelectedCmd.Execute(true);
                        return;
                    }
                }

                _subPartCells[0].OnSelectedCmd.Execute(true);
                return;
            }

            // if not have sub part, continue to generate preset style panel
            var partUIData = _uiDescMap[_partItem.Path];

            _changeAvatarImageRequest.Raise(partUIData.Mode);

            if (_presetStylePanelVMs.ContainsKey(_partItem.Path))
            {
                var model = _presetStylePanelVMs[_partItem.Path];
                if (model != null)
                {
                    model.Dispose();
                }
            }

            _presetStylePanelVMs[_partItem.Path] = new EditorPresetStylePanelViewModel(
                _loggerFactory,
                _avatarType,
                _presetIconHeader,
                _partItem,
                _styleAccessor,
                _initAssetId);

            _showPresetPanelRequest.Raise((EditorPresetStylePanelViewModel)_presetStylePanelVMs[_partItem.Path]);

            _initAssetId = string.Empty;
        }

        private void DeSelectPartCell()
        {
            if (IsSelected)
            {
                IsSelected = false;
            }

            foreach (var subPartCell in _subPartCells)
            {
                if (subPartCell.IsSelected)
                {
                    subPartCell.OnSelectedCmd.Execute(false);
                }
            }
        }
    }
}
