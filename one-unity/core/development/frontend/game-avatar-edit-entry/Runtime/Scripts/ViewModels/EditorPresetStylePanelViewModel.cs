using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using Loxodon.Framework.Observables;
using Loxodon.Framework.ViewModels;
using Microsoft.Extensions.Logging;
using TPFive.Game.Extensions;
using UnityEngine;
using XR.AvatarEditing;
using XR.AvatarEditing.Core;
using XR.AvatarEditing.Model;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Game.AvatarEdit.Entry
{
    internal class EditorPresetStylePanelViewModel : ViewModelBase
    {
        private static readonly Dictionary<string, SliderItem> FloatStyleItemMap = new Dictionary<string, SliderItem>()
        {
            { "body_height", new SliderItem(0.9f, 1f, 3) },
            { "body_shape", new SliderItem(new Dictionary<float, float>() { { 0f, 1f }, { 1f, 2f }, { 2f, 0f }, { 3f, 3f } }) },
        };

        private readonly AvatarStyleItem _partStyleItem;
        private readonly IAvatarStyleAccessor _styleAccessor;

        private ObservableList<EditorColorCellViewModel> _colorCells = new ObservableList<EditorColorCellViewModel>();
        private ObservableList<EditorSliderCellViewModel> _sliderCells = new ObservableList<EditorSliderCellViewModel>();
        private ObservableList<EditorPresetStyleCellViewModel> _presetStyleCells = new ObservableList<EditorPresetStyleCellViewModel>();
        private string _assetId;
        private bool _presetStylesIsReady;
        private bool _haveColorStyles;
        private bool _haveSliderStyles;
        private bool _havePresentStyles;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _disposed = false;
        private ILoggerFactory _loggerFactory;

        public EditorPresetStylePanelViewModel(
            ILoggerFactory loggerFactory,
            AvatarType avatarType,
            Dictionary<string, EditAvatarDetailWindowViewModel.PresetIcon> presetIconHeader,
            AvatarStyleItem styleItem,
            IAvatarStyleAccessor styleAccessor,
            string assetId)
        {
            _loggerFactory = loggerFactory;
            Logger = _loggerFactory.CreateLogger<EditorPresetStylePanelViewModel>();
            _partStyleItem = styleItem;
            _styleAccessor = styleAccessor;
            AssetId = assetId;
            PresetStylesIsReady = true;
            _cancellationTokenSource = new CancellationTokenSource();
            ColorCells.CollectionChanged += OnColorStylesCollectionChanged;
            SliderCells.CollectionChanged += OnFloatStylesCollectionChanged;
            PresetStyleCells.CollectionChanged += OnPresetStylesCollectionChanged;

            SliderCells.AddRange(CreateFloatItems());
            ColorCells.AddRange(CreateColorItems());
            _presetStyleCells.AddRange(CreatePresetItems(
                avatarType,
                presetIconHeader));
        }

        ~EditorPresetStylePanelViewModel()
        {
            Dispose(true);
        }

        public ObservableList<EditorColorCellViewModel> ColorCells => _colorCells;

        public ObservableList<EditorSliderCellViewModel> SliderCells => _sliderCells;

        public ObservableList<EditorPresetStyleCellViewModel> PresetStyleCells => _presetStyleCells;

        public bool HaveColorStyles
        {
            get => _haveColorStyles;
            set => Set(ref _haveColorStyles, value, nameof(HaveColorStyles));
        }

        public bool HaveSliderStyles
        {
            get => _haveSliderStyles;
            set => Set(ref _haveSliderStyles, value, nameof(HaveSliderStyles));
        }

        public bool HavePresetStyles
        {
            get => _havePresentStyles;
            set => Set(ref _havePresentStyles, value, nameof(HavePresetStyles));
        }

        public string AssetId
        {
            get => _assetId;
            set => Set(ref _assetId, value, nameof(AssetId));
        }

        public bool PresetStylesIsReady
        {
            get => _presetStylesIsReady;
            set => Set(ref _presetStylesIsReady, value, nameof(PresetStylesIsReady));
        }

        private ILogger Logger { get; set; }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (_colorCells != null)
                {
                    _colorCells.DisposeAll();
                    _colorCells = null;
                }

                if (_sliderCells != null)
                {
                    _sliderCells.DisposeAll();
                    _sliderCells = null;
                }

                if (_presetStyleCells != null)
                {
                    _presetStyleCells.DisposeAll();
                    _presetStyleCells = null;
                }

                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource.Dispose();
                }
            }

            _disposed = true;
        }

        private void OnColorStylesCollectionChanged(object sender, NotifyCollectionChangedEventArgs eventArgs)
        {
            HaveColorStyles = ColorCells.Count > 0;
        }

        private void OnFloatStylesCollectionChanged(object sender, NotifyCollectionChangedEventArgs eventArgs)
        {
            HaveSliderStyles = SliderCells.Count > 0;
        }

        private void OnPresetStylesCollectionChanged(object sender, NotifyCollectionChangedEventArgs eventArgs)
        {
            HavePresetStyles = PresetStyleCells.Count > 0;
        }

        private ObservableList<EditorColorCellViewModel> CreateColorItems()
        {
            var items = GetColors();
            if (items == null)
            {
                return new ObservableList<EditorColorCellViewModel>();
            }

            var viewModels = items.Select(x =>
            {
                var (selected, styleId, color) = x;
                var vm = new EditorColorCellViewModel(styleId, selected, color);
                vm.OnSelected += SelectedColor;
                return vm;
            }).ToList();

            return new ObservableList<EditorColorCellViewModel>(viewModels);
        }

        private ObservableList<EditorSliderCellViewModel> CreateFloatItems()
        {
            var items = GetFloats();
            if (items == null)
            {
                return new ObservableList<EditorSliderCellViewModel>();
            }

            var viewModels = items.Select(x =>
            {
                var (styleId, currentValue) = x;
                var item = FloatStyleItemMap[styleId];
                var vm = new EditorSliderCellViewModel(
                    styleId,
                    currentValue,
                    item);
                vm.OnValueChanged += OnSliderValueChanged;
                return vm;
            }).ToList();

            return new ObservableList<EditorSliderCellViewModel>(viewModels);
        }

        private ObservableList<EditorPresetStyleCellViewModel> CreatePresetItems(
            AvatarType avatarType,
            Dictionary<string, EditAvatarDetailWindowViewModel.PresetIcon> presetIconHeader)
        {
            var (isSingleSelect, presetItems) = GetPresets();
            if (presetItems == null)
            {
                return new ObservableList<EditorPresetStyleCellViewModel>();
            }

            var viewModels = presetItems.Select(x =>
            {
                var vm = new EditorPresetStyleCellViewModel(
                    _loggerFactory,
                    avatarType,
                    presetIconHeader,
                    isSingleSelect,
                    x.StyleID,
                    x.Selected,
                    x.AssetId);
                vm.OnSelected += SelectedStyle;
                return vm;
            }).ToList();

            return new ObservableList<EditorPresetStyleCellViewModel>(viewModels);
        }

        private void SelectedColor(EditorColorCellViewModel item, bool isOn)
        {
            if (!isOn)
            {
                return;
            }

            _styleAccessor.Set(item.StyleID, item.Color);
        }

        private void OnSliderValueChanged(string styleID, object slideValue)
        {
            _styleAccessor.Set(styleID, slideValue);
        }

        private void SelectedStyle(EditorPresetStyleCellViewModel item, bool isOn)
        {
            if (isOn)
            {
                _styleAccessor.Set(item.StyleID, item.StyleName);
            }
            else if (!item.IsSingleOption)
            {
                _styleAccessor.Set(item.StyleID, "None");
            }
        }

        private (bool selected, string styleId, Color color)[] GetColors()
        {
            if (!_partStyleItem.IsLeaf)
            {
                var colorItems = _partStyleItem.GetChildItemsOfType(AvatarStyleItemType.Color);

                if (colorItems != null && colorItems.Length > 0)
                {
                    return GetColors(colorItems[0]);
                }
            }
            else if (_partStyleItem.Type == AvatarStyleItemType.Color)
            {
                return GetColors(_partStyleItem);
            }

            return null;
        }

        private (bool selected, string styleId, Color color)[] GetColors(AvatarStyleItem colorItem)
        {
            var styleID = colorItem.name;
            object currentValue = _styleAccessor.Get(styleID);
            Color? selectedColor = null;

            if (currentValue != null)
            {
                selectedColor = (Color)currentValue;
            }

            var colors = colorItem.GetValues();
            if (colors != null)
            {
                var result = new (bool selected, string styleId, Color color)[colors.Length];
                for (int i = 0; i < colors.Length; ++i)
                {
                    var color = (Color)colors[i];
                    var selected = selectedColor.HasValue && selectedColor.Value.Equals(color);
                    result[i] = (selected, styleID, color);
                }

                return result;
            }

            return null;
        }

        private (string styleId, object currentValue)[] GetFloats()
        {
            if (!_partStyleItem.IsLeaf)
            {
                var floatItems = _partStyleItem.GetChildItemsOfType(AvatarStyleItemType.Float);
                return GetFloatItems(floatItems);
            }
            else if (_partStyleItem.Type == AvatarStyleItemType.Float)
            {
                return GetFloatItems(new AvatarStyleItem[] { _partStyleItem });
            }

            return null;
        }

        private (string styleId, object currentValue)[] GetFloatItems(AvatarStyleItem[] avatarStyleItems)
        {
            var result = new (string styleId, object)[avatarStyleItems.Length];
            for (int i = 0; i < avatarStyleItems.Length; ++i)
            {
                var styleId = avatarStyleItems[i].Name;
                object value = _styleAccessor.Get(styleId);
                result[i] = (styleId, value);
            }

            return result;
        }

        private (bool isSingleSelect, List<PresetItem> presetItems) GetPresets()
        {
            var items = new List<AvatarStyleItem>();
            CollectPresetItems(_partStyleItem, ref items);

            if (items.Count == 0)
            {
                return (false, null);
            }

            var singleSelect = items.Count == 1;
            List<PresetItem> styles;
            if (singleSelect)
            {
                var item = items[0];
                var styleID = item.name;
                var currentValue = _styleAccessor.Get(item)?.ToString();
                var values = item.GetValues();
                styles = new List<PresetItem>();
                for (int i = 0; i < values.Length; ++i)
                {
                    var isSelected = currentValue == values[i].ToString();
                    styles.Add(new PresetItem(isSelected, styleID, (string)values[i]));
                }
            }
            else
            {
                styles = new List<PresetItem>(items.Count);
                for (int i = 0; i < items.Count; ++i)
                {
                    var item = items[i];
                    var styleID = item.name;
                    var currentValue = _styleAccessor.Get(styleID)?.ToString();
                    var values = item.GetValues();
                    var isSelected = currentValue == values[1].ToString();
                    styles.Add(new PresetItem(isSelected, styleID, (string)values[1]));
                }
            }

            return (singleSelect, styles);
        }

        private void CollectPresetItems(AvatarStyleItem item, ref List<AvatarStyleItem> items)
        {
            if (item.IsLeaf)
            {
                if (item.Type == AvatarStyleItemType.Preset)
                {
                    items.Add(item);
                }
            }
            else
            {
                var childItems = item.GetChildItemsOfType(AvatarStyleItemType.Preset);

                items.AddRange(childItems);
            }
        }
    }
}