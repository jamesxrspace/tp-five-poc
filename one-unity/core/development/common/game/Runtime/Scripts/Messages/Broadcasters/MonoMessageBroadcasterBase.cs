using System;
using MessagePipe;
using UnityEngine;
using UnityEngine.Events;

namespace TPFive.Game.Messages.Broadcasters
{
    /// <summary>
    /// A base class for broadcasting 'MessagePipe' message to 'UnityEvent'.
    /// </summary>
    /// <typeparam name="TMessage">'MessagePipe' message.</typeparam>
    public class MonoMessageBroadcasterBase<TMessage> : MonoBehaviour
    {
        [SerializeField]
        private MessageEvent onReceived = new MessageEvent();

        private MessageHandlerFilter<TMessage>[] filtersCache = Array.Empty<MessageHandlerFilter<TMessage>>();
        private IDisposable subscription;

        public MessageEvent OnReceived => onReceived;

        public void SetFilters(params MessageHandlerFilter<TMessage>[] filters)
        {
            filtersCache = filters;

            if (!this.enabled)
            {
                return;
            }

            // Re-subscribe
            Unsubscribe();
            Subscribe(filtersCache);
        }

        protected virtual void OnEnable()
        {
            Subscribe(filtersCache);
        }

        protected virtual void OnDisable()
        {
            Unsubscribe();
        }

        private void Subscribe(params MessageHandlerFilter<TMessage>[] filters)
        {
            var msgSubscriber = GlobalMessagePipe.GetSubscriber<TMessage>();
            if (msgSubscriber == null)
            {
                Debug.LogError($"[{nameof(MonoMessageBroadcasterBase<TMessage>)}] OnEnable(): No subscriber found for {typeof(TMessage)}");
                return;
            }

            subscription = msgSubscriber.Subscribe(OnReceivedMessageCallback, filters);
        }

        private void Unsubscribe()
        {
            subscription?.Dispose();
        }

        private void OnReceivedMessageCallback(TMessage message)
        {
            onReceived.Invoke(message);
        }

        [Serializable]
        public sealed class MessageEvent : UnityEvent<TMessage>
        {
        }
    }
}
