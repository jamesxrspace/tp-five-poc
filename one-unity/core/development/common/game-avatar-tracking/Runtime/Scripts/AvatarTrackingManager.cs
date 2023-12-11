using System;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TPFive.Game.Mocap;
using XR.BodyTracking;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using IMocapService = TPFive.Game.Mocap.IService;

namespace TPFive.Game.Avatar.Tracking
{
    public sealed class AvatarTrackingManager : IAvatarTrackingManager
    {
        private readonly AvatarTrackingSettings settings;
        private readonly IMocapService mocapService;
        private readonly ILogger logger;
        private readonly BodyTrackingMotion bodyMotion;
        private readonly FaceTrackingMotion faceMotion;

        private bool isTracking;
        private bool isBusy;
        private bool disposed;
        private CaptureOptions options = default;

        public AvatarTrackingManager(
            BodyTrackingMotion bodyMotion,
            FaceTrackingMotion faceMotion,
            AvatarTrackingSettings settings,
            IMocapService mocapService,
            ILoggerFactory loggerFactory)
        {
            this.bodyMotion = bodyMotion ?? throw new ArgumentNullException(nameof(bodyMotion));
            this.faceMotion = faceMotion ?? throw new ArgumentNullException(nameof(faceMotion));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.mocapService = mocapService ?? throw new ArgumentNullException(nameof(mocapService));
            this.logger = loggerFactory.CreateLogger<AvatarTrackingManager>();
            this.mocapService.OnTrackingStatusChanged += OnTrackingStatusChanged;
        }

        ~AvatarTrackingManager()
        {
            Dispose(disposing: false);
        }

        public event Action OnFaceTrackingStarted;

        public event Action OnBodyTrackingStarted;

        public event Action<bool> OnLossTrackingChanged;

        public bool IsTracking => isTracking;

        private bool ServiceEnabled => mocapService.IsMocapEnabled;

        private float FadeDuration => settings.LayerFadeDuration;

        public async UniTask StartTracking(CaptureOptions options)
        {
            if (options == CaptureOptions.None)
            {
                throw new ArgumentException("CaptureOptions is None", nameof(options));
            }

            if (isBusy)
            {
                logger.LogWarning("Tracking is starting");
                return;
            }

            try
            {
                isBusy = true;
                if (isTracking || ServiceEnabled)
                {
                    await Stop();

                    // Waiting for mocap service to destroy the object
                    await UniTask.DelayFrame(1);
                }

                this.options = options;
                await Start();
                isTracking = true;
            }
            finally
            {
                isBusy = false;
            }
        }

        public async UniTask StopTracking()
        {
            if (isBusy)
            {
                logger.LogWarning("Tracking is starting");
                return;
            }

            try
            {
                isBusy = true;
                await Stop();

                isTracking = false;
                options = CaptureOptions.None;
            }
            finally
            {
                isBusy = false;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
        private async UniTask Start()
        {
            logger.LogDebug(
                "{Method}: Begin. {OptionsName}={OptionsValue}",
                nameof(Start),
                nameof(options),
                options);

            if (mocapService.IsMocapEnabled)
            {
                mocapService.DisableMocap();
            }

            var utcs = new UniTaskCompletionSource();
            void OnMocapEnabled(CaptureOptions _) => utcs.TrySetResult();
            mocapService.OnMocapEnabled += OnMocapEnabled;
            mocapService.OnBodyTrackingStarted += OnBodyTrackingStarted;
            mocapService.OnFaceTrackingStarted += OnFaceTrackingStarted;
            mocapService.EnableMocap(options);

            try
            {
                await utcs.Task.Timeout(TimeSpan.FromSeconds(300));
            }
            catch (TimeoutException)
            {
                logger.LogWarning($"{nameof(Start)} timeout");
                return;
            }
            finally
            {
                mocapService.OnMocapEnabled -= OnMocapEnabled;
            }

            // set avatar mask
            var mask = options.IsEnableUpperBody ? settings.UpperBodyMask : null;
            bodyMotion.SetMask(mask);

            // fade in motions
            RefreshTracking();

            logger.LogDebug("{Method}: End.", nameof(Start));
        }
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter

        private async UniTask Stop()
        {
            logger.LogDebug("{Method}: Begin", nameof(Stop));

            // fade out motions
            LossTracking();

            // Wait for motion to fade out
            await UniTask.Delay(TimeSpan.FromSeconds(FadeDuration));

            mocapService.DisableMocap();

            mocapService.OnBodyTrackingStarted -= OnBodyTrackingStarted;
            mocapService.OnFaceTrackingStarted -= OnFaceTrackingStarted;

            logger.LogDebug("{Method}: End", nameof(Stop));
        }

        private void OnTrackingStatusChanged(BodyPart bodyPart, TrackingStatus status)
        {
            if (!isTracking || !ServiceEnabled
                || bodyPart != BodyPart.Body)
            {
                return;
            }

            switch (status)
            {
                case TrackingStatus.Loss:
                    LossTracking();
                    OnLossTrackingChanged?.Invoke(true);
                    break;
                case TrackingStatus.Tracking:
                    RefreshTracking();
                    OnLossTrackingChanged?.Invoke(false);
                    break;
            }
        }

        private void LossTracking()
        {
            bodyMotion.IsEnable = false;
            faceMotion.IsEnable = false;
        }

        private void RefreshTracking()
        {
            bodyMotion.IsEnable = options.IsEnableBody;
            faceMotion.IsEnable = options.IsEnableFace;
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                bodyMotion?.Dispose();
                faceMotion?.Dispose();

                if (mocapService == null)
                {
                    return;
                }

                mocapService.OnTrackingStatusChanged -= OnTrackingStatusChanged;
                mocapService.OnBodyTrackingStarted -= OnBodyTrackingStarted;
                mocapService.OnFaceTrackingStarted -= OnFaceTrackingStarted;

                if (mocapService.IsMocapEnabled)
                {
                    mocapService.DisableMocap();
                }
            }

            disposed = true;
        }
    }
}