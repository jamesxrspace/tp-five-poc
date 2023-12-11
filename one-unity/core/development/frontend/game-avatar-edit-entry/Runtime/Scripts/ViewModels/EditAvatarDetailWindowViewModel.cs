using System;
using System.Collections.Generic;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Observables;
using Loxodon.Framework.ViewModels;
using Microsoft.Extensions.Logging;
using UnityEngine;
using XR.AvatarEditing;
using XR.AvatarEditing.Core;
using XR.AvatarEditing.Model;
using GameLoggingUtility = TPFive.Game.Logging.Utility;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Game.AvatarEdit.Entry
{
    internal class EditAvatarDetailWindowViewModel : ViewModelBase
    {
        private readonly Dictionary<string, PartUIDesc> _uiDescMap = new Dictionary<string, PartUIDesc>()
        {
            // Editor Tab
            { "/appearance/head",        new PartUIDesc(new string[] { "⬷" }, CameraMovement.Half) },
            { "/appearance/eyes",        new PartUIDesc(new string[] { "⭅", "⭇" }, CameraMovement.Half) },
            { "/appearance/nose_preset", new PartUIDesc(new string[] { "⬴" }, CameraMovement.Half) },
            { "/appearance/lip_preset",  new PartUIDesc(new string[] { "⬵" }, CameraMovement.Half) },
            { "/appearance/hair",        new PartUIDesc(new string[] { "💈" }, CameraMovement.Half) },
            { "/appearance/beard",       new PartUIDesc(new string[] { "⭒" }, CameraMovement.Half) },
            { "/appearance/Body",        new PartUIDesc(new string[] { "%" }, CameraMovement.Full) },
            { "/appearance/tattoo",      new PartUIDesc(new string[] { "🐲" }, CameraMovement.Half) },
            { "/apparel/facedeck",       new PartUIDesc(new string[] { "⭁" }, CameraMovement.Half) },
            { "/apparel/coat",           new PartUIDesc(new string[] { "⬰" }, CameraMovement.Full) },
            { "/apparel/pants",          new PartUIDesc(new string[] { "⭐" }, CameraMovement.Full) },
            { "/apparel/body_socks",     new PartUIDesc(new string[] { "🧦" }, CameraMovement.Full) },
            { "/apparel/foot",           new PartUIDesc(new string[] { "⬲" }, CameraMovement.Full) },
            { "/apparel/suit",           new PartUIDesc(new string[] { "⭉" }, CameraMovement.Full) },
            { "/apparel/back",           new PartUIDesc(new string[] { "🎒" }, CameraMovement.Full) },
            { "/makeup/blush",           new PartUIDesc(new string[] { "⭃", "⭓" }, CameraMovement.Half) },
            { "/makeup/eyeshadow",       new PartUIDesc(new string[] { "⭅", "⭄" }, CameraMovement.Half) },
            { "/makeup/lipstick",        new PartUIDesc(new string[] { "⬵" }, CameraMovement.Half) },
            { "/makeup/nail",            new PartUIDesc(new string[] { "💅" }, CameraMovement.Full) },

            // Editor Sub Tab
            { "/appearance/eyes/eyes",           new PartUIDesc(new string[] { "Avatar 2.0/Eyes" }, CameraMovement.Half) },
            { "/appearance/eyes/eye",            new PartUIDesc(new string[] { "Avatar 2.0/Pupil" }, CameraMovement.Half) },
            { "/appearance/eyes/eyebrow",        new PartUIDesc(new string[] { "Avatar 2.0/Eyebrow" }, CameraMovement.Half) },
            { "/appearance/Body/Body",           new PartUIDesc(new string[] { "Avatar 2.0/Body" }, CameraMovement.Full) },
            { "/appearance/Body/body_scar",      new PartUIDesc(new string[] { "Avatar 2.0/Decoration" }, CameraMovement.Full) },
            { "/appearance/tattoo/face_tattoo",  new PartUIDesc(new string[] { "Avatar 2.0/Head" }, CameraMovement.Half) },
            { "/appearance/tattoo/face_scar",    new PartUIDesc(new string[] { "Avatar 2.0/Decoration" }, CameraMovement.Half) },
            { "/appearance/tattoo/body_tattoo1", new PartUIDesc(new string[] { "Avatar 2.0/Body" }, CameraMovement.Full) },
            { "/appearance/tattoo/body_tattoo2", new PartUIDesc(new string[] { "Avatar 2.0/Arm" }, CameraMovement.Full) },
            { "/appearance/tattoo/body_tattoo3", new PartUIDesc(new string[] { "Avatar 2.0/Feet" }, CameraMovement.Full) },
        };

        private readonly Dictionary<string, PresetIcon> _presetIconHeader = new Dictionary<string, PresetIcon>()
        {
            { "face_preset", new PresetIcon("face") },
            { "face_scar", new PresetIcon("head") },
            { "eyes_preset", new PresetIcon("eyes") },
            { "eye", new PresetIcon("eye") },
            { "eyebrow_preset", new PresetIcon("brow") },
            { "nose_preset", new PresetIcon("nose") },
            { "lip_preset", new PresetIcon("lip") },
            { "beard_style", new PresetIcon("head") },
            { "body_scar", new PresetIcon("body") },
            { "face_tattoo", new PresetIcon("head") },
            { "body_tattoo1", new PresetIcon("body") },
            { "body_tattoo2", new PresetIcon("body") },
            { "body_tattoo3", new PresetIcon("body") },
            { "body_socks", new PresetIcon("body") },
            { "blush", new PresetIcon("head") },
            { "eyeshadow", new PresetIcon("head") },
            { "lipstick", new PresetIcon("head") },
            { "top", new PresetIcon(string.Empty) },
        };

        private readonly IAvatarEditController _editorController;
        private readonly IAvatarStyleAccessor _styleAccessor;
        private readonly AvatarType _avatarType;
        private readonly IReadOnlyDictionary<AvatarEditorPage, AvatarStyleItem> _styleItems;
        private readonly RenderTexture _texture;
        private readonly ILoggerFactory _loggerFactory;

        private readonly SimpleCommand _windowShowCmd;
        private readonly SimpleCommand _backCmd;
        private readonly SimpleCommand _saveCmd;
        private readonly SimpleCommand<bool> _showAppearancesCmd;
        private readonly SimpleCommand<bool> _showApparelsCmd;
        private readonly SimpleCommand<bool> _showMakeupCmd;
        private readonly InteractionRequest _dismissRequest;
        private readonly InteractionRequest<CameraMovement> _changeAvatarImageRequest;
        private readonly InteractionRequest<EditorPresetStylePanelViewModel> _showPresetPanelRequest;

        private Dictionary<string, ViewModelBase> _presetStylePanelVMs;
        private ObservableList<EditorPartCellViewModel> _partCells;
        private ObservableList<EditorPartCellViewModel> _subPartCells;
        private AvatarStyleItem[] _partItems;
        private bool _disposed = false;
        private string _initTabPath;
        private string _assetId;
        private AvatarEditorPage _currentStylePage;
        private bool _isEditingAvatar;
        private bool _showFullImg;
        private bool _showHalfImg;

        public EditAvatarDetailWindowViewModel(
            ILoggerFactory loggerFactory,
            AvatarType avatarType,
            IReadOnlyDictionary<AvatarEditorPage, AvatarStyleItem> styleItems,
            AvatarEditorPage stylepage,
            IAvatarEditController editorController,
            IAvatarStyleAccessor styleAccessor,
            RenderTexture texture,
            string tabName,
            string assetId,
            Action onBack)
        {
            _avatarType = avatarType;
            _styleItems = styleItems;
            _editorController = editorController;
            _styleAccessor = styleAccessor;
            _texture = texture;
            _loggerFactory = loggerFactory;
            _initTabPath = tabName;
            _assetId = assetId;

            Logger = GameLoggingUtility.CreateLogger<EditAvatarDetailWindowViewModel>(loggerFactory);
            _presetStylePanelVMs = new Dictionary<string, ViewModelBase>();
            _partCells = new ObservableList<EditorPartCellViewModel>();
            _subPartCells = new ObservableList<EditorPartCellViewModel>();
            _backCmd = new SimpleCommand(() => { BackClick(onBack); });
            _saveCmd = new SimpleCommand(SaveClick);

            _showAppearancesCmd = new SimpleCommand<bool>((result) =>
            {
                if (result)
                {
                    ShowEditorPage(AvatarEditorPage.Appearances);
                }
            });

            _showApparelsCmd = new SimpleCommand<bool>((result) =>
            {
                if (result)
                {
                    ShowEditorPage(AvatarEditorPage.Apparels);
                }
            });

            _showMakeupCmd = new SimpleCommand<bool>((result) =>
            {
                if (result)
                {
                    ShowEditorPage(AvatarEditorPage.Makeup);
                }
            });

            _windowShowCmd = new SimpleCommand(ShowInitTab);
            _dismissRequest = new InteractionRequest();
            _changeAvatarImageRequest = new InteractionRequest<CameraMovement>();
            _showPresetPanelRequest = new InteractionRequest<EditorPresetStylePanelViewModel>();

            _changeAvatarImageRequest.Raised += ChangeAvatarImage;
            _showPresetPanelRequest.Raised += ShowPresetPanel;

            ShowEditorPage(stylepage);
        }

        ~EditAvatarDetailWindowViewModel()
        {
            Dispose(false);
        }

        public ILoggerFactory LoggerFactory => _loggerFactory;

        public RenderTexture Texture => _texture;

        public AvatarEditorPage CurrentStylePage
        {
            get => _currentStylePage;
            set => Set(ref _currentStylePage, value, nameof(CurrentStylePage));
        }

        public bool IsEditingAvatar
        {
            get => _isEditingAvatar;
            set => Set(ref _isEditingAvatar, value, nameof(IsEditingAvatar));
        }

        public bool ShowFullImg
        {
            get => _showFullImg;
            set => Set(ref _showFullImg, value, nameof(ShowFullImg));
        }

        public bool ShowHalfImg
        {
            get => _showHalfImg;
            set => Set(ref _showHalfImg, value, nameof(ShowHalfImg));
        }

        public EditorPresetStylePanelViewModel PresetStylePanelViewModel { get; set; }

        public ICommand BackCmd => _backCmd;

        public ICommand SaveCmd => _saveCmd;

        public ICommand ShowAppearancesCmd => _showAppearancesCmd;

        public ICommand ShowApparelsCmd => _showApparelsCmd;

        public ICommand ShowMakeupCmd => _showMakeupCmd;

        public ICommand WindowShowCmd => _windowShowCmd;

        public IInteractionRequest DismissRequest => _dismissRequest;

        public IInteractionRequest ShowPresetAvatarPanelRequest => _showPresetPanelRequest;

        public ObservableList<EditorPartCellViewModel> PartCells => _partCells;

        public ObservableList<EditorPartCellViewModel> SubPartCells => _subPartCells;

        private ILogger Logger { get; set; }

        public void ShowInitTab()
        {
            EditorPartCellViewModel initPartCell = null;
            foreach (var partCell in _partCells)
            {
                // _initTabPath is like "tattoo/face_tattoo" or "eyes/eyebrow"
                // partCell.Path is like "tattoo" or "eyes"
                if (_initTabPath.StartsWith(partCell.Path))
                {
                    initPartCell = partCell;
                }
            }

            if (initPartCell != null)
            {
                initPartCell.OnSelectedCmd.Execute(true);
            }
            else
            {
                _partCells[0].OnSelectedCmd.Execute(true);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            base.Dispose(disposing);

            if (disposing)
            {
                _changeAvatarImageRequest.Raised -= ChangeAvatarImage;
                _showPresetPanelRequest.Raised -= ShowPresetPanel;
                ClearPartCells();
            }

            _disposed = true;
        }

        private void ChangeAvatarImage(object sender, InteractionEventArgs args)
        {
            var mode = (CameraMovement)args.Context;
            ShowFullImg = mode == CameraMovement.Full;
            ShowHalfImg = mode == CameraMovement.Half;
        }

        private void ShowPresetPanel(object sender, InteractionEventArgs args)
        {
            PresetStylePanelViewModel = (EditorPresetStylePanelViewModel)args.Context;
        }

        private void ShowEditorPage(AvatarEditorPage page)
        {
            if (CurrentStylePage == page)
            {
                return;
            }

            SetCurrentPage(page);
            ShowInitTab();
        }

        private void SetCurrentPage(AvatarEditorPage stylepage)
        {
            if (_styleItems == null)
            {
                return;
            }

            if (!_styleItems.TryGetValue(stylepage, out var styleItem))
            {
                Logger.LogError("can not get the style item when editing avatar.");
                return;
            }

            CurrentStylePage = stylepage;
            ClearPartCells();

            _partItems = styleItem.items;
            for (int i = 0; i < _partItems.Length; ++i)
            {
                var partItem = _partItems[i];

                var partCellViewModel = new EditorPartCellViewModel(
                    _loggerFactory,
                    _avatarType,
                    _uiDescMap,
                    _presetIconHeader,
                    partItem,
                    _presetStylePanelVMs,
                    partItem.name,
                    _initTabPath,
                    _assetId,
                    _editorController,
                    _styleAccessor,
                    _changeAvatarImageRequest,
                    _showPresetPanelRequest);

                partCellViewModel.OnSelected += OnPartCellSelected;

                _partCells.Add(partCellViewModel);
            }
        }

        private void OnPartCellSelected(ObservableList<EditorPartCellViewModel> subPartCell)
        {
            _subPartCells.Clear();
            foreach (var subPart in subPartCell)
            {
                _subPartCells.Add(subPart);
            }
        }

        private void ClearPartCells()
        {
            foreach (var partCell in _partCells)
            {
                partCell.OnSelected -= OnPartCellSelected;
                partCell.Dispose();
            }

            _partCells.Clear();
        }

        private void SaveClick()
        {
            _styleAccessor.Save();

            _dismissRequest.Raise();
            _editorController.ShowMainEditWindow().Forget();
        }

        private void BackClick(Action backAction)
        {
            _styleAccessor.Revert();
            GoToPreviousWindow(backAction);
        }

        private void GoToPreviousWindow(Action previousWindowAction)
        {
            if (previousWindowAction != null)
            {
                previousWindowAction.Invoke();
            }
            else
            {
                _editorController.ShowMainEditWindow().Forget();
            }

            _dismissRequest.Raise();
        }

        public class PartUIDesc
        {
            public PartUIDesc(string[] icons, CameraMovement mode)
            {
                Icons = icons;
                Mode = mode;
            }

            public string[] Icons { get; }

            public CameraMovement Mode { get; }
        }

        public class PresetIcon
        {
            public PresetIcon(string header, string maleNoneIcon = null, string femaleNoneIcon = null)
            {
                Header = header;
                MaleNoneIcon = maleNoneIcon;
                FemaleNoneIcon = femaleNoneIcon;
            }

            public string Header { get; }

            public string MaleNoneIcon { get; }

            public string FemaleNoneIcon { get; }
        }
    }
}