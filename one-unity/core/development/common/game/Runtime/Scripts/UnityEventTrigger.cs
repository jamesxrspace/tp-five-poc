using UnityEngine;

namespace TPFive.Game
{
    internal sealed class UnityEventTrigger : MonoBehaviour
    {
        private void Awake()
        {
            var objects = FindObjectsByType<UnityEventTrigger>(FindObjectsSortMode.None);
            if (objects.Length == 1)
            {
                return;
            }

            Debug.LogWarning($"More than one instance of \"{nameof(UnityEventTrigger)}\".");
            for (var i = 0; i < objects.Length; ++i)
            {
                if (objects[i] == this)
                {
                    continue;
                }

                // Only the Component will be destoryed, but Component's GameObject will not.
                // Because we don't know what the GameObject is used for.
                Destroy(objects[i]);
            }
        }

        private void OnApplicationFocus(bool focus)
        {
            GameApp.RaiseOnApplicationFocusEvent(focus);
        }

        private void OnApplicationPause(bool pause)
        {
            GameApp.RaiseOnApplicationPauseEvent(pause);
        }

        private void OnApplicationQuit()
        {
            GameApp.RaiseOnApplicationQuitEvent();
        }
    }
}
