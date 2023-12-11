using Fusion;
using Microsoft.Extensions.Logging;
using TPFive.Game.Camera;
using UnityEngine;
using VContainer;
using ICameraService = TPFive.Game.Camera.IService;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Room
{
    public class CameraTargetHandler : NetworkBehaviour
    {
        [SerializeField]
        private Transform cameraTarget;

        private ILogger log;
        private ICameraService cameraService;

        [Inject]
        public void Construct(ILoggerFactory loggerFactory, ICameraService cameraService)
        {
            this.log = loggerFactory.CreateLogger<CameraTargetHandler>();
            this.cameraService = cameraService;
        }

        public override void Spawned()
        {
            SetupCamera();
        }

        private void SetupCamera()
        {
            if (!HasInputAuthority)
            {
                return;
            }

            var liveCamera = cameraService.GetLiveCamera();
            if (liveCamera == null)
            {
                log.LogError(
                    "{Method}: The live camera is null",
                    nameof(SetupCamera));

                return;
            }

            if (liveCamera is ICameraFollowTarget cameraFollowTarget)
            {
                cameraFollowTarget.FollowTarget.Value = cameraTarget;
            }

            if (liveCamera is ICameraLookAtTarget cameraLookAtTarget)
            {
                cameraLookAtTarget.LookAtTarget.Value = cameraTarget;
            }
        }
    }
}
