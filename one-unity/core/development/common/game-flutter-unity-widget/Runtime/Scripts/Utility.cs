using MessagePipe;

namespace TPFive.Game.FlutterUnityWidget
{
    public class Utility
    {
        public static void SendGeneralMessage(
            IPublisher<PostUnityMessage> pubPostUnityMessage,
            string message)
        {
            pubPostUnityMessage.Publish(new PostUnityMessage()
            {
                UnityMessage = new Model.UnityMessage()
                {
                    Type = Model.UnityMessageTypeEnum.GeneralStatus,
                    Data = message,
                    SessionId = string.Empty
                },
            });
        }
    }
}
