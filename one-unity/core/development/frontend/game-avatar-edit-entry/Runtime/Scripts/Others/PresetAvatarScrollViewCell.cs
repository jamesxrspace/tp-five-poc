using DG.Tweening;
using FancyScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace TPFive.Game.AvatarEdit.Entry
{
    internal sealed class PresetAvatarScrollViewCell : FancyCell<PresetAvatarScrollViewCellData, PresetAvatarScrollContext>
    {
        [SerializeField]
        private Animator _animator = default;
        [SerializeField]
        private RawImage _image = default;
        [SerializeField]
        private Button _button = default;
        [SerializeField]
        private GameObject _defaultImage;

        private float _currentPosition = 0;
        private PresetAvatarScrollViewCellData _currentCellData;
        private Tweener _imageTweener;

        private bool IsSelected => Context.SelectedIndex == Index;

        public override void UpdateContent(PresetAvatarScrollViewCellData itemData)
        {
            if (_currentCellData != null)
            {
                _currentCellData.OnLoadTextureCompleted -= OnTextureLoaded;
            }

            _currentCellData = itemData;
            UpdateDisplayTexture();

            if (_currentCellData != null)
            {
                _currentCellData.OnLoadTextureCompleted += OnTextureLoaded;
            }
        }

        public override void UpdatePosition(float position)
        {
            _currentPosition = position;

            if (_animator.isActiveAndEnabled)
            {
                _animator.Play(AnimatorHash.Scroll, -1, position);
            }

            _animator.speed = 0;
        }

        protected void Start()
        {
            _button.onClick.AddListener(() => Context.OnCellClicked?.Invoke(Index));
        }

        protected void OnEnable() => UpdatePosition(_currentPosition);

        protected void OnDestroy()
        {
            _currentCellData = null;

            if (_imageTweener != null && _imageTweener.IsComplete() == false)
            {
                _imageTweener.Complete();
            }
        }

        private void UpdateDisplayTexture()
        {
            _image.texture = _currentCellData == null ? null : IsSelected ? _currentCellData.SelectedTexture : _currentCellData.IdleTexture;
            _image.enabled = _image.texture != null;
            _defaultImage.SetActive(_image.texture == null);
        }

        private void OnTextureLoaded()
        {
            UpdateDisplayTexture();
            if (_image.enabled)
            {
                var color = _image.color;
                _image.color = new Color(color.r, color.g, color.b, 0.35f);
                _imageTweener = _image.DOFade(1f, 0.35f);
                _imageTweener.OnComplete(() => _imageTweener = null);
            }
        }

        private static class AnimatorHash
        {
            public static readonly int Scroll = Animator.StringToHash("scroll");
        }
    }
}
