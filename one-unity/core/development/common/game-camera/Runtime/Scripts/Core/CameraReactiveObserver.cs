using UniRx;

namespace TPFive.Game.Camera
{
    public delegate void CameraReactiveValueChangedDelegate<TValue>(ICamera camera, TValue value);

    public class CameraReactiveObserver<TValue> : System.IDisposable
    {
        private readonly ICamera camera;
        private readonly CameraReactiveValueChangedDelegate<TValue> valueChangedCallback;
        private readonly System.IDisposable subscription;

        public CameraReactiveObserver(
            ICamera camera,
            IReadOnlyReactiveProperty<TValue> reactiveProperty,
            CameraReactiveValueChangedDelegate<TValue> valueChangedCallback)
        {
            this.camera = camera;
            this.valueChangedCallback = valueChangedCallback;
            this.subscription = reactiveProperty.Subscribe(OnValueChanged);
        }

        ~CameraReactiveObserver()
        {
            OnDispose(false);
        }

        protected bool IsDisposed { get; private set; }

        public void Dispose()
        {
            OnDispose(true);
            System.GC.SuppressFinalize(this);
        }

        protected virtual void OnDispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (disposing)
            {
                subscription?.Dispose();
            }

            IsDisposed = true;
        }

        private void OnValueChanged(TValue value)
        {
            valueChangedCallback.Invoke(camera, value);
        }
    }
}
