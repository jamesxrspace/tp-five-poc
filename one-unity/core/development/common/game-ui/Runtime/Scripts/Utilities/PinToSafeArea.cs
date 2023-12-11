using UnityEngine;
using UnityEngine.Events;

namespace TPFive.UI
{
    /// <summary>
    /// Safe area implementation for notched mobile devices. Usage:
    ///  (1) Add this component to the top level of any GUI panel.
    ///  (2) If the panel uses a full screen background image, then create an immediate child and put the component on that instead, with all other elements childed below it.
    ///      This will allow the background image to stretch to the full extents of the screen behind the notch, which looks nicer.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public sealed class PinToSafeArea : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent onSafeAreaChanged = new UnityEvent();

        private RectTransform panel;
        private Rect lastSafeArea = Rect.zero;
        private Vector2Int lastScreenSize = Vector2Int.zero;
        private ScreenOrientation lastOrientation = ScreenOrientation.AutoRotation;

        public UnityEvent OnSafeAreaChanged => onSafeAreaChanged;

        private void Awake()
        {
            if (!TryGetComponent(out panel))
            {
                Debug.LogError($"Cannot apply safe area - no RectTransform found on {name}");
                Destroy(this);
            }

            Refresh();
        }

        private void Update()
        {
            Refresh();
        }

        private void Refresh()
        {
            Rect safeArea = Screen.safeArea;

            if (safeArea != lastSafeArea
                || Screen.width != lastScreenSize.x
                || Screen.height != lastScreenSize.y
                || Screen.orientation != lastOrientation)
            {
                // Fix for having auto-rotate off and manually forcing a screen orientation.
                // See https://forum.unity.com/threads/569236/#post-4473253 and https://forum.unity.com/threads/569236/page-2#post-5166467
                lastScreenSize.x = Screen.width;
                lastScreenSize.y = Screen.height;
                lastOrientation = Screen.orientation;

                ApplySafeArea(safeArea);
            }
        }

        private void ApplySafeArea(Rect safeAreaRect)
        {
            lastSafeArea = safeAreaRect;

            // Check for invalid screen startup state on some Samsung devices (see below)
            if (Screen.width > 0 && Screen.height > 0)
            {
                // Convert safe area rectangle from absolute pixels to normalised anchor coordinates
                Vector2 anchorMin = safeAreaRect.position;
                Vector2 anchorMax = safeAreaRect.position + safeAreaRect.size;
                anchorMin.x /= Screen.width;
                anchorMin.y /= Screen.height;
                anchorMax.x /= Screen.width;
                anchorMax.y /= Screen.height;

                // Fix for some Samsung devices (e.g. Note 10+, A71, S20) where Refresh gets called twice and the first time returns NaN anchor coordinates
                // See https://forum.unity.com/threads/569236/page-2#post-6199352
                if (anchorMin.x >= 0 && anchorMin.y >= 0 && anchorMax.x >= 0 && anchorMax.y >= 0)
                {
                    panel.anchorMin = anchorMin;
                    panel.anchorMax = anchorMax;
                }

                onSafeAreaChanged?.Invoke();
            }
        }
    }
}