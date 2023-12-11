using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UnityEngine;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Game.Record
{
    public class SessionManager : IDisposable
    {
        private const float FrameRate = 30f;
        private const int Frequency = 44100;

        private readonly ILogger logger;

        private readonly List<RecordSession> recordSessions;
        private readonly List<PlaySession> playSessions;
        private readonly FilmSession filmSession;

        private CancellationTokenSource playCancellationTokenSource;

        public SessionManager(ILoggerFactory loggerFactory)
        {
            logger = Logging.Utility.CreateLogger<SessionManager>(loggerFactory);

            recordSessions = new List<RecordSession>()
            {
                new MotionRecordSession(logger, FrameRate),
                new AudioRecordSession(logger),
                new SceneRecordSession(logger),
                new DecorationRecordSession(logger),
            };

            playSessions = new List<PlaySession>()
            {
                new MotionPlaySession(logger, FrameRate),
                new AudioPlaySession(logger),
            };

            filmSession = new FilmSession();
        }

        public bool IsPlaying()
        {
            return playSessions.Count != 0 && playSessions.Where(session => session.IsStandBy()).Count() != playSessions.Count;
        }

        public void StartRecord(RecordData[] recordData)
        {
            if (recordData == null || recordData.Length == 0)
            {
                logger.LogWarning("StartRecord fail because without SessionData.");
                return;
            }

            try
            {
                foreach (var session in recordSessions)
                {
                    session.Setup(recordData);
                    session.Start();
                }
            }
            catch (Exception e)
            {
                logger.LogError("StartRecord exception : ", e);
            }
        }

        public void StartPlay(RecordData[] recordData, Action<bool> callback)
        {
            if (recordData == null || recordData.Length == 0)
            {
                logger.LogWarning("Play fail because without SessionData.");
                return;
            }

            try
            {
                logger.LogDebug("StartPlay start");

                playSessions.ForEach(session =>
                {
                    session.Setup(recordData);
                    session.Start();
                });
                var maxDuration = playSessions.Select(x => x.GetDuration()).Max();
                var playDuration = TimeSpan.FromSeconds(maxDuration);

                // Play to end.
                var token = RenewPlayCancellationToken();
                UniTask.Delay(playDuration, cancellationToken: token)
                    .ContinueWith(() =>
                    {
                        StopPlay();

                        callback.Invoke(true);
                    });
            }
            catch (OperationCanceledException)
            {
                logger.LogDebug("StartPlay canceled");
                callback.Invoke(false);
            }
            catch (Exception e)
            {
                logger.LogError($"StartPlay exception : {e}");
                callback.Invoke(false);
            }
        }

        public void StopRecord()
        {
            recordSessions.ForEach(session => session.Stop());
        }

        public void StopPlay()
        {
            playCancellationTokenSource?.Cancel();

            playSessions.ForEach(session => session.Stop());
        }

        public RecordData[] GetRecordData()
        {
            // Append each session data.
            return recordSessions.SelectMany(x => x.GetRecordData()).ToArray();
        }

        public void StartFilm(int videoWidth, int videoHeight, int frameRate, AudioSource audioSource = default, params UnityEngine.Camera[] cameras)
        {
            filmSession.Start(audioSource, frameRate, videoWidth, videoHeight, cameras);
        }

        public UniTask<(string thumbnailPath, string filmPath)> StopFilm()
        {
            return filmSession.Stop();
        }

        public void Dispose()
        {
            playCancellationTokenSource?.Cancel();

            recordSessions.ForEach(session => session?.Dispose());

            playSessions.ForEach(session => session?.Dispose());
        }

        private CancellationToken RenewPlayCancellationToken()
        {
            playCancellationTokenSource?.Cancel();
            playCancellationTokenSource = new CancellationTokenSource();
            return playCancellationTokenSource.Token;
        }
    }
}