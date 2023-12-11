using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TPFive.Game.Mocap;
using TPFive.OpenApi.GameServer;
using TPFive.OpenApi.GameServer.Model;
using UniRx;
using UnityEngine;
using VContainer;
using XR.BodyTracking;
using XRSpace.OpenAPI;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;
using Object = UnityEngine.Object;
using ObservableExtensions = UniRx.ObservableExtensions;

namespace TPFive.Game.Record.Entry
{
    public class MusicToMotionService : IDisposable
    {
        private readonly ILogger log;
        private readonly IAuthTokenProvider authTokenProvider;
        private readonly IServerBaseUriProvider serverBaseUriProvider;
        private readonly MusicToMotionPlayerConfig config;
        private readonly HttpClient httpClient = new ();
        private readonly int timeout = 3;
        private readonly IAigcApi aigcApi;
        private readonly CompositeDisposable compositeDisposable = new CompositeDisposable();

        private XRMusicToMotionService motionPlayer;
        private AIMotionDummyAvatarController controller;
        private HumanPoseHandler humanPoseHandler;
        private HumanPose capturedHumanPose;
        private bool isMotionPlaying;
        private bool disposed;

        [Inject]
        public MusicToMotionService(
            ILoggerFactory loggerFactory,
            IServerBaseUriProvider serverBaseUriProvider,
            IAuthTokenProvider authTokenProvider,
            MusicToMotionPlayerConfig config,
            IAigcApi aigcApi)
        {
            if (loggerFactory is null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            log = loggerFactory.CreateLogger<MusicToMotionService>();
            this.serverBaseUriProvider = serverBaseUriProvider ?? throw new ArgumentNullException(nameof(serverBaseUriProvider));
            this.authTokenProvider = authTokenProvider ?? throw new ArgumentNullException(nameof(authTokenProvider));
            this.config = config;
            this.aigcApi = aigcApi ?? throw new ArgumentNullException(nameof(aigcApi));

            ObservableExtensions.Subscribe(Observable.EveryUpdate(), Tick)
                .AddTo(compositeDisposable);
        }

        ~MusicToMotionService()
        {
            Dispose(false);
        }

        public event Action OnMotionFinish;

        public bool IsReady => motionPlayer != null && motionPlayer.SystemReady;

        public bool IsMotionPlaying => isMotionPlaying;

        public bool HasMotionPlayer => motionPlayer != null;

        public HumanPose CapturedHumanPose => capturedHumanPose;

        private GameObject AvatarModel => controller != null ? controller.AvatarModel : null;

        public async UniTask CreateMotionPlayer()
        {
            if (motionPlayer != null)
            {
                log.LogError("{Method}: MusicToMotionService is already initialized.", nameof(CreateMotionPlayer));
                throw new Exception("MusicToMotionService is already initialized.");
            }

            if (config.MotionPlayerPrefab == null)
            {
                throw new NullReferenceException($"{nameof(config.MotionPlayerPrefab)} is null");
            }

            var serviceGO = Object.Instantiate(config.MotionPlayerPrefab);
            if (!serviceGO.TryGetComponent(out motionPlayer))
            {
                log.LogError("{Method}: XRMusicToMotionService not exists", nameof(CreateMotionPlayer));
                Object.Destroy(serviceGO);
                return;
            }

            var go = Object.Instantiate(config.DummyAvatarPrefab);
            controller = go.GetComponent<AIMotionDummyAvatarController>();
            humanPoseHandler = new HumanPoseHandler(controller.Animator.avatar, controller.AvatarModel.transform);
            ResetBoneRotation(controller.Animator);

            motionPlayer.avatarModel = AvatarModel;
            motionPlayer.OnPrepareSetting += OnPrepareSetting;
            motionPlayer.OnFinished += OnFinished;
            try
            {
                await UniTask.WaitUntil(() => motionPlayer.SystemReady == true)
                    .Timeout(TimeSpan.FromSeconds(timeout));

                AvatarModel.GetComponent<AvatarAnimator>().enabled = false;
            }
            catch (TimeoutException)
            {
                log.LogError("{Method}: Timeout occurred while waiting for service to be ready.", nameof(CreateMotionPlayer));
                throw new Exception("Timeout occurred while waiting for service to be ready.");
            }
        }

        public void DestroyMotionPlayer()
        {
            StopAigcMotion();

            if (motionPlayer != null)
            {
                Object.Destroy(motionPlayer.gameObject);
            }

            if (controller != null)
            {
                Object.Destroy(controller.gameObject);
            }

            if (humanPoseHandler != null)
            {
                humanPoseHandler.Dispose();
                humanPoseHandler = null;
            }
        }

        public async UniTask<byte[][]> GenerateMotion(string url)
        {
            var payload = new GenerateMotionRequest { InputUrl = url };
            var response = await aigcApi.GenerateMotionAsync(payload);

            if (response.IsSuccess)
            {
                try
                {
                    var motionUrl = response.Data;
                    List<UniTask<byte[]>> bufferList = new ()
                    {
                        Download(motionUrl),
                    };
                    return await UniTask.WhenAll(bufferList);
                }
                catch (Exception ex)
                {
                    log.LogError("{Method}: DownloadMotionFile Exception: {ex}", nameof(GenerateMotion), ex);
                    throw;
                }
            }

            log.LogError("{Method}: HTTP Error: {statusCode}, Message {Message}", nameof(GenerateMotion), response.ErrorCode, response.Message);
            throw new HttpRequestException($"GenerateMotion failed. StatusCode: {response.ErrorCode}");
        }

        public void PlayAigcMotion(byte[][] bufferList)
        {
            if (!IsReady)
            {
                log.LogError("{Method}: MusicToMotionService is not ready yet.", nameof(PlayAigcMotion));
                throw new Exception("MusicToMotionService is not ready yet.");
            }

            try
            {
                // Enable the AvatarAnimator before PlayAigcMotion, allowing the motionPlayer to have full control of the avatar.
                var avatarAnimator = AvatarModel.GetComponent<AvatarAnimator>();
                avatarAnimator.enabled = true;
                isMotionPlaying = true;

                foreach (var buffer in bufferList)
                {
                    motionPlayer.UpdateSliceSamplesData(buffer);
                }
            }
            catch (Exception ex)
            {
                log.LogError("{Method}: PlayAigcMotion Exception: {ex}", nameof(PlayAigcMotion), ex);
                throw;
            }
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                compositeDisposable.Dispose();

                if (motionPlayer != null)
                {
                    Object.Destroy(motionPlayer.gameObject);
                }

                if (controller != null)
                {
                    Object.Destroy(controller.gameObject);
                }

                humanPoseHandler?.Dispose();
            }

            disposed = true;
        }

        private void StopAigcMotion()
        {
            if (motionPlayer != null)
            {
                motionPlayer.ServiceReset();
            }

            if (AvatarModel != null &&
                AvatarModel.TryGetComponent<AvatarAnimator>(out var avatarAnimator))
            {
                avatarAnimator.enabled = false;
            }

            isMotionPlaying = false;
        }

        private void Tick(long tick)
        {
            if (motionPlayer == null ||
                !IsReady ||
                !isMotionPlaying)
            {
                return;
            }

            humanPoseHandler.GetHumanPose(ref capturedHumanPose);
        }

        private void ResetBoneRotation(Animator animator)
        {
            // Avatar need to be reset to T pose before initiate the MusicToMotionService
            foreach (HumanBodyBones bone in Enum.GetValues(typeof(HumanBodyBones)))
            {
                if (bone == HumanBodyBones.LastBone)
                {
                    continue;
                }

                Transform boneTransform = animator.GetBoneTransform(bone);
                if (boneTransform != null)
                {
                    boneTransform.localRotation = Quaternion.identity;
                }
            }
        }

        // Currently, the OpenAPI code generator does not support stream responses. Temporarily send the request using httpClient.
        private async UniTask<HttpResponseMessage> RequestMotion(string fileUrl)
        {
            var url = $"{serverBaseUriProvider.BaseUri}api/v1/aigc/motion/generate";
            var token = authTokenProvider.GetAuthToken();

            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("Authorization", $"Bearer {token}");
            var requestJsonPayload = JsonConvert.SerializeObject(new AigcMotionRequest { InputUrl = fileUrl });
            request.Content = new StringContent(requestJsonPayload, Encoding.UTF8, "application/json");

            return await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        }

        private async UniTask<byte[]> Download(string url)
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    log.LogError("{Method}: HTTP Error: {statusCode}", nameof(Download), response.StatusCode);
                    throw new HttpRequestException($"Download failed. StatusCode: {response.StatusCode}");
                }

                return await response.Content.ReadAsByteArrayAsync();
            }
            catch (Exception ex)
            {
                log.LogError("{Method}: DownloadMotionFile Exception: {ex}", nameof(Download), ex);
                throw;
            }
        }

        private void OnPrepareSetting(bool ready)
        {
            if (ready)
            {
                BodyTrackingSettings.MoveRange = config.MoveRange;
            }

            BodyTrackingSettings.MusicToMotionStreamingAssetsPath =
                config.MusicToMotionStreamingAssetsPath;
        }

        private void OnFinished()
        {
            motionPlayer.ServiceReset();
            AvatarModel.GetComponent<AvatarAnimator>().enabled = false;
            isMotionPlaying = false;
            OnMotionFinish?.Invoke();
        }
    }
}
