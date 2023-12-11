using System;
using Fusion;
using MessagePipe;
using TPFive.Game.FlutterUnityWidget;
using TPFive.Game.Messages;
using TPFive.Model;
using UniRx;
using VContainer;
using VContainer.Unity;

namespace TPFive.Room
{
    public class GeneralRoomManager<TRoom> : IRoomManager, IStartable, IDisposable
        where TRoom : IRoom, new()
    {
        private readonly CompositeDisposable compositeDisposable = new CompositeDisposable();

        [Inject]
        private IOpenRoomCmd openRoomCmd;
        [Inject]
        private IObjectResolver objectResolver;
        [Inject]
        private ISubscriber<FlutterMessage> subFlutterMessage;
        [Inject]
        private IPublisher<PostUnityMessage> pubUnityMessage;
        [Inject]
        private IPublisher<BackToHome> pubBackToHome;

        public GameMode Mode => GameMode.AutoHostOrClient;

        public IRoom Room { get; private set; }

        void IStartable.Start()
        {
            if (Room != null)
            {
                return;
            }

            // Fetch content of the room-opening command
            if (!openRoomCmd.Fetch(out var cmdContent))
            {
                throw new Exception("Failed fetching room-opening command");
            }

            // Create and set up the room
            Room = new TRoom();
            Room.OnStarted += OnRoomStarted;
            objectResolver.Inject(Room);

            // Open the room
            Room.Open(Mode, cmdContent.SpaceID, cmdContent.RoomID, cmdContent.SceneKey, cmdContent.Region);

            // subscribe flutter back to home event
            subFlutterMessage
                .Subscribe(OnFlutterRequestToLobby, arg => arg.Type == FlutterMessageTypeEnum.RequestToSocialLobbyPage)
                .AddTo(compositeDisposable);
        }

        void IDisposable.Dispose()
        {
            Room?.Dispose();
            Room = null;

            compositeDisposable?.Dispose();
        }

        private void OnRoomStarted()
        {
            pubUnityMessage.Publish(new PostUnityMessage()
            {
                UnityMessage = new Model.UnityMessage()
                {
                    Type = Model.UnityMessageTypeEnum.SwitchedToSpace,
                    Data = string.Empty,
                    SessionId = string.Empty,
                },
            });
        }

        private void OnFlutterRequestToLobby(FlutterMessage flutterMessage)
        {
            pubBackToHome.Publish(default(BackToHome));
        }
    }
}
