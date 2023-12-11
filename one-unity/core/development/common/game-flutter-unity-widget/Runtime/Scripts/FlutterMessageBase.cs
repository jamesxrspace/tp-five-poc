using System;
using MessagePipe;
using Microsoft.Extensions.Logging;
using TPFive.Model;
using UniRx;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Game.FlutterUnityWidget
{
    public abstract class FlutterMessageBase : IDisposable
    {
        private const UnityMessageTypeEnum ackReturnType = UnityMessageTypeEnum.Unity;
        private readonly CompositeDisposable compositeDisposable = new CompositeDisposable();
        private readonly ILogger log;
        private readonly ISubscriber<FlutterMessage> subFlutterMessage;
        private readonly IPublisher<PostUnityMessage> pubUnityMessage;

        public FlutterMessageBase(
            ILogger log,
            ISubscriber<FlutterMessage> subFlutterMessage,
            IPublisher<PostUnityMessage> pubUnityMessage)
        {
            this.log = log;
            this.subFlutterMessage = subFlutterMessage;
            this.pubUnityMessage = pubUnityMessage;
            subFlutterMessage.Subscribe(OnFlutterMessage).AddTo(compositeDisposable);
        }

        protected ILogger Log { get => log; }

        public void Dispose()
        {
            compositeDisposable?.Dispose();
        }

        protected abstract void OnFlutterMessage(FlutterMessage flutterMessage);

        protected void SendUnityMessage(
            string data = "",
            string sessionId = null,
            UnityMessageTypeEnum type = ackReturnType,
            Action<FlutterAckData> ack = null,
            ushort timeout = 5,
            ErrorCodeEnum errorCode = ErrorCodeEnum.Success,
            string errorMsg = "")
        {
            pubUnityMessage.Publish(new PostUnityMessage()
            {
                UnityMessage = new UnityMessage()
                {
                    Type = type,
                    Data = data,
                    SessionId = string.IsNullOrEmpty(sessionId) ? string.Empty : sessionId,
                    ErrorCode = errorCode,
                    ErrorMsg = errorMsg
                },
                Ack = ack,
                timeout = timeout
            });
            log.LogDebug("Send message to flutter, SessionId : {sessionId} , Data : {data}", sessionId, data);
        }
    }
}
