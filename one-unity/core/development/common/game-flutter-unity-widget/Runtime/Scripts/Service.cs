namespace TPFive.Game.FlutterUnityWidget
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
#if HAS_HIGH_PRIORITY_ENTRY
    using FlutterUnityIntegration;
#endif
    using MessagePipe;
    using TPFive.Model;
    using TPFive.SCG.DisposePattern.Abstractions;
    using TPFive.SCG.ServiceEco.Abstractions;
    using UniRx;
    using VContainer;

    public class PostUnityMessage
    {
        public Model.UnityMessage UnityMessage;
        public Action<FlutterAckData> Ack;
        public ushort timeout = 5;
    }

    public class FlutterAckData
    {
        public bool Success;
        public FlutterMessage FlutterMessage;
    }

    [Dispose]
    [RegisterToContainer]
    public sealed partial class Service : IService
    {
        private readonly CompositeDisposable compositeDisposable = new CompositeDisposable();
        private Dictionary<string, UniTaskCompletionSource<FlutterAckData>> promises = new();
        private IPublisher<FlutterMessage> flutterMessagePubliser;
        private ISubscriber<PostUnityMessage> unityMessageSubcriber;

        [Inject]
        private Service(IPublisher<FlutterMessage> publisher, ISubscriber<PostUnityMessage> subscriber)
        {
            if(GameApp.IsFlutter)
            {
                flutterMessagePubliser = publisher;
                unityMessageSubcriber = subscriber;
#if HAS_HIGH_PRIORITY_ENTRY
                UnityMessageManager.Instance.OnMessage += ReceiveMessageFromFlutter;
#endif
                unityMessageSubcriber.Subscribe(SendMessageToFlutter).AddTo(compositeDisposable);
            }
        }

        private void ReceiveMessageFromFlutter(string jsonString)
        {
            var flutterMessage = FlutterMessage.FromJson(jsonString);
            if (promises.ContainsKey(flutterMessage.SessionId))
            {
                promises[flutterMessage.SessionId].TrySetResult(new FlutterAckData()
                {
                    Success = true,
                    FlutterMessage = flutterMessage,
                });
            }
            else
            {
                flutterMessagePubliser.Publish(flutterMessage);
            }
        }

        private async void SendMessageToFlutter(PostUnityMessage postUnityMessage)
        {
            if (postUnityMessage.Ack == null)
            {
                PostMessage(postUnityMessage.UnityMessage);
            }
            else
            {
                var ack = new FlutterAckData();
                var uuid = Guid.NewGuid().ToString();
                postUnityMessage.UnityMessage.SessionId = uuid;
                promises.Add(uuid, new UniTaskCompletionSource<FlutterAckData>());
                PostMessage(postUnityMessage.UnityMessage);
                try
                {
                    ack = await promises[uuid].Task.Timeout(TimeSpan.FromSeconds(postUnityMessage.timeout));
                }
                catch (Exception e)
                {
                    ack.Success = false;

                    if (e is TimeoutException)
                    {
                        PostMessage (new Model.UnityMessage()
                        {
                            Type = UnityMessageTypeEnum.ShowToast,
                            Data = "Wait for flutter message timeout",
                        });
                    }
                }
                finally
                {
                    promises.Remove(uuid);
                    postUnityMessage.Ack?.Invoke(ack);
                }
            }
        }

        private void PostMessage(Model.UnityMessage unityMessage)
        {
            if (string.IsNullOrEmpty(unityMessage.SessionId))
            {
                unityMessage.SessionId = string.Empty;
            }

            if (string.IsNullOrEmpty(unityMessage.ErrorMsg))
            {
                unityMessage.ErrorMsg = string.Empty;
            }

            if (unityMessage.ErrorCode == null)
            {
                unityMessage.ErrorCode = ErrorCodeEnum.Success;
            }
#if HAS_HIGH_PRIORITY_ENTRY
            UnityMessageManager.Instance.SendMessageToFlutter(unityMessage.ToJson());
#endif
        }

        private void HandleDispose(bool disposing)
        {
            if (disposing)
            {
                compositeDisposable?.Dispose();
                _disposed = true;
            }
        }
    }
}
