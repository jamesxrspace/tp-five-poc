using UniRx;
using UnityEngine;

namespace TPFive.Extended.InputDeviceProvider.OnScreen
{
    public class OnScreenStickController : MonoBehaviour, IOnScreenStickController
    {
        [SerializeField]
        [Tooltip("The interact area of the stick controller.")]
        private RectTransform interactArea;

        private CompositeDisposable disposables = new CompositeDisposable();

        public IReactiveProperty<bool> IsActive { get; private set; }

        public void SetupInteractArea(Vector2 anchorMin, Vector2 anchorMax)
        {
            if (interactArea == null)
            {
                throw new System.NullReferenceException("Interact area isn't set.");
            }

            interactArea.anchorMin = anchorMin;
            interactArea.anchorMax = anchorMax;
        }

        protected void Awake()
        {
            IsActive = new BoolReactiveProperty(false);
            IsActive.Subscribe(ToggleControllerActive)
                .AddTo(disposables);
        }

        protected void OnDestroy()
        {
            disposables.Dispose();
            disposables = null;
        }

        private void ToggleControllerActive(bool isActive)
        {
            if (gameObject.activeSelf == isActive)
            {
                return;
            }

            gameObject.SetActive(isActive);
        }
    }
}