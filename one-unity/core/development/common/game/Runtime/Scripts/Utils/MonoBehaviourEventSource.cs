using UnityEngine;
using UnityEngine.Events;

namespace TPFive.Game
{
    public sealed class MonoBehaviourEventSource : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent<Event> onEvent;

        public enum Event
        {
            OnStart,
            OnEnable,
            OnDisable,
            OnDestroy,
        }

        public UnityEvent<Event> OnEvent => onEvent;

        private void Start()
        {
            onEvent?.Invoke(Event.OnStart);
        }

        private void OnEnable()
        {
            onEvent?.Invoke(Event.OnEnable);
        }

        private void OnDisable()
        {
            onEvent?.Invoke(Event.OnDisable);
        }

        private void OnDestroy()
        {
            onEvent?.Invoke(Event.OnDestroy);
        }
    }
}