using Loxodon.Framework.ViewModels;

namespace TPFive.Game.Decoration
{
    public class DecorationItemViewModel : ViewModelBase
    {
        private string _bundleID;
        private bool _isVisible;
        private bool _disposed;

        public DecorationItemViewModel(string bundleID)
        {
            BundleID = bundleID;
        }

        ~DecorationItemViewModel()
        {
            Dispose(false);
        }

        public string BundleID
        {
            get => _bundleID;
            set => Set(ref _bundleID, value, nameof(BundleID));
        }

        public bool IsVisible
        {
            get => _isVisible;
            set => Set(ref _isVisible, value, nameof(IsVisible));
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            base.Dispose(disposing);
            _disposed = true;
        }
    }
}
