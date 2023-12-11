using System;
using Microsoft.Extensions.Logging;
using TPFive.Game.Avatar;
using TPFive.Game.Camera;
using TPFive.Game.Record.Scene;
using TPFive.Game.Utils;
using UnityEngine;
using ICameraService = TPFive.Game.Camera.IService;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Game.Record.Entry
{
    public sealed class ReelCameraController
    {
        private readonly ILogger log;
        private readonly ICameraService cameraService;

        public ReelCameraController(ILoggerFactory loggerFactory, ICameraService cameraService)
        {
            this.log = loggerFactory.CreateLogger<ReelCameraController>();
            this.cameraService = cameraService;
        }

        public void TurnOnCamera(ReelCameraSetting cameraSetting, GameObject followTargetAvatar)
        {
            Transform followTarget = null;
            Transform lookAtTarget = null;
            if (followTargetAvatar != null)
            {
                followTarget = GetCameraTargetFromPlayer(followTargetAvatar);
                lookAtTarget = followTargetAvatar.transform;
            }

            TurnOnCamera(cameraSetting, followTarget, followTarget, lookAtTarget);
        }

        public void TurnOnCamera(
            ReelCameraSetting cameraSetting,
            Transform wantFaceToTarget = null,
            Transform followTarget = null,
            Transform lookAtTarget = null)
        {
            if (cameraSetting == null ||
                cameraSetting.CameraObject == null)
            {
                log.LogError(
                    "{Method}: cameraSetting or cameraSetting.CameraObject is null.",
                    nameof(TurnOnCamera));
                return;
            }

            string cameraName = cameraSetting.CameraObject.name;

            if (!cameraService.TryGetCamera(cameraName, out var camera) ||
                !camera.IsAlive())
            {
                throw new CameraException($"The camera '{cameraName}' isn't registered or not alive.");
            }

            ProcessCameraGotoLiveFacingMode(camera, cameraSetting, wantFaceToTarget);

            camera.AllowInteractWithMoveInput = cameraSetting.AllowInteractWithMoveInput;
            camera.AllowInteractWithRotateInput = cameraSetting.AllowInteractWithRotateInput;
            camera.State.Value = CameraState.Live;

            followTarget = cameraSetting.AllowInteractWithMoveInput ? followTarget : null;
            lookAtTarget = cameraSetting.AllowInteractWithRotateInput ? lookAtTarget : null;
            SetupCameraTarget(followTarget, lookAtTarget);

            log.LogInformation("{Method}: set {CameraName} goto live", nameof(TurnOnCamera), cameraName);
        }

        public void TurnOffCamera()
        {
            var camera = cameraService.GetLiveCamera();
            if (!camera.IsAlive())
            {
                log.LogWarning("{Method}: No camera is alive", nameof(TurnOffCamera));
                return;
            }

            camera.State.Value = CameraState.Standby;

            log.LogInformation("{Method}: set {CameraName} goto standby", nameof(TurnOffCamera), camera.Name);
        }

        private void SetupCameraTarget(Transform followTarget, Transform lookAtTarget)
        {
            var liveCamera = cameraService.GetLiveCamera();

            SetupCameraTarget(liveCamera, followTarget, lookAtTarget);
        }

        private void SetupCameraTarget(ICamera camera, Transform followTarget, Transform lookAtTarget)
        {
            if (camera == null)
            {
                log.LogWarning("{Method}: camera is null", nameof(SetupCameraTarget));
                return;
            }

            if (camera is ICameraFollowTarget cameraFollowTarget)
            {
                cameraFollowTarget.FollowTarget.Value = followTarget;
            }

            if (camera is ICameraLookAtTarget cameraLookAtTarget)
            {
                cameraLookAtTarget.LookAtTarget.Value = lookAtTarget;
            }

            log.LogDebug("{Method}: setup camera target", nameof(SetupCameraTarget));
        }

        private Transform GetCameraTargetFromPlayer(GameObject player)
        {
            if (player == null)
            {
                throw new ArgumentNullException(nameof(player), "player is null");
            }

            var cameraTarget = player.transform.Find("Avatar/Camera Target");
            if (!cameraTarget)
            {
                throw new CameraException("avatar does not have camera target");
            }

            return cameraTarget;
        }

        private void ProcessCameraGotoLiveFacingMode(
            ICamera camera,
            ReelCameraSetting cameraSetting,
            Transform wantFaceToTarget)
        {
            if (!camera.IsAlive())
            {
                throw new CameraException("The camera isn't alive.");
            }

            if (cameraSetting == null)
            {
                log.LogError(
                    "{Method}: cameraSetting is null",
                    nameof(TurnOnCamera));
                return;
            }

            log.LogDebug(
                "{Method}: process camera goto live facing mode({FacingMode})",
                nameof(ProcessCameraGotoLiveFacingMode),
                cameraSetting.GotoLiveFacingMode);

            switch (cameraSetting.GotoLiveFacingMode)
            {
                case ReelCameraGotoLiveFacingMode.NoneOrAuto:
                    break;

                case ReelCameraGotoLiveFacingMode.FaceToTarget:
                    {
                        Vector3 targetPosition = Vector3.zero;
                        Vector3 targetForward = Vector3.forward;
                        if (wantFaceToTarget != null)
                        {
                            targetPosition = wantFaceToTarget.position;
                            targetForward = wantFaceToTarget.forward;
                        }

                        Vector3 newPosition = targetPosition + (targetForward * cameraSetting.DistanceBetweenTarget);
                        Vector3 lookDirection = targetPosition - newPosition;
                        Quaternion newRotation = Quaternion.LookRotation(lookDirection);
                        camera.Pose = new Pose(newPosition, newRotation);
                    }

                    break;

                case ReelCameraGotoLiveFacingMode.AlignPreviousCameraPlace:
                    {
                        var mainCamera = CameraCache.Main.transform;
                        camera.Pose = new Pose(mainCamera.position, mainCamera.rotation);
                    }

                    break;

                default:
                    throw new NotSupportedException($"Unknown facing mode({cameraSetting.GotoLiveFacingMode})");
            }
        }
    }
}
