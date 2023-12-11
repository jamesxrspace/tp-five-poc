using UnityEngine;

namespace TPFive.Game.Record
{
    using System;
    using System.IO;
    using Cysharp.Threading.Tasks;
    using Stateless;
    using TPFive.Game.Extensions;

    public partial class FilmSession : IDisposable
    {
        private const int ThumbnailWidth = 160;
        private MP4VideoMuxer muxer;
        private StateMachine<State, Event> machine;
        private string thumbnailPath;

        public FilmSession()
        {
            InitStateMachine();
        }

        private enum Event
        {
            /// <summary>
            /// The state where the game play is being Film.
            /// </summary>
            Film,

            /// <summary>
            /// The state where the Film session has been stopped.
            /// </summary>
            Stop,
        }

        private enum State
        {
            /// <summary>
            /// The idle state of the Film state machine.
            /// This state is entered when the Film session is not actively recording or Filming back a session.
            /// </summary>
            Idle,

            /// <summary>
            /// The Filming state of the Film state machine.
            /// </summary>
            Filming,
        }

        public void InitStateMachine()
        {
            machine = new StateMachine<State, Event>(State.Idle);
            machine.Configure(State.Idle)
                .Permit(Event.Film, State.Filming);
            machine.Configure(State.Filming)
                .Permit(Event.Stop, State.Idle);
        }

        public void Start(AudioSource audioSource = default, int frameRate = 30, int videoWidth = 1080, int videoHeight = 1920, params UnityEngine.Camera[] cameras)
        {
            machine.Fire(Event.Film);
            thumbnailPath = ScreenCapture(cameras[0]);
            muxer = new MP4VideoMuxer(videoWidth, videoHeight, frameRate);
            muxer.StartRecord(true, audioSource, cameras);
        }

        public async UniTask<(string, string)> Stop()
        {
            machine.Fire(Event.Stop);
            muxer.StopRecord();
            string muxedPath = await muxer.Export();
            muxer.Dispose();
            var filmPath = FileStorageUtility.MoveFileDirectory(muxedPath, FileStorageUtility.BaseDirectory);
            return (thumbnailPath, filmPath);
        }

        public void Dispose()
        {
            muxer?.Dispose();
        }

        private string ScreenCapture(Camera cam)
        {
            Texture2D renderedTexture = ScreenCaptureExtensions.CaptureScreenshotAsTexture(ThumbnailWidth, ThumbnailWidth, false, cam);
            byte[] byteArray = renderedTexture.EncodeToPNG();

            var filePath = Path.Combine(FileStorageUtility.BaseDirectory, FileStorageUtility.GetUniqueFileName(FileExtensionType.Png, "thumbnail"));
            File.WriteAllBytes(filePath, byteArray);
            return filePath;
        }
    }
}
