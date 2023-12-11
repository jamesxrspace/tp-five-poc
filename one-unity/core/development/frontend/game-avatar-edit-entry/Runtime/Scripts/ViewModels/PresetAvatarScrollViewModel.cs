using Loxodon.Framework.Observables;
using Loxodon.Framework.ViewModels;

namespace TPFive.Game.AvatarEdit.Entry
{
    internal sealed class PresetAvatarScrollViewModel : ViewModelBase
    {
        private readonly ObservableList<PresetAvatarScrollViewCellData> _items = new ();

        private int _currentIndex = 0;
        private bool _disposed = false;

        ~PresetAvatarScrollViewModel()
        {
            Dispose(false);
        }

        public int CurrentIndex
        {
            get => _currentIndex;
            set => Set(ref _currentIndex, value, nameof(CurrentIndex));
        }

        public ObservableList<PresetAvatarScrollViewCellData> Items => _items;

        public void AddItem(PresetAvatarScrollViewCellData item)
        {
            if (item == null)
            {
                return;
            }

            _items.Add(item);
        }

        public void AddItems(PresetAvatarScrollViewCellData[] items)
        {
            if (items == null)
            {
                return;
            }

            _items.AddRange(items);
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
                foreach (var item in _items)
                {
                    item?.Dispose();
                }

                _items.Clear();
            }

            _disposed = true;
        }
    }
}
