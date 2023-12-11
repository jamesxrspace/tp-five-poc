using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Microsoft.Extensions.Logging;
using TPFive.Game.Record.Scene;
using TPFive.Game.Reel.Camera;
using TPFive.Game.Utils;
using UnityEngine;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace TPFive.Game.Record.Entry
{
    public class ReelDirector : IDisposable
    {
        private const string FirstTrackTag = "Selfie";
        private const int FirstRandomShotAmount = 3;
        private const int SecondRandomShotAmount = 5;

        private readonly ILogger log;
        private readonly ReelDirectorConfig reelDirectorConfig;
        private readonly List<GameObject> virtualCameraList = new (); // All Instantiated Camera
        private readonly GameObject cameraRoot;
        private readonly ReelSceneInfo reelSceneInfo;

        private bool disposed = false;
        private Vector3 initCameraTargetLocation = Vector3.one;
        private List<List<BaseReelCamera>> cameraTrackList = new ();
        private CinemachineBrain cinemachineBrain;
        private CinemachineBlendDefinition cinemachineBrainStyle;
        private Pose initCameraPose;

        public ReelDirector(
            ReelDirectorConfig reelDirectorConfig,
            ReelSceneInfo reelSceneInfo,
            ILoggerFactory loggerFactory)
        {
            log = loggerFactory.CreateLogger<ReelDirector>();

            this.reelDirectorConfig = reelDirectorConfig;
            this.reelSceneInfo = reelSceneInfo;

            cameraRoot = new GameObject("CameraRoot");
        }

        ~ReelDirector()
        {
            Dispose(false);
        }

        public int CameraTrackCount => cameraTrackList.Count;

        public void Build(IEnumerable<ReelCameraTag> cameraTagList, Vector3 initCameraTargetLocation, Pose initCameraPose)
        {
            if (this.initCameraTargetLocation == initCameraTargetLocation)
            {
                return;
            }

            ResetDirector();

            this.initCameraPose = initCameraPose;
            cameraTrackList = reelSceneInfo.RandomTrack ?
                ProduceGeneralTracks(cameraTagList, initCameraTargetLocation) :
                ProducePresetTracks(cameraTagList, initCameraTargetLocation);

            this.initCameraTargetLocation = initCameraTargetLocation;
        }

        public IEnumerator PlayTrack(int trackIndex, float recordDuration, Transform cameraTarget = null)
        {
            IEnumerator PlayCamera(BaseReelCamera camera, float duration)
            {
                camera.gameObject.SetActive(true);
                camera.Play(cameraTarget);
                yield return new WaitForSeconds(duration);
                camera.gameObject.SetActive(false);
            }

            var cameraList = cameraTrackList[trackIndex];

            if (cameraList.Count == 0)
            {
                throw new InvalidOperationException("Camera List is empty");
            }

            PreserveContext();
            virtualCameraList.ForEach(camera => camera.SetActive(false));

            log.LogDebug("Start Play");
            int i = 0;
            var remainingDuration = recordDuration;
            while (remainingDuration > 0f)
            {
                log.LogDebug($"{i} is playing.");

                float duration = Mathf.Min(remainingDuration, cameraList[i].Duration);
                yield return PlayCamera(cameraList[i], duration);
                remainingDuration -= duration;

                // In case we run out of virtual cameras too early
                i = (i + 1) % cameraList.Count;
                log.LogDebug(remainingDuration <= 0f ? "End" : "To Next Camera");
            }

            RestoreContext();
        }

        public void RestoreContext()
        {
            if (cinemachineBrain != null)
            {
                cinemachineBrain.m_DefaultBlend = cinemachineBrainStyle;
                cinemachineBrain = null;
            }

            CameraCache.OnMainCameraChanged -= OnMainCameraChanged;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    ResetDirector();
                    RestoreContext();
                    UnityEngine.Object.Destroy(cameraRoot);
                }

                disposed = true;
            }
        }

        private void ResetDirector()
        {
            virtualCameraList.ForEach(camera => UnityEngine.Object.Destroy(camera));
            virtualCameraList.Clear();
            cameraTrackList.Clear();
            log.LogInformation("ResetDirector Done");
        }

        private List<List<BaseReelCamera>> ProduceGeneralTracks(IEnumerable<ReelCameraTag> cameraTag, Vector3 avatarLocation)
        {
            var track = ProducePresetTracks(cameraTag, avatarLocation);

            track.AddRange(new List<List<BaseReelCamera>>()
            {
                ProduceRandomTrack(FirstRandomShotAmount, cameraTag, avatarLocation),
                ProduceRandomTrack(SecondRandomShotAmount, cameraTag, avatarLocation),
            });
            return track;
        }

        private List<List<BaseReelCamera>> ProducePresetTracks(IEnumerable<ReelCameraTag> cameraTag, Vector3 avatarLocation)
        {
            var firstTrackTag = reelDirectorConfig.GetReelCameraTag(FirstTrackTag);

            if (firstTrackTag == null)
            {
                throw new InvalidOperationException("Produce Preset Tracks Failed. No first track tag found.");
            }

            BaseReelCamera firstCameraPrefab = reelDirectorConfig.RandomCameraDataList
                .Where(shot => ReelCameraTag.AnyIntersection(shot.CameraTags, firstTrackTag))
                .Select(shot => shot.Camera).FirstOrDefault();

            if (firstCameraPrefab == null)
            {
                throw new InvalidOperationException("Produce Preset Tracks Failed. No first track camera found.");
            }

            var firstCameraInstance = CloneReelCam(firstCameraPrefab, avatarLocation);

            firstCameraInstance.transform.SetPositionAndRotation(initCameraPose.position, initCameraPose.rotation);

            var firstTrack = new List<BaseReelCamera>() { firstCameraInstance };
            var presetTrack = reelDirectorConfig.PresetTrackDataList.Where(shot => ReelCameraTag.AllIntersection(shot.CameraTags, cameraTag))
                                .Select(shot => CloneReelCam(shot.Camera, avatarLocation)).ToList();

            return new List<List<BaseReelCamera>>()
            {
                firstTrack,
                presetTrack,
            };
        }

        private List<BaseReelCamera> ProduceRandomTrack(int randomTrackAmount, IEnumerable<ReelCameraTag> cameraTag, Vector3 avatarLocation)
        {
            System.Random random = new System.Random();
            var track = reelDirectorConfig.RandomCameraDataList
                .Where(cameraData => ReelCameraTag.AnyIntersection(cameraData.CameraTags, cameraTag))
                .Select(cameraData => CloneReelCam(cameraData.Camera, avatarLocation).GetComponent<BaseReelCamera>())
                .Where(reelCamera => reelCamera != null)
                .OrderBy(reelCamera => random.Next())
                .Take(randomTrackAmount)
                .ToList();

            if (track.Count == 0)
            {
                throw new InvalidOperationException("Produce Random Track Failed. No corresponding random track found.");
            }

            return track;
        }

        private BaseReelCamera CloneReelCam(BaseReelCamera vCam, Vector3 avatarLocation)
        {
            var cloned = UnityEngine.Object.Instantiate(
                vCam,
                avatarLocation + vCam.transform.position,
                vCam.transform.rotation,
                cameraRoot.transform);

            cloned.gameObject.SetActive(false);
            virtualCameraList.Add(cloned.gameObject);
            return cloned;
        }

        private void PreserveContext()
        {
            CameraCache.OnMainCameraChanged += OnMainCameraChanged;
            cinemachineBrain = CameraCache.Main.GetComponent<CinemachineBrain>();
            cinemachineBrainStyle = cinemachineBrain.m_DefaultBlend;
            cinemachineBrain.m_DefaultBlend = reelDirectorConfig.BlendDefinition;
        }

        private void OnMainCameraChanged(UnityEngine.Camera newMainCamera)
        {
            // If you saw the fallowing exception, that means the assumption that the main camera
            // does not chang while using ReelDirector is broken.
            throw new Exception("Main Camera Changed");
        }
    }
}
